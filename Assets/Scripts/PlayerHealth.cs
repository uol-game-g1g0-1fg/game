using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    #region Variables
    private int m_GaugeCrackImgIndex = -1;
    #endregion

    #region Property Inspector Variables
    [Header("Player Health")]
    [SerializeField] float m_Health = 100.0f;
    [SerializeField] Image[] m_GaugeCracks;

    [Header("Events")]
    [SerializeField] GameEvent OnGaugeCrack;
    [SerializeField] GameEvent OnPlayerDeath;
    #endregion

    private void Start()
    {
        ResetGauge();
    }

    public void TakeDamage(float damage)
    {
        m_Health -= damage;

        UpdateGauge();

        if (m_Health <= 0.0f)
        {
            // Disable the ability to control the submarine
            gameObject.GetComponent<PlayerMotor>().enabled = false;
            OnPlayerDeath.Invoke();
        }
    }

    private void ResetGauge()
    {
        if (m_Health.Equals(100.0f))
        {
            foreach (var gaugeCrack in m_GaugeCracks)
            {
                gaugeCrack.enabled = false;
            }
        }
    }

    private void UpdateGauge()
    {
        if (m_GaugeCracks.Length < 1)
            return;

        int gaugeCrackImgIndex = MapToArrayRange(m_Health, m_GaugeCracks.Length);
        if (gaugeCrackImgIndex < 0)
            return;

        if (m_GaugeCrackImgIndex == gaugeCrackImgIndex)
            return;

        m_GaugeCrackImgIndex = gaugeCrackImgIndex;
        m_GaugeCracks[m_GaugeCrackImgIndex].enabled = true;
        OnGaugeCrack.Invoke();
    }

    private static int MapToArrayRange(float healthValue, int numImages)
    {
        const float minHealth = 0.0f;
        const float maxHealth = 100.0f;
        float result = minHealth + ((float)numImages - minHealth) * ((healthValue - maxHealth) / (minHealth - maxHealth));
        return Mathf.Clamp(Mathf.FloorToInt(result), 0, numImages - 1);
    }

    public bool IsDead()
    {
        return m_Health <= 0.0f;
    }
}
