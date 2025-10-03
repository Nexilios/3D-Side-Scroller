using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public PlayerController player;
    public static bool IsPaused = false;
    public static bool IsGameOver = false;
    public static GameManager Instance;

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        IsPaused = false;
        IsGameOver = false;
    }
    
    public void GameOver()
    {
        
    }

    public void StageComplete()
    {
        
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0;
        if (player)
        {
            player.DisableGameplayInput();
        }
        
        if (pauseMenu)
        {
            pauseMenu.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1;
        if (player)
        {
            player.EnableGameplayInput();
        }
        
        if (pauseMenu)
        {
            pauseMenu.SetActive(false);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        /*Time.timeScale = 1;
        pauseMenu.SetActive(false);*/
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}