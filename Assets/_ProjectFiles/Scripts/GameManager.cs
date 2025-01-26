using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private SceneTransition sceneTransition;

    public float EatToWinPercentage = 90;
    private float m_eatenPercentage = 0;
    private int m_eatablesCount = 0;
    bool wonthelevel = false;

    private List<Eatable> m_eatables = new List<Eatable>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        m_eatables.AddRange(FindObjectsByType<Eatable>(FindObjectsSortMode.None));
        m_eatablesCount = m_eatables.Count;
        PlayerPrefs.SetInt("Level", SceneManager.GetActiveScene().buildIndex);
    }

    public void RemoveEatable(Eatable eatable)
    {
        m_eatables.Remove(eatable);
        m_eatenPercentage = (float)(m_eatablesCount - m_eatables.Count) / (float)m_eatablesCount * 100;
        if (m_eatenPercentage >= EatToWinPercentage && wonthelevel == false)
        {
            wonthelevel = true;
            CompleteLevel();
        }
    }

    public void CompleteLevel()
    {
        Debug.Log("You win!");
        sceneTransition.StartTransitionOut();
    }

}
