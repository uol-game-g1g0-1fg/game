using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : MonoBehaviour {
    [SerializeField] float propellerSpeed = 0.3f;

    void Update() {
        transform.Rotate(new Vector3(10, 0, 0) * propellerSpeed);
    }
}
