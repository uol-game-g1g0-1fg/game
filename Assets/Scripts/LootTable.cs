using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyLoot
{
    public GameObject m_LootItem;
    public int m_SpawnProbability;
}

[CreateAssetMenu]
public class LootTable : ScriptableObject
{
    public EnemyLoot[] m_LootItems;

    public GameObject GetLootItem()
    {
        int cumulativeProb = 0;
        int randProb = Random.Range(0, 100);

        foreach (EnemyLoot loot in m_LootItems)
        {
            cumulativeProb += loot.m_SpawnProbability;
            if (randProb > cumulativeProb) continue;
            return loot.m_LootItem;
        }

        return null;
    }
}
