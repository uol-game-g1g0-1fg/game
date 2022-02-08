using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] GameEvent OnPlantDamage;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (gameObject.CompareTag("PlantProjectile"))
            {
                OnPlantDamage.Invoke();
            }
        }

        gameObject.SetActive(false);
    }
}