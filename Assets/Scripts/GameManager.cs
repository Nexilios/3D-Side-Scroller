using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject stageCompleteMenu;
    public GameObject firstPuzzleTimeline;
    public PlayerController player;
    public static bool IsPhysicsPaused = false;
    public static GameManager Instance;
    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        IsPhysicsPaused = false;
    }

    public void StartFirstPuzzleCutscene()
    {
        if (!firstPuzzleTimeline) return;
        
        firstPuzzleTimeline.SetActive(true);
    }

    public void DestroyCurrentTimeline(GameObject tl)
    {
        if (tl) Destroy(tl);
    }
    
    public void GameOver()
    {
        IsPhysicsPaused = true;
        Time.timeScale = 0;
        gameOverMenu.SetActive(true);
    }

    public void StageComplete()
    {
        IsPhysicsPaused = true;
        Time.timeScale = 0;
        stageCompleteMenu.SetActive(true);
    }

    private void DisablePlayerMovement()
    {
        if (!player) return;
        player.DisableGameplayInput();
    }

    private void EnablePlayerMovement()
    {
        if (!player) return;
        player.EnableGameplayInput();
    }

    public void TogglePauseMenu()
    {
        if (!pauseMenu) return;
        
        if (!IsPhysicsPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }
    
    private void PauseGame()
    {
        if (!pauseMenu) return;
        
        IsPhysicsPaused = true;
        Time.timeScale = 0;
        DisablePlayerMovement();
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        if(!pauseMenu) return;
        
        IsPhysicsPaused = false;
        Time.timeScale = 1;
        EnablePlayerMovement();
        pauseMenu.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}