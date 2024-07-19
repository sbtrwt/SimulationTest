// Timer.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public TMP_Text timerText;
    [SerializeField] private float maxTime = 300f;
    [SerializeField] private float timeRemaining = 300f;
    [SerializeField] private bool timerIsRunning = false;
    [SerializeField] private GameOverUI gameOverUI ;
    public static float TimeTaken;
    void Start()
    {
        timeRemaining = maxTime;
        gameOverUI.gameObject.SetActive(false);
        timerIsRunning = true;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
                TimeTaken = maxTime - timeRemaining;
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                // Handle game over logic
                gameOverUI.GameOver("Time up !! try again.");
            }
        }
    }

    private void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void EnableTimerCount(bool isEnable) 
    {
        timerIsRunning = isEnable;
    }
    private void OnGameOver()
    {
        EnableTimerCount(false);
    }

    private void OnEnable()
    {
        GameController.OnGameOver += OnGameOver;
       
    }

    private void OnDisable()
    {
        GameController.OnGameOver -= OnGameOver;
     

    }
}
