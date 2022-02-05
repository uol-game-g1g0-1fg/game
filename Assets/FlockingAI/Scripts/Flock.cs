using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour {
    public FlockManager flockManager;
    float speed;
    bool turning;
    
    void Start() {
        speed = Random.Range(flockManager.minSpeed, flockManager.maxSpeed);
    }

    void Update() {
        // Make sure the fish doesn't go out of bounds
        var b = new Bounds(flockManager.transform.position, flockManager.swimLimits * 2);
        RaycastHit hit = new RaycastHit();
        Vector3 direction = Vector3.zero;
        
        if (!b.Contains(transform.position)) {
            turning = true;
            direction = flockManager.transform.position - transform.position;
        } else if (Physics.Raycast(transform.position, this.transform.forward * 50, out hit)) {
            turning = true;
            direction = Vector3.Reflect(this.transform.forward, hit.normal);
        } else {
            turning = false;
        }

        if (turning) {
            direction = flockManager.transform.position - transform.position;
            transform.rotation = UpdateDirection(direction);
        }
        
        if (Random.Range(0, 100) < 10) {
            speed = Random.Range(flockManager.minSpeed, flockManager.maxSpeed);
        }
        
        if (Random.Range(0, 100) < 20) {
            ApplyRules();
        }

        transform.Translate(0, 0, Time.deltaTime * speed);
    }

    void ApplyRules() {
        GameObject[] gos;
        gos = flockManager.allFish;
        
        // Average center of the fish
        var vCenter = Vector3.zero;
        // Average avoidance vector of the fish
        var vAvoid = Vector3.zero;
        var gSpeed = 0f;
        var groupSize = 0;
        float nDistance;

        foreach (GameObject go in gos) {
            if (go == this.gameObject) continue;
            
            nDistance = Vector3.Distance(go.transform.position, this.transform.position);
            
            if (!(nDistance <= flockManager.neighbourDistancce)) continue;
            
            vCenter += go.transform.position;
            groupSize++;

            if (nDistance < 1f) {
                vAvoid += (this.transform.position - go.transform.position);
            }

            var anotherFlock = go.GetComponent<Flock>();
            gSpeed += anotherFlock.speed;
        }

        if (groupSize <= 0) return;
        
        vCenter = vCenter / groupSize + flockManager.goalPos - this.transform.position;
        speed = gSpeed / groupSize;

        var direction = (vCenter + vAvoid) - transform.position;
        if (direction != Vector3.zero) {
            transform.rotation = UpdateDirection(direction);
        }
    }

    Quaternion UpdateDirection(Vector3 direction) {
        return Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            flockManager.rotationSpeed * Time.deltaTime);
    }
}
