using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour {

    [SerializeField] GameEvent OnHit;
    bool garbageCollection = false;
    
    void Update() {
        if (garbageCollection) {
            DestroyImmediate(this.gameObject);
        }
    }
    void OnCollisionEnter(Collision other) {
        OnHit?.Invoke();
        garbageCollection = true;
    }
}
