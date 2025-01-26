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
        SceneTransition sceneTransition = FindFirstObjectByType<SceneTransition>();
        sceneTransition.OnTransitionDone.AddListener(() =>
        {
            SceneManager.LoadScene(furthestLevelt);
        });
        sceneTransition.StartTransitionOut();
    }

    void StartGame()
    {
        SceneTransition sceneTransition = FindFirstObjectByType<SceneTransition>();
        sceneTransition.OnTransitionDone.AddListener(() =>
        {
            SceneManager.LoadScene(1);
        });
        sceneTransition.StartTransitionOut();
    }

    void Exit()
    {
        Application.Quit();
    }   
}
