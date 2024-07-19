// MainMenu.cs
using Simulation.Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button buttonStart;
    [SerializeField] private Button buttonExit;
    [SerializeField] private Button buttonOption;
    [SerializeField] private Button buttonOptionClose;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject timer;
    [SerializeField] private Button buttonRestart;
    [SerializeField] private Toggle toggleMusic;
    [SerializeField] private Toggle toggleSFX;
    [SerializeField] private GameObject optionPanel;
    private void Start()
    {
        buttonExit.onClick.AddListener(ExitGame);
        buttonStart.onClick.AddListener(StartGame);
        buttonRestart.onClick.AddListener(RestartGame);
        buttonOption.onClick.AddListener(OnOptionOpen);
        buttonOptionClose.onClick.AddListener(OnOptionClose);

        toggleMusic.onValueChanged.AddListener(OnToggleMusic);
        toggleSFX.onValueChanged.AddListener(OnToggleSFX);

        int isMusic = PlayerPrefs.GetInt("music", 0);
        toggleMusic.isOn = isMusic == 1;

        int isSfx = PlayerPrefs.GetInt("sfx",0);
        toggleSFX.isOn = isSfx == 1;
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
    public void OnToggleMusic(bool change)
    {
        PlayerPrefs.SetInt("music", toggleMusic.isOn ? 1: 0);
        
        SoundManager.Instance.ToggleMusic(change);
       
    }
    public void OnToggleSFX(bool change)
    {
        SoundManager.Instance.IsSfxOn = toggleSFX.isOn;
        PlayerPrefs.SetInt("sfx", toggleSFX.isOn ? 1 : 0);
    }
    public void EnableOptionPanel(bool isShow)
    {
        optionPanel.SetActive(isShow);
    }

    private void OnOptionOpen() { EnableOptionPanel(true); }
    private void OnOptionClose() { EnableOptionPanel(false); }
}
