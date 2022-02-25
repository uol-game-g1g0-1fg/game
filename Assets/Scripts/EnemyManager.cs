using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    readonly HashSet<Enemy> enemies = new HashSet<Enemy>();
    
    // Allows an enemy to add itself to the enemy manager
    public void Add(Enemy obj) => enemies.Add(obj);
    // Allows an enemy to remove itself from the enemy manager
    public void Remove(Enemy obj) => enemies.Remove(obj);

    // Evaluates if any enemy is attacking the player
    public bool AnyEnemyAttacking() {
        return enemies.Any(enemy => enemy.State == Enemy.StateTypes.ATTACK);
    }

    // Evaluates if this enemy is attacking the player
    public bool IsEnemyAttacking(Enemy obj) {
        return enemies.Contains(obj) && obj.State == Enemy.StateTypes.ATTACK;
    }

    public bool IsEnemyPlant(Enemy obj)
    {
        if (!enemies.Contains(obj)) { return false; }

        string[] enemyPlantTags = { "EnemyPlant", "EnemyPlant1", "EnemyPlant2" };

        foreach (var tag in enemyPlantTags)
        {
            if (obj.CompareTag(tag))
                return true;
        }
        return false;
    }
}