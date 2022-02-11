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
    #endregion

    private void Start()
    {
        ResetHealth();
    }

    public void TakeDamage(float damage)
    {
        m_Health -= damage;
        int gaugeCrackImgIndex = MapToArrayRange(m_Health, m_GaugeCracks.Length);
        //Debug.Log("Previous Img Index: " + m_GaugeCrackImgIndex + ", Current Img Index: " + gaugeCrackImgIndex);

        if (m_GaugeCrackImgIndex != gaugeCrackImgIndex)
        {
            m_GaugeCrackImgIndex = gaugeCrackImgIndex;
            m_GaugeCracks[m_GaugeCrackImgIndex].enabled = true;
            OnGaugeCrack.Invoke();
        }

        if (m_Health <= 0.0f)
        {
            // TODO: Add the player death event
        }
    }

    private void ResetHealth()
    {
        if (m_Health.Equals(100.0f))
        {
            foreach (var gaugeCrack in m_GaugeCracks)
            {
                gaugeCrack.enabled = false;
            }
        }
    }

    private static int MapToArrayRange(float healthValue, int numImages)
    {
        const float minHealth = 0.0f;
        const float maxHealth = 100.0f;
        float result = minHealth + ((float)numImages - minHealth) * ((healthValue - maxHealth) / (minHealth - maxHealth));
        return Mathf.Clamp(Mathf.FloorToInt(result), 0, numImages - 1);
    }
}
