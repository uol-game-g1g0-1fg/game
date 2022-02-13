using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    #region Property Inspector Variables
    [SerializeField] GameObject m_ShowOnDeath;
    [SerializeField] GameObject m_ShowOnWin;
    [SerializeField] GameObject[] m_HideOnFinish;
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
        if (m_ShowOnDeath)
        {
            m_ShowOnDeath.SetActive(true);
        }
    }

    public void HideOnFinish()
    {
        if (m_HideOnFinish.Length > 0)
        {
            foreach (GameObject gameObject in m_HideOnFinish)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void ShowOnWin()
    {
        if (m_ShowOnWin)
        {
            m_ShowOnWin.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
