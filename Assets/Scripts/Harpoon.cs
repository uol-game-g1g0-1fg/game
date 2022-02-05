using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour {

    [SerializeField] GameEvent OnHit;
    bool garbageCollection = false;
    
    void Update() {
        Debug.Log(this.GetComponent<Rigidbody>().velocity);
        if (garbageCollection) {
            DestroyImmediate(this.gameObject);
        }
    }
    void OnCollisionEnter(Collision other) {
        Debug.Log("Harpoon hit: " + other.gameObject.name);
        OnHit?.Invoke();
        garbageCollection = true;
    }
}
