﻿using UnityEngine;

public class PickupController : MonoBehaviour {
    float pickupRadius;
    bool enableAnimation;
    GameObject player;
    
    [SerializeField] GameEvent OnPickup;
    [SerializeField] float journeyTime = 1.0f;
    
    void Update() {
        if (!enableAnimation) return;

        if (Vector3.Distance(transform.position, player.transform.position) < .1f) {
            PublishEventAndDestroy();
            return;
        }
        
        transform.position = Vector3.Slerp(transform.position, player.transform.position, journeyTime);
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Player") {
            enableAnimation = true;
            player = collider.gameObject;
        }
    }

    void PublishEventAndDestroy() {
        OnPickup?.Invoke();
        DestroyImmediate(this.gameObject);
    }
}