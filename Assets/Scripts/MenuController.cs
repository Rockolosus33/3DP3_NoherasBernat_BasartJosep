using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static bool m_IsInGameOver, m_IsInPause;
    public Button RestartButton;
    public GameObject m_GameOverCanvas;
    public static MenuController Instance;
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        m_IsInGameOver = false;
        m_IsInPause = false;

        if (m_GameOverCanvas != null)
            DisableGameOverCanvas();
    }
    private void Update()
    {
        if (GameManager.GetGameManager().m_Player.m_GeneralLifes == 0)
        {
            RestartButton.enabled = false;
            RestartButton.gameObject.SetActive(false);
        }
    }


    public void EnableGameOverCanvas()
    {
        if (m_GameOverCanvas != null)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0.0f;
            
            m_IsInGameOver = true;
            m_GameOverCanvas.SetActive(true);
        }
    }

    public void DisableGameOverCanvas()
    {
        if (m_GameOverCanvas != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1.0f;
            
            m_GameOverCanvas.SetActive(false);
        }
    }

    public void Restart()
    {
        m_IsInPause = false;
        m_IsInGameOver = false;
        DisableGameOverCanvas();

        GameManager.GetGameManager().RestartGame();
    }


    public void Exit()
    {
        Application.Quit();
    }
}