using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SceneTransition : MonoBehaviour 
{
    [SerializeField] private Material screenTransitionMaterial;
    [SerializeField] private float TransitionTime =1f;
    [SerializeField] private string propertyName = "_Progress";
    public UnityEvent OnTransitionDone;

    private void Start()
    {
        StartCoroutine(TransitionCoroutine());
    }
    private IEnumerator TransitionCoroutine()
    {
        float currentTime = 0;
        while (currentTime < TransitionTime)
        { 
            currentTime += Time.deltaTime;
            screenTransitionMaterial.SetFloat(propertyName, Mathf.Clamp01(currentTime/TransitionTime));
            yield return null;
        }
        OnTransitionDone?.Invoke();
    }
}
