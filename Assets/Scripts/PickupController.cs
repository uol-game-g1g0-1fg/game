using UnityEngine;

public class PickupController : MonoBehaviour {
    float pickupRadius;
    public bool enableAnimation;
    float magnetTime = 0.4f;
    
    public Transform arm;
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

    public void Consume() {
        Destroy(this.gameObject);
        // Other behavior here?
    }
}
