using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlockManager : MonoBehaviour {

    public GameObject fishPrefab;
    public int numFish = 20;
    public float fishSize = 1f;
    public GameObject[] allFish;
    public Vector3 swimLimits = new Vector3(5, 5, 5);
    public Vector3 goalPos;

    [Header("Fish Settings")] 
    [Range(0f, 5f)] public float minSpeed;
    [Range(0f, 5f)] public float maxSpeed;
    [Range(0f, 10f)] public float neighbourDistancce;
    [Range(0f, 5f)] public float rotationSpeed;
    
    void Start() {
        allFish = new GameObject[numFish];
        for (var i = 0; i < numFish; i++) {
            allFish[i] = Instantiate(fishPrefab, ARandomPositionInRange(), Quaternion.identity);
            allFish[i].GetComponent<Flock>().flockManager = this;
            allFish[i].transform.parent = this.transform;
            allFish[i].transform.localScale *= fishSize;

        }

        goalPos = this.transform.position;
    }

    void Update() {
        if (Random.Range(0, 100) < 10) {
            goalPos = ARandomPositionInRange();
        }
    }

    Vector3 ARandomPositionInRange() {
        return this.transform.position
               + new Vector3(
                   Random.Range(-swimLimits.x, swimLimits.x),
                   Random.Range(-swimLimits.y, swimLimits.y),
                   Random.Range(-swimLimits.z, swimLimits.z)
               );
    }
}
