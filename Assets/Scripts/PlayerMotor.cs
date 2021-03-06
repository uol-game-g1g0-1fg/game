using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerEnergy))]
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
    [SerializeField, Range(2, 10)] float snapSpeed = 2f;

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
    [SerializeField] Transform grabPoint;
    [SerializeField] float armSpeed = 0.3f;
    Vector3 armFullyExtendedPosition = new Vector3(0.85f, 0, 0);
    [SerializeField] public ArmState armState = ArmState.RESET;
    PickupController pickup;
    
    [Header("Events")] 
    [SerializeField] GameEvent OnCollision;
    [SerializeField] GameEvent OnEnableCore;
    [SerializeField] GameEvent OnPickupHealth;
    [SerializeField] GameEvent OnPickupEnergy;
    [SerializeField] GameEvent OnPickupScore;
    [SerializeField] GameEvent OnExtendArm;
    [SerializeField] GameEvent OnRetractArm;
    [SerializeField] GameEvent OnPlayerWin;

    [Header("Player Stats")] 
    [SerializeField] bool enableCore = false;

    [Header("Player Damage Settings")]
    [SerializeField] float damageToPlantFromPlayerCollision = 30.0f;
    [SerializeField] float damageToPlayerFromPlantCollision = 10.0f;

    Camera mainCamera;

    float ballast;

    Rigidbody rb;
    PlayerControls playerControls;
    PlayerHealth playerHealth;
    PlayerEnergy playerEnergy;

    private GameObject waterSurface;
    private GameObjectPoolManager objectPoolMgr;

    public enum ArmState {
        RESET,
        EXTEND,
        RETRACT
    }

    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        playerControls = new PlayerControls();
        playerHealth = GetComponent<PlayerHealth>();
        playerEnergy = GetComponent<PlayerEnergy>();
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

    private void Start() {
        objectPoolMgr = GameObjectPoolManager.Instance;
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
            
            // Debug.Log(playerEnergy.Energy);
        }

        // Harpoon updates
        if (nextFire <= Time.time) {
            // Show the mounted harpoon, it is now ready to fire again
            fixedHarpoon.SetActive(true);
        }
        
        // Mechanical arm grabs pickup
        LayerMask mask = LayerMask.GetMask("Pickups");
        var hitColliders = Physics.OverlapSphere(grabPoint.position, 0.3f, mask, QueryTriggerInteraction.Collide);
        if (hitColliders.Length > 0 && IsArmExtended()) {
            var item = hitColliders[0].gameObject.GetComponent<PickupController>();
            item.arm = grabPoint;
            item.enableAnimation = true;
        }

        // Mechanical arm handle pickup if one has been grabbed and pulled all the way in
        if (armState == ArmState.RESET && pickup && pickup.isActiveAndEnabled) {
            HandlePickups();
        }
        
        // Mechanical arm movement updates
        if (armState == ArmState.EXTEND && Vector3.Distance(arm.transform.position, model.transform.position) > 0.7f) {
            armState = ArmState.RETRACT;
            OnRetractArm?.Invoke();
            RetractArm();
        } else if (armState == ArmState.RETRACT && Vector3.Distance(arm.transform.position, model.transform.position) < 0.1f) {
            armState = ArmState.RESET;
            arm.transform.position = model.transform.position;
        } else if (armState == ArmState.RETRACT) {
            RetractArm();
        } else if (armState == ArmState.EXTEND && pickup && pickup.isActiveAndEnabled) {
            armState = ArmState.RETRACT;
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
        if (col.gameObject.tag == "Ground") return;
        
        // Calculate Angle Between the collision point and the player
        var dir = col.contacts[0].point - transform.position;
        // We then get the opposite (-Vector3) and normalize it
        dir = -dir.normalized;
        // And finally we add force in the direction of dir and multiply it by force. 
        // This will push back the player
        var playerForce = Mathf.Clamp(rb.velocity.magnitude, 0.02f, 1f);
        rb.AddForce(dir * playerForce * collisionForce);

        // Apply damage on collision with enemies
        if (col.gameObject.GetComponent<Enemy>())
        {
            Enemy enemy = col.gameObject.GetComponent<Enemy>();
            EnemyManager enemyMgr = gameObject.GetComponent<EnemyManager>();

            if (enemyMgr && enemyMgr.IsEnemyPlant(enemy)) {
                var plantHealth = enemy.GetComponent<EnemyHealth>();
                if (!plantHealth.IsDead() && !playerHealth.IsDead()) {
                    plantHealth.TakeDamage(damageToPlantFromPlayerCollision);
                    playerHealth.TakeDamage(damageToPlayerFromPlantCollision);
                }
            }
            
            if (enemyMgr && enemyMgr.IsEnemyFish(enemy)) {
                var plantHealth = enemy.GetComponent<EnemyHealth>();
                if (!plantHealth.IsDead() && !playerHealth.IsDead()) {
                    plantHealth.TakeDamage(15);
                    playerHealth.TakeDamage(15);
                }
            }
        }

        OnCollision?.Invoke();
    }

    public bool IsArmExtended() {
        return armState != ArmState.RESET;
    }

    public void SetPickedUpItem(PickupController item) {
        pickup = item;
    }

    void HandlePickups() {
        switch (pickup.type) {
            case PickupController.Type.CORE when !enableCore:
                Debug.Log("Enabled Core");
                OnEnableCore?.Invoke();
                enableCore = true;
                break;
            case PickupController.Type.HEALTH:
                Debug.Log("Grabbed a health item");
                OnPickupHealth?.Invoke();
                playerHealth.Heal(pickup.value);
                break;
            case PickupController.Type.ENERGY:
                Debug.Log("Grabbed an energy item worth " + pickup.value);
                OnPickupEnergy?.Invoke();
                playerEnergy.Increase(pickup.value);
                break;
            case PickupController.Type.SCORE:
                Debug.Log("Grabbed an points item that is worth " + pickup.value);
                OnPickupScore?.Invoke();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        pickup.Consume();
    }

    void HandleMovement(Vector2 inputVector) {
        if (playerEnergy.Velocity <= 0) return;

        float direction;
        
        if (useControlScheme2) {
            inputVector = RemapForControlScheme2(inputVector);
            direction = 1;
        } else {
            float actualYaw = ConstrainAngle(model.transform.eulerAngles.y);
            direction = (actualYaw < 0) ? -1 : 1;
        }

        var actualVelocity = moveSpeed * playerEnergy.Power * playerEnergy.Velocity;
        
        if (inputVector.y > 0) {
            rb.AddForce(transform.right * direction * actualVelocity);
        } else if (inputVector.y < 0) {
            rb.AddForce(-transform.right * direction * actualVelocity);
        }

        if (!enableCore) return;

        var verticalVelocity = Remap(floatSpeed * playerEnergy.Power * playerEnergy.Velocity, 0, 30, 15, 30);
        rb.AddForce(-ballast * transform.up * verticalVelocity);
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
            model.transform.rotation = Quaternion.Lerp(model.transform.rotation, Quaternion.Euler(angle), snapSpeed * Time.deltaTime);
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
        if (playerEnergy.Velocity <= 0) return;
        
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
            if (fixedHarpoon) {
                fixedHarpoon.SetActive(false);
            }

            if (!harpoon || !objectPoolMgr) return;

            // Spawn the projectile at the position and rotation of this transform
            var clone = objectPoolMgr.SpawnFromPool(harpoon.tag, harpoonSpawnPoint.transform.position, transform.rotation);
            // Give the cloned object an initial velocity along the current object's Z axis
            clone.transform.Rotate(15, (snapDirection.Equals(1.0f)) ? 180 : 0, 0);
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

    static float ConstrainAngle(float angle) {
        // Handle Euler angles coming back as positive values
        // Returns a negative value representation of the angle if the angle is greater than 180 degrees
        return (angle > 180) ? angle - 360 : angle;
    }

    static float Remap(float playerEnergyVelocity, float aLow, float aHigh, float bLow, float bHigh) {
        // General re-mapping function
        var normalized = Mathf.InverseLerp(aLow, aHigh, playerEnergyVelocity);
        return Mathf.Lerp(bLow, bHigh, normalized);
    }

    private bool ReachedSurface() {
        var modelPos = model.transform.position;
        var waterSurfacePos = waterSurface.transform.position;
        var modelHalfBound = model.transform.localScale.y / 2.0f;

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