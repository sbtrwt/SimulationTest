// MainMenu.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button buttonStart;
    [SerializeField] private Button buttonExit;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject timer;
    [SerializeField] private Button buttonRestart;
    private void Start()
    {
        buttonExit.onClick.AddListener(ExitGame);
        buttonStart.onClick.AddListener(StartGame);
        buttonRestart.onClick.AddListener(RestartGame);
    }
    public void StartGame()
    {
        menuPanel.SetActive(false);
        timer.SetActive(true);
        //SceneManager.LoadScene("GameScene");
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
