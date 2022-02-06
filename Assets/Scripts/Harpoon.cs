using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour {

    [SerializeField] GameEvent OnHit;

    void Update() {
        Debug.Log(GetComponent<Rigidbody>().velocity);
    }
    void OnCollisionEnter(Collision other) {
        Debug.Log("Harpoon hit: " + other.gameObject.name);
        OnHit?.Invoke();
        gameObject.SetActive(false);
    }
}
