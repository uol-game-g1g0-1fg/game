using UnityEngine;

public class PickupController : MonoBehaviour {
    float pickupRadius;
    bool enableAnimation;
    GameObject arm;
    GameObject player;
    PlayerMotor playerMotor;
    
    [SerializeField] GameEvent OnPickup;
    [SerializeField] float journeyTime = 1.0f;

    void Awake() {
        player = GameObject.FindWithTag("Player");
        playerMotor = player.GetComponent<PlayerMotor>();
    }
    
    void Update() {
        if (!enableAnimation) return;

        if (Vector3.Distance(transform.position, arm.transform.position) < .1f) {
            OnPickup?.Invoke();
            return;
        }

        this.transform.parent = arm.transform;
        transform.position = Vector3.Slerp(transform.position, arm.transform.position, journeyTime);
    }

    void OnTriggerEnter(Collider collider) {
        // Debug.Log("Pickup collided with " + collider.gameObject.name);
        // Debug.Log(playerMotor.IsArmExtended());
        if (collider.gameObject.CompareTag("MechArm") && playerMotor.IsArmExtended()) {
            enableAnimation = true;
            arm = collider.gameObject;
        }
    }

    void DestroyPickup() {
        Destroy(this.gameObject);
    }
}
