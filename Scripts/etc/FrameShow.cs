using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class FrameShow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI frameTxt;
    float dt;
    float ms;
    float fps;
    float timer;

    // Update is called once per frame
    void Update()
    {
        dt += (Time.unscaledDeltaTime - dt) * 0.1f;

        timer += Time.unscaledDeltaTime;
        if(timer > 0.2f)
        {
            ms = dt * 1000.0f;
            fps = 1.0f / dt;

            frameTxt.text = string.Format("{0:0.0} ms ({1:0.} FPS)", ms, fps);

            timer = 0;
        }
    }
}
