using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlantHealth : MonoBehaviour
{
    #region Property Inspector Variables
    [Header("Enemy Plant Health")]
    [SerializeField] private float m_Health;
    #endregion

    #region Variables
    private bool m_IsDead = false;
    private bool m_EnemyAttacked = false;
    #endregion

    public void TakeDamage(float damage)
    {
        m_Health -= damage;
        m_EnemyAttacked = true;
    }

    public float GetHealth()
    {
        return m_Health;
    }

    public bool IsDead()
    {
        return m_Health <= 0.0f || m_IsDead;
    }

    // Called at the end of the animation when it hits the event tag.
    private void SetIsDead()
    {
        m_IsDead = true;
    }

    public bool HasReceivedDamage()
    {
        bool enemyAttacked = m_EnemyAttacked;
        m_EnemyAttacked = false;
        return enemyAttacked;
    }
}
