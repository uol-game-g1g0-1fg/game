using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField] GameEvent onDied;
    bool dead;

    void OnMouseDown() {
        if (!dead) 
          Die();
    }

    void Die() {
        // GetComponent<Animator>().SetBool("Dead", true);
        onDied?.Invoke();
        dead = true;
    }
}
