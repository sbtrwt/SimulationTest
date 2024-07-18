// MainMenu.cs
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button buttonRestart;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject gameOverPanel;
    private void Start()
    {
        buttonRestart.onClick.AddListener(RestartGame);
    }
    public void RestartGame()
    {
      SceneManager.LoadScene(0);
    }
    public void GameOver(string message)
    {
        messageText.text = message;
        gameOverPanel.SetActive(true);
    }

}
