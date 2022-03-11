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
    [SerializeField] ParticleSystem[] impactVFX;
    #endregion

    private void OnCollisionEnter(Collision other)
    {
        ParticleSystem impactVFXSelected = impactVFX[0];

        if (other.gameObject.GetComponent<EnemyPlantHealth>())
        {
            other.gameObject.GetComponent<EnemyPlantHealth>().TakeDamage(damageToEnemyPlant);

            foreach (var particle in impactVFX)
            {
                if (other.gameObject.CompareTag(particle.tag))
                {
                    impactVFXSelected = particle;
                }
            }
        }

        Debug.Log("Harpoon hit: " + other.gameObject.name);
        
        // Instantiate a small particle effect
        var contact = other.contacts[0];
        Instantiate(impactVFXSelected, contact.point, Quaternion.Euler(contact.normal));
        OnHit.Invoke();
        gameObject.SetActive(false);
    }
}