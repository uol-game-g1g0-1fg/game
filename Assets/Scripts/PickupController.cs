using UnityEngine;

public class PickupController : MonoBehaviour {
    float pickupRadius;
    bool enableAnimation;
    GameObject arm;
    
    [SerializeField] GameEvent OnPickup;
    [SerializeField] float journeyTime = 1.0f;
    
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
        if (collider.gameObject.tag == "MechArm") {
            enableAnimation = true;
            arm = collider.gameObject;
        }
    }

    void DestroyPickup() {
        Destroy(this.gameObject);
    }
}
