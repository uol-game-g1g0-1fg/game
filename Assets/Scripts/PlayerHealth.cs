using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    #region Variables
    int m_GaugeCrackImgIndex;
    const float minHealth = 0.0f;
    const float maxHealth = 100.0f;
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
            OnPlayerDeath.Invoke();
        }
    }

    public void Heal(float value)
    {
        m_Health += value;
        
        if (m_Health >= maxHealth) {
            m_Health = maxHealth;
            ResetGauge();
            return;
        }

        UpdateGauge();
    }

    private void ResetGauge() {
        if (!m_Health.Equals(maxHealth)) return;
        
        foreach (var gaugeCrack in m_GaugeCracks)
        {
            gaugeCrack.enabled = false;
        }

        m_GaugeCrackImgIndex = -1;
    }

    private void UpdateGauge()
    {
        if (m_GaugeCracks.Length < 1 || m_Health == maxHealth)
            return;

        var gaugeCrackImgIndex = MapToArrayRange(m_Health, m_GaugeCracks.Length);
        Debug.Log("Guage Index: " + gaugeCrackImgIndex);

        if (m_GaugeCrackImgIndex == gaugeCrackImgIndex)
            return;
        
        if (m_GaugeCrackImgIndex < gaugeCrackImgIndex) {
            OnGaugeCrack?.Invoke();
        }
        
        m_GaugeCrackImgIndex = gaugeCrackImgIndex;
        for (var i = 0; i < m_GaugeCracks.Length; i++) {
            m_GaugeCracks[i].enabled = (i <= gaugeCrackImgIndex) ? true : false;
        }
    }

    private static int MapToArrayRange(float healthValue, int numImages)
    {
        var result = minHealth + ((float)numImages - minHealth) * ((healthValue - maxHealth) / (minHealth - maxHealth));
        return Mathf.Clamp(Mathf.FloorToInt(result), 0, numImages - 1);
    }

    public bool IsDead()
    {
        return m_Health <= 0.0f;
    }
}
