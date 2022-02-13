using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    [Header("General")] 
    [SerializeField] float boostSpeed = 40f;
    [SerializeField] float moveSpeed = 40f;
    [SerializeField] float floatSpeed = 40f;

    [Header("Keyboard Settings")]
    [SerializeField] float torque = 80f;
    [SerializeField, Range(0, 60)] float maxPitch = 15f;
    [SerializeField, Tooltip("Clamp rotation so that the player cannot face the camera.")] 
    bool useClampRotation = true;
    [SerializeField, Tooltip(("Use WA and WD to move left/right, W/D up/down"))]
    bool useControlScheme2;
    
    [Header("Snap Left/Right Settings")]
    [SerializeField, Tooltip("Snap rotation to 90 degrees")] 
    bool useSnapRotation = true;
    [SerializeField, Range(0, 5), Tooltip("Time player is idle before snapping left or right")] 
    float snapCooldownTimer;
    float snapDirection;
    bool snapCooldown;

    [Header("Boost Settings")]
    [SerializeField, Range(0, 10), Tooltip("Time between boost usage")] 
    float boostCooldownTimer = 4f;
    bool boostCooldown;
    bool boostPressed;
    
    [Header("Mouse Settings")]
    [SerializeField] bool useMouseForLookAt = true;
    [SerializeField] bool useMouseForBallast = true;
    [SerializeField, Range(0, 10)] int mouseLookSpeed = 2;
    [SerializeField, Range(0, 50)] float mouseDistanceToTarget = 35f;

    [Header("Submarine Model")]
    [SerializeField] GameObject model;
    [SerializeField] float collisionForce = 250f;
    
    [Header("Harpoon")]
    [SerializeField, Tooltip("Harpoon projectile prefab.")] GameObject harpoon;
    [SerializeField, Tooltip("Location to spawn harpoon projectiles.")] GameObject harpoonSpawnPoint;
    [SerializeField, Tooltip("Harpoon that is attached to the submarine model.")] GameObject fixedHarpoon;
    [SerializeField] float harpoonForce = 8000f;
    [SerializeField, Range(0.1f, 1f)] float fireRate = 0.1f;
    [SerializeField] GameEvent OnFire;
    float nextFire = 0.0f;

    [Header("Mechanical Arm")] 
    [SerializeField] GameObject arm;
    [SerializeField] float armSpeed = 0.3f;
    Vector3 armFullyExtendedPosition = new Vector3(0.85f, 0, 0);
    [SerializeField] ArmState armState = ArmState.RESET;
    
    [Header("Events")] 
    [SerializeField] GameEvent OnCollision;
    [SerializeField] GameEvent OnEnableCore;
    [SerializeField] GameEvent OnExtendArm;
    [SerializeField] GameEvent OnRetractArm;
    [SerializeField] GameEvent OnPlayerWin;

    [Header("Player Stats")] 
    [SerializeField] bool enableCore = false;
    bool hasCore = false;

    Camera mainCamera;

    float ballast;

    Rigidbody rb;
    PlayerControls playerControls;
    PlayerHealth playerHealth;

    private GameObject waterSurface;

    enum ArmState {
        RESET,
        EXTEND,
        RETRACT
    }

    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        playerControls = new PlayerControls();
        playerHealth = GetComponent<PlayerHealth>();
        mainCamera = Camera.main;
        waterSurface = GameObject.FindGameObjectWithTag("WaterSurface");

        playerControls.Submarine.Enable();
        playerControls.Submarine.Boost.performed += HandleBoost;
        playerControls.Submarine.Boost.canceled += HandleBoost;
        playerControls.Submarine.Ballast.performed += HandleBallast;
        playerControls.Submarine.Ballast.canceled += HandleBallast;
        playerControls.Submarine.Harpoon.performed += FireHarpoon;
        playerControls.Submarine.Arm.performed += FireArm;
    }

    void Update() {
        // Snapping and Look With Mouse don't work together very well
        if (useSnapRotation) useMouseForLookAt = false;
        if (useMouseForLookAt) useSnapRotation = false;

        if (useMouseForLookAt) {
            float rotateSpeed = (1 + (mouseLookSpeed / 10)) * Time.deltaTime;

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = mouseDistanceToTarget;

            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
    
            Quaternion targetRotation = Quaternion.LookRotation(worldPos - model.transform.position, Vector3.up);
            targetRotation.z = 0;

            model.transform.rotation = Quaternion.Slerp(model.transform.rotation, targetRotation, rotateSpeed);

            if (useMouseForBallast) {
                ballast = (worldPos.y > model.transform.position.y) ? -1 : 1;
            }
        }

        // Harpoon updates
        if (nextFire <= Time.time) {
            // Show the mounted harpoon, it is now ready to fire again
            fixedHarpoon.SetActive(true);
        }

        // Mechanical arm handle pickups
        if (armState == ArmState.RESET) {
            // Do something with the items retrieved
            // Rework this system as the pickup types are expanded
            if (hasCore && !enableCore) {
                EnableCore();
            }
        }
        
        // Mechanical arm move updates
        if (armState == ArmState.EXTEND && Vector3.Distance(arm.transform.position, model.transform.position) > 0.7f) {
            armState = ArmState.RETRACT;
            OnRetractArm?.Invoke();
            RetractArm();
        } else if (armState == ArmState.RETRACT && Vector3.Distance(arm.transform.position, model.transform.position) < 0.1f) {
            armState = ArmState.RESET;
            arm.transform.position = model.transform.position;
        } else if (armState == ArmState.RETRACT) {
            RetractArm();
        } else if (armState == ArmState.EXTEND) {
            ExtendArm();
        }
    }

    void FixedUpdate() {
        if (playerHealth.IsDead() || ReachedSurface()) return;

        Vector2 inputVector = playerControls.Submarine.Movement.ReadValue<Vector2>();

        HandleMovement(inputVector);
        HandleRotation(inputVector);
        HandlePitch(inputVector);
    }

    void OnCollisionEnter(Collision col) {
        // Do not bounce on the Floor
        if (col.gameObject.tag != "Ground") {
            // Calculate Angle Between the collision point and the player
            var dir = col.contacts[0].point - transform.position;
            // We then get the opposite (-Vector3) and normalize it
            dir = -dir.normalized;
            // And finally we add force in the direction of dir and multiply it by force. 
            // This will push back the player
            var playerForce = Mathf.Clamp(rb.velocity.magnitude, 0.02f, 1f);
            rb.AddForce(dir * playerForce * collisionForce);
            
            OnCollision?.Invoke();
        }
    }

    public bool IsArmExtended() {
        return armState == ArmState.EXTEND;
    }

    void HasCore() {
        hasCore = true;
    }
    
    void EnableCore() {
        Debug.Log("Enabled Core");
        OnEnableCore?.Invoke();
        enableCore = true;
    }

    void HandleMovement(Vector2 inputVector) {
        float direction;
        
        if (useControlScheme2) {
            inputVector = RemapForControlScheme2(inputVector);
            direction = 1;
        } else {
            float actualYaw = ConstrainAngle(model.transform.eulerAngles.y);
            direction = (actualYaw < 0) ? -1 : 1;
        }
        
        if (inputVector.y > 0) {
            rb.AddForce(transform.right * direction * moveSpeed);
        } else if (inputVector.y < 0) {
            rb.AddForce(-transform.right * direction * moveSpeed);
        }

        if (!enableCore) return;

        rb.AddForce(-ballast * transform.up * floatSpeed);
    }

    Vector2 RemapForControlScheme2(Vector2 inputVector) {
        Vector2 playerVector = inputVector;

        if (useControlScheme2) {
            var inputX = inputVector.x;
            var inputY = inputVector.y;
            // Remap input vector as per Berta's suggestion
            if (inputX == 0) {
                // Player is only pressing a key in the Y direction
                // so move up and down, need to reverse the vector
                playerVector = Vector2.zero;
                if (inputY != 0) {
                    ballast = -inputY;
                } else {
                    if (!boostPressed) {
                        ballast = 0;
                    }
                }
            } else if (inputX != 0 && inputY > 0) {
                // Player is attempting to move side to side with inter-cardinal input
                if (inputX > 0) {
                    playerVector = new Vector2(0, 1);
                } else {
                    playerVector = new Vector2(0, -1);
                }

                if (!boostPressed) {
                    ballast = 0;
                }
                
            } else if (inputY < 0) {
                playerVector = Vector2.zero;
            }
        }

        return playerVector;
    }

    void HandleRotation(Vector2 inputVector) {
        model.transform.Rotate(transform.up * inputVector.x * torque * Time.deltaTime, Space.World);

        if (useClampRotation) {
            ClampRotation();
        }
        
        if (useSnapRotation) {
            SnapRotation(inputVector);
        }
    }

    void ClampRotation() {
        // Clamp the rotation so the Submarine cannot face the camera
        float maxRotation = 90;

        Vector3 currentRotation = model.transform.eulerAngles;

        float actualYaw = ConstrainAngle(currentRotation.y);

        if (actualYaw < -maxRotation) {
            currentRotation.y = -maxRotation;
        } else if (actualYaw > maxRotation) {
            currentRotation.y = maxRotation;
        }

        model.transform.rotation = Quaternion.Euler(currentRotation);
    }

    void SnapRotation(Vector2 inputVector) {
        // Snap rotation quickly to left and right
        // What direction were they moving in last?
        if (inputVector.x < 0) {
            snapDirection = -1;
        } else if (inputVector.x > 0) {
            snapDirection = 1;
        }

        if (inputVector != Vector2.zero) {
            snapCooldown = true;
            StopCoroutine("SnapCooldown");
        } else {
            StartCoroutine("SnapCooldown");
        }
        
        // Player has been idle longer than the coroutine, snap them left or right
        if (inputVector.x == 0 && snapCooldown == false) {
            Vector3 angle = new Vector3(0, 90, 0) * snapDirection;
            model.transform.rotation = Quaternion.Lerp(model.transform.rotation, Quaternion.Euler(angle), 2 * Time.deltaTime);
        }
    }

    IEnumerator SnapCooldown() {
        float normalizedTime = 0;
        while(normalizedTime <= 1f) {
            
            normalizedTime += Time.deltaTime / snapCooldownTimer;
            yield return null;
        }

        snapCooldown = false;
    }
    
    void HandlePitch(Vector2 inputVector) {
        Vector3 currentRotation = model.transform.localEulerAngles;
        float actualCorrection = 0f;
        float correctionPace = 5f;

        float actualPitch = ConstrainAngle(currentRotation.x);

        if (inputVector.y != 0 && actualPitch < maxPitch && actualPitch > -maxPitch) {
            actualCorrection = Mathf.Sin(inputVector.y) * 15 * Time.deltaTime;
        } else if (actualPitch < 0) {
            actualCorrection = correctionPace * Time.deltaTime;
        } else if (actualPitch > 0) {
            actualCorrection = -correctionPace * Time.deltaTime;
        }

        Vector3 targetRotation = new Vector3(currentRotation.x + actualCorrection, currentRotation.y, currentRotation.z); 

        model.transform.localRotation = Quaternion.Euler(targetRotation);
    }

    void HandleBallast(InputAction.CallbackContext ctx) {
        ballast = ctx.ReadValue<float>();
    }

    void HandleBoost(InputAction.CallbackContext ctx) {
         if (!enableCore) return;
        
         if (ctx.performed && boostCooldown == false) {
             // Boost is ready and player pressed the button
             rb.AddForce(Vector3.up * boostSpeed, ForceMode.Impulse);
             boostCooldown = true;
             ballast = -1;
             boostPressed = true;
             StartCoroutine("BoostCooldown");
         } else if (ctx.performed) {
             // While the button is pressed, the player will ascend
             ballast = -1;
             boostPressed = true;
         } else if (ctx.canceled) { 
             // Player has released the button
             ballast = 0;
             boostPressed = false;
         }
    }

    IEnumerator BoostCooldown() {
        float normalizedTime = 0;
        while(normalizedTime <= 1f) {
            
            normalizedTime += Time.deltaTime / boostCooldownTimer;
            yield return null;
        }

        boostCooldown = false;
    }

    void FireHarpoon(InputAction.CallbackContext ctx) {
        if (nextFire <= Time.time) {
            nextFire = Time.time + fireRate;
            
            // Hide the mounted harpoon
            fixedHarpoon.SetActive(false);
            // Instantiate the projectile at the position and rotation of this transform
            var clone = Instantiate(harpoon, harpoonSpawnPoint.transform.position, transform.rotation);
            // Give the cloned object an initial velocity along the current object's Z axis
            clone.transform.Rotate(-15, 0, 0);
            clone.GetComponent<Rigidbody>().AddForce(model.transform.forward * harpoonForce);
            OnFire?.Invoke();
        }
    }
    
    void FireArm(InputAction.CallbackContext obj) {
        if (armState != ArmState.RESET) return;
        armState = ArmState.EXTEND;
        OnExtendArm?.Invoke();
    }

    void ExtendArm() {
        arm.transform.position += model.transform.forward * Time.deltaTime * armSpeed;
    }

    void RetractArm() {
        arm.transform.position += -model.transform.forward * Time.deltaTime * armSpeed;
    }
    
    float ConstrainAngle(float angle) {
        // Handle Euler angles coming back as positive values
        // Returns a negative value representation of the angle if the angle is greater than 180 degrees
        return (angle > 180) ? angle - 360 : angle;
    }

    private bool ReachedSurface() {
        Vector3 modelPos = model.transform.position;
        Vector3 waterSurfacePos = waterSurface.transform.position;
        float modelHalfBound = model.transform.localScale.y / 2.0f;

        if (modelPos.y > (waterSurfacePos.y + modelHalfBound)) {
            Vector3 newPosition = new Vector3(modelPos.x, waterSurfacePos.y + modelHalfBound, modelPos.z);
            transform.position = newPosition;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            OnPlayerWin.Invoke();
            return true;
        }

        return false;
    }
}