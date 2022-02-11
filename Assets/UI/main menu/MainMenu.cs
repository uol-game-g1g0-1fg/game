using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    #region Property Inspector Variables
    [SerializeField] GameObject m_showOnDeath;
    [SerializeField] GameObject[] m_hideOnDeath;
    #endregion

    public void StartGame()
    {
        SceneManager.LoadScene("GameMap");
    }

    public void GameMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowOnDeath()
    {
        if (m_showOnDeath)
        {
            m_showOnDeath.SetActive(true);
        }
    }

    public void HideOnDeath()
    {
        if (m_hideOnDeath.Length > 0)
        {
            foreach (GameObject gameObject in m_hideOnDeath)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
