using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    #region Property Inspector Variables
    [SerializeField] GameObject m_PauseMenu;
    [SerializeField] GameObject m_ShowOnDeath;
    [SerializeField] GameObject m_ShowOnWin;
    [SerializeField] GameObject[] m_HideOnFinish;
    #endregion

    #region Variables
    public static bool m_IsGamePaused = false;
    public static bool m_IsShowingCanvas = false;
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !m_IsShowingCanvas)
        {
            if (m_IsGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        m_PauseMenu.SetActive(false);
        m_IsGamePaused = false;
        Time.timeScale = 1.0f;
    }

    public void PauseGame()
    {
        m_PauseMenu.SetActive(true);
        m_IsGamePaused = true;
        Time.timeScale = 0.0f;
    }

    public void StartGame()
    {
        m_IsShowingCanvas = false;

        SceneManager.LoadScene("GameMap");
    }

    public void GameMainMenu()
    {
        if (m_IsGamePaused)
        {
            m_IsGamePaused = false;
            Time.timeScale = 1.0f;
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void ShowOnDeath()
    {
        if (m_ShowOnDeath)
        {
            m_IsShowingCanvas = true;

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
            m_IsShowingCanvas = true;

            m_ShowOnWin.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
