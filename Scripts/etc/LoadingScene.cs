using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    public static string nextScene;
    [SerializeField] Image fadeOutBackground;
    [SerializeField] Image loadingFill;
    [SerializeField] Image secondLoadingFill;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float minLoadingTime = 1f;
    [SerializeField] float secondDuration = 0.2f;
    void Start()
    {
        SoundManager.instance.AllAudioStop();
        loadingFill.fillAmount = 0f;
        secondLoadingFill.fillAmount = 0f;
        StartCoroutine(LoadSceneCoroutine());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("10.Loading");
    }

    IEnumerator LoadSceneCoroutine()
    {
        fadeOutBackground.color = Color.black;
        float fadeTimer = 0f;
        float fadeDuration = 1f;

        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);
            fadeOutBackground.color = new Color(0, 0, 0, alpha);
            yield return null;

        }
        fadeOutBackground.color = new Color(0, 0, 0, 0);

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;
        timer = 0f;
        while (timer < minLoadingTime)
        {
            yield return null;
            timer += Time.deltaTime;
            loadingFill.fillAmount = Mathf.Lerp(0f, 1f, timer / minLoadingTime);
        }
        loadingFill.fillAmount = 1f;

        timer = 0f;
        while (timer < secondDuration)
        {
            yield return null;
            timer += Time.deltaTime;
            secondLoadingFill.fillAmount = Mathf.Lerp(0f, 1f, timer / secondDuration);
        }
        secondLoadingFill.fillAmount = 1f;

        while (op.progress < 0.9f)
        {
            yield return null;
        }

        op.allowSceneActivation = true;
    }


}
