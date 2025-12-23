using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] GameObject mainUI;
    [SerializeField] GameObject settingUI;
    [SerializeField] Image fadeBackground;

    [Header("Main")]
    [SerializeField] Button startBtn;
    [SerializeField] Button settingBtn;
    [SerializeField] Button quitBtn;

    [Header("Setting")]
    [SerializeField] Button settingQuitBtn;
    [SerializeField] Button settingCloseBtn;
    bool settingAcitve = false;

    IEnumerator startGameCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        settingAcitve = true;
        SettingUI();

        startBtn.onClick.AddListener(() => StartGame());
        settingBtn.onClick.AddListener(() => SettingUI());
        settingCloseBtn.onClick.AddListener(() => SettingUI());
        quitBtn.onClick.AddListener(() => Application.Quit());

        settingQuitBtn.onClick.AddListener(() => SettingUI());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SettingUI();
        }
    }
    void StartGame()
    {
        if (startGameCoroutine == null)
        { 
            startGameCoroutine = StartGameCoroutine();
            StartCoroutine(startGameCoroutine);
        }
    }

    void SettingUI() 
    {
        settingAcitve = !settingAcitve;
        settingUI.SetActive(settingAcitve);
        mainUI.SetActive(!settingAcitve);
    }

    IEnumerator StartGameCoroutine()
    {
        fadeBackground.color = new Color(0, 0, 0, 0);
        yield return null;
        float timer = 0f;
        while (fadeBackground.color.a < 0.99f)
        {
            timer += Time.deltaTime;
            fadeBackground.color = Color.Lerp(fadeBackground.color, new Color(0, 0, 0, 1), timer);
            if (fadeBackground.color.a >= 0.99f)
            {
                fadeBackground.color = new Color(0, 0, 0, 1);
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        LoadingScene.LoadScene("NewLevel");
    }
}
