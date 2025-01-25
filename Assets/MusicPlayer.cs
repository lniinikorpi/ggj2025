using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance;

    [Header("Music Clips")]
    [SerializeField] private AudioClip clipA;
    [SerializeField] private AudioClip clipB;

    private AudioSource audioSource;

    private void Awake()
    {
        // Basic singleton check
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Get or add an AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = true;

        // Start playing *something* when first scene loads
        SetMusicBySceneIndex(SceneManager.GetActiveScene().buildIndex);
        audioSource.Play();
    }

    private void OnEnable()
    {
        // Subscribe to sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks / errors
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Called by SceneManager when a new scene is loaded.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Choose and set the new music clip based on the newly loaded scene
        SetMusicBySceneIndex(scene.buildIndex);
        audioSource.Play();
    }

    /// <summary>
    /// Picks which clip to play based on a scene index.
    /// </summary>
    /// <param name="sceneIndex">Scene's build index</param>
    private void SetMusicBySceneIndex(int sceneIndex)
    {
        // Replace with your own logic for which scenes get clip A or B
        if (sceneIndex == 0|| sceneIndex == 2)
        {
            audioSource.clip = clipB;
        }
        else
        {
            audioSource.clip = clipA;
        }
    }
}
