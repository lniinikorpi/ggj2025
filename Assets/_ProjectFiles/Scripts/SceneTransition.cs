using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Material screenTransitionMaterial;
    [SerializeField] private float TransitionTime = 1f;
    [SerializeField] private string propertyName = "_Progress";
    public UnityEvent OnTransitionDone;

    private void Start()
    {
        StartCoroutine(TransitionCoroutine());
    }
    public void StartTransitionOut()
    {
        StartCoroutine(TransitionOutCoroutine());
    }
    private IEnumerator TransitionCoroutine()
    {
        float currentTime = 0;
        while (currentTime < TransitionTime)
        {
            currentTime += Time.deltaTime;
            screenTransitionMaterial.SetFloat(propertyName, Mathf.Clamp01(currentTime / TransitionTime));
            yield return null;
        }
        OnTransitionDone?.Invoke();
    }
    public IEnumerator TransitionOutCoroutine()
    {
        float currentTime = 0f;
        while (currentTime < TransitionTime)
        {
            currentTime += Time.deltaTime;
            // Compute progress going from 1 to 0
            float progress = 1f - Mathf.Clamp01(currentTime / TransitionTime);
            screenTransitionMaterial.SetFloat(propertyName, progress);
            yield return null;
        }

        // Determine the next scene index
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        // Make sure it's still within our scene list in Build Settings
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
        }

        // Fire your event if appropriate
        StartCoroutine(TransitionCoroutine());
        OnTransitionDone?.Invoke();
    }
}