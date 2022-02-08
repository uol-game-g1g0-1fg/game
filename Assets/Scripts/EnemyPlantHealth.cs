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
    #endregion

    public void TakeDamage(float damage)
    {
        m_Health -= damage;
    }

    public float GetHealth()
    {
        return m_Health;
    }

    public bool GetIsDead()
    {
        return m_IsDead;
    }

    // Called at the end of the animation when it hits the event tag.
    private void SetIsDead()
    {
        m_IsDead = true;
    }
}
