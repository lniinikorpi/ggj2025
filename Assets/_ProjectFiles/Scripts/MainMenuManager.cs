using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button continueButton;
    public Button startButton;
    public Button exitButton;

    private int furthestLevelt = -1;

    private void Awake()
    {
        furthestLevelt = PlayerPrefs.GetInt("Level", -1);

        if(furthestLevelt == -1)
        {
            continueButton.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        continueButton.onClick.AddListener(Continue);
        startButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(Exit);
    }

    void Continue() { 
        SceneManager.LoadScene(furthestLevelt);
    }

    void StartGame() { 
        SceneManager.LoadScene(1);
    }

    void Exit()
    {
        Application.Quit();
    }   
}
