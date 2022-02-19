using UnityEngine;

public class PickupController : MonoBehaviour {
    float pickupRadius;
    bool enableAnimation;
    float magnetTime = 1.0f;
    
    Transform arm;
    PlayerMotor playerMotor;
    
    [SerializeField] GameEvent OnPickup;
    [SerializeField] public float value = 10f;

    public enum Type {
        CORE,
        HEALTH,
        ENERGY,
        SCORE
    }

    public Type type;

    void Awake() {
        playerMotor = GameObject.FindWithTag("Player").GetComponent<PlayerMotor>();
    }
    
    void Update() {
        if (!enableAnimation) return;

        if (Vector3.Distance(transform.position, arm.transform.position) < .1f) {
            // When the pickup is close enough to the arm, change it's state.  It has been picked up.
            playerMotor.SetPickedUpItem(this);
            return;
        }

        this.transform.parent = arm.transform;
        transform.position = Vector3.Slerp(transform.position, arm.position, magnetTime);
    }

    void OnTriggerEnter(Collider collider) {
        // Debug.Log("Pickup collided with " + collider.gameObject.name);
        // Debug.Log(playerMotor.IsArmExtended());
        if (collider.gameObject.CompareTag("MechArm") && playerMotor.IsArmExtended()) {
            // Move the item slightly towards the arm like a magnet
            enableAnimation = true;
            arm = collider.gameObject.transform;
        }
    }

    public void Consume() {
        Destroy(this.gameObject);
        // Other behavior here?
    }
}
