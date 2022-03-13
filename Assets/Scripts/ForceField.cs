using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    #region Property Inspector Variables
    [SerializeField] private float m_PullForce;
    #endregion

    #region Variables
    private Enemy m_Enemy;
    #endregion

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        m_Enemy = gameObject.GetComponentInParent<Enemy>();

        if (m_Enemy)
        {
            if (m_Enemy.GetComponent<EnemyHealth>().IsDead() || !other.GetComponent<EnemyManager>().IsEnemyAttacking(m_Enemy))
                return;
        }

        PullObject(other, true);
    }

    private void PullObject(Collider other, bool shouldPull)
    {
        if (!shouldPull) { return; }

        Vector3 vPullDir = transform.position - other.transform.position;
        other.GetComponent<Rigidbody>().AddForce(m_PullForce * vPullDir.normalized * Time.fixedDeltaTime);
    }
}
