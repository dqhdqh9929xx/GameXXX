using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }
    [SerializeField] Image fadeImage;
    [SerializeField] float fadeDuration = 1f;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        fadeImage.gameObject.SetActive(true);
        // Start Fading
        StartCoroutine(FadeOut());
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    IEnumerator FadeAndLoad(string sceneToLoad)
    {
        yield return StartCoroutine(FadeIn());
        SceneManager.LoadScene(sceneToLoad);
    }
    
    IEnumerator FadeOut()
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0;
            
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                color.a = Mathf.Lerp(1, 0, t / fadeDuration);
                fadeImage.color = color;
                yield return null;
            }
            color.a = 0;
            fadeImage.color = color;
        }
    }

    IEnumerator FadeIn()
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0;

            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                color.a = Mathf.Lerp(0, 1, t / fadeDuration);
                fadeImage.color = color;
                yield return null;
            }
            color.a = 0;
            fadeImage.color = color;
        }
    }
}
