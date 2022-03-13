using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    #region Property Inspector Variables
    [Header("Enemy Plant Health")]
    [SerializeField] private float m_Health;

    [Header("Enemy Loot Table")]
    [SerializeField] public LootTable m_LootTable;
    #endregion

    #region Variables
    private bool m_IsDead = false;
    private bool m_EnemyAttacked = false;
    private GameObjectPoolManager m_ObjectPoolMgr;
    private GameObjectSpawner m_ObjectSpawner;
    private Vector3 m_LootSpawnPoint;
    #endregion

    public void Start()
    {
        m_ObjectPoolMgr = GameObjectPoolManager.Instance;
        m_ObjectSpawner = GameObjectSpawner.Instance;
    }

    public void TakeDamage(float damage)
    {
        m_Health -= damage;
        m_EnemyAttacked = true;
    }

    public float GetHealth() { return m_Health; }
    public bool IsDead() { return m_Health <= 0.0f || m_IsDead; }

    // Called at the end of the animation when it hits the event tag.
    public void SetIsDead()
    {
        if (!m_IsDead) GenerateLoot();
        m_IsDead = true;
    }

    public bool HasReceivedDamage()
    {
        bool enemyAttacked = m_EnemyAttacked;
        m_EnemyAttacked = false;
        return enemyAttacked;
    }

    private void GenerateLoot()
    {
        if (!m_LootTable) { return; }

        var lootItem = m_LootTable.GetLootItem();
        if (!lootItem) { return; }

        m_LootSpawnPoint = m_ObjectSpawner.FindTransformObjectWithTag("LootSpawnPoint", transform).position;
        m_ObjectPoolMgr.SpawnFromPool(lootItem.tag, m_LootSpawnPoint, Quaternion.identity);
    }
}