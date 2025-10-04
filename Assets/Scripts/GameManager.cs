using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject stageCompleteMenu;
    public GameObject firstPuzzleTimeline;
    public GameObject stageCompleteTimeline;
    public Animator playerCinemachineAnimator;
    public PlayerController player;
    public static bool IsPhysicsPaused = false;
    public static GameManager Instance;

    private GameObject _currentPlayingTimeline;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Cursor.visible = false;
        IsPhysicsPaused = false;
        player = FindFirstObjectByType<PlayerController>();
    }

    public void StartFirstPuzzleCutscene()
    {
        if (!firstPuzzleTimeline) return;
        
        _currentPlayingTimeline = firstPuzzleTimeline;
        firstPuzzleTimeline.SetActive(true);
    }

    public void DestroyCurrentTimeline()
    {
        if (_currentPlayingTimeline) Destroy(_currentPlayingTimeline);
    }
    
    public void GameOver()
    {
        Cursor.visible = true;
        IsPhysicsPaused = true;
        Time.timeScale = 0;
        player.gameObject.SetActive(false);
        gameOverMenu.SetActive(true);
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        IsPhysicsPaused = false;
        Time.timeScale = 1;
    }

    public void StageComplete()
    {
        IsPhysicsPaused = true;
        if (player)
        {
            player.StageComplete();
        }
        
        if (stageCompleteTimeline)
        {
            _currentPlayingTimeline = stageCompleteTimeline;
            stageCompleteTimeline.SetActive(true);
        }

        if (playerCinemachineAnimator)
        {
            playerCinemachineAnimator.enabled = true;
        }
    }
    
    public void ShowStageCompleteMenu()
    {
        Cursor.visible = true;
        IsPhysicsPaused = true;
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
        Cursor.visible = true;
        if (!pauseMenu) return;
        
        IsPhysicsPaused = true;
        Time.timeScale = 0;
        DisablePlayerMovement();
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Cursor.visible = false;
        if(!pauseMenu) return;
        
        IsPhysicsPaused = false;
        Time.timeScale = 1;
        EnablePlayerMovement();
        pauseMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}