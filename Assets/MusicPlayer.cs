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
        // Singleton check
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Get or create an AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = true;

        // First scene's music
        AudioClip initialClip = GetMusicBySceneIndex(SceneManager.GetActiveScene().buildIndex);
        audioSource.clip = initialClip;
        audioSource.Play();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Determine which clip should play
        AudioClip newClip = GetMusicBySceneIndex(scene.buildIndex);

        // Only switch tracks if it’s different
        if (audioSource.clip != newClip)
        {
            audioSource.clip = newClip;
            audioSource.Play(); // restart from the beginning
        }
        // If it's the same clip, do nothing; let it continue playing
    }

    /// <summary>
    /// Logic for mapping scene indices to the correct AudioClip.
    /// </summary>
    private AudioClip GetMusicBySceneIndex(int sceneIndex)
    {
        if (sceneIndex == 9 || sceneIndex == 0)
        {
            return clipB;
        }
        else
        {
            return clipA;
        }
    }
}
