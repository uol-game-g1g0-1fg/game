using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour {

    #region Property Inspector Variables
    [Header("Events")]
    [SerializeField] GameEvent OnHit;

    [Header("Damage Points")]
    [SerializeField] float damageToEnemyPlant;
    
    [Header("Particle Effect")]
    [SerializeField] ParticleSystem impactVFX;
    #endregion

    private void Update()
    {
        //Debug.Log(GetComponent<Rigidbody>().velocity);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<EnemyPlantHealth>())
        {
            other.gameObject.GetComponent<EnemyPlantHealth>().TakeDamage(damageToEnemyPlant);
        }

        Debug.Log("Harpoon hit: " + other.gameObject.name);
        
        // Instantiate a small particle effect
        var contact = other.contacts[0];
        Instantiate(impactVFX, contact.point, Quaternion.Euler(contact.normal));
        OnHit.Invoke();
        gameObject.SetActive(false);
    }
}