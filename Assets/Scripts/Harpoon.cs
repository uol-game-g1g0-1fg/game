﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour {

    #region Property Inspector Variables
    [Header("Events")]
    [SerializeField] GameEvent OnHit;

    [Header("Damage Points")]
    [SerializeField] float damageToEnemyPlant;
    #endregion

    private void Update()
    {
        //Debug.Log(GetComponent<Rigidbody>().velocity);
    }

    private void OnCollisionEnter(Collision other)
    {
        // TODO BERTA: Find a better way to query for plant variants
        if (other.gameObject.CompareTag("EnemyPlant") || other.gameObject.CompareTag("EnemyPlant2"))
        {
            other.gameObject.GetComponent<EnemyPlantHealth>().TakeDamage(damageToEnemyPlant);
        }

        Debug.Log("Harpoon hit: " + other.gameObject.name);
        OnHit.Invoke();
        gameObject.SetActive(false);
    }
}