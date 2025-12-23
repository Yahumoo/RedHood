using UnityEngine;
using TMPro;
using System.Collections;

public interface Interactable
{
    public string objectName { get; }
    public string interactMessage { get; }
}

public class InteractUI : MonoBehaviour
{
    public float fadeTime;
    public Vector2 posOffset;
    public Transform followTarget;
    public TextMeshProUGUI messageTxt;
    public CanvasGroup canvasGroup;

    RectTransform parentRect;
    Vector3 targetPos;
    Vector3 sizeDelta;
    RectTransform rect;
    Camera mainCam;

    IEnumerator fadeInCoroutine;
    IEnumerator fadeOutCoroutine;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        mainCam = Camera.main;
        parentRect = transform.parent.GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        InventoryController.OnFindItem -= SetUI;
    }

    void Update()
    {
        if (followTarget == null)
        {
            if (canvasGroup.alpha > 0)
            {
                if (fadeInCoroutine != null)
                {
                    StopCoroutine(fadeInCoroutine);
                    fadeInCoroutine = null;
                }

                if(fadeOutCoroutine == null)
                {
                    fadeInCoroutine = null;
                    fadeOutCoroutine = FadeOutCoroutine();
                    StartCoroutine(fadeOutCoroutine);
                }
            }
        }
        Vector3 _offset = new Vector3(posOffset.x, posOffset.y, 0);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(mainCam, targetPos + _offset);

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, null, out localPos);

        rect.anchoredPosition = localPos;
    }

    public void SetUI(Transform _target)
    {
        if(_target == null)
        {
            followTarget = null;
            return;
        }
        else
        {
            Door door = _target.GetComponent<Door>();
            if(door != null)
            {
                if (door.isOpen)
                {
                    followTarget = null;
                    return;
                }
            }

            messageTxt.text = _target.GetComponent<Interactable>().interactMessage;
            followTarget = _target;
            targetPos = _target.position;

            Vector3 _offset = new Vector3(posOffset.x, posOffset.y, 0);
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(mainCam, targetPos + _offset);

            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, null, out localPos);

            rect.anchoredPosition = localPos;

            if (fadeOutCoroutine != null) 
            { 
                StopCoroutine(fadeOutCoroutine);
                fadeOutCoroutine = null;
            }

            if (fadeInCoroutine == null)
            {
                fadeOutCoroutine = null;
                fadeInCoroutine = FadeInCoroutine();
                StartCoroutine(fadeInCoroutine);
            }
        }
    }

    IEnumerator FadeInCoroutine()
    {
        float t = 0;
        float alpha = canvasGroup.alpha;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            if(alpha > 0) canvasGroup.alpha = (t / fadeTime) / alpha;
            else canvasGroup.alpha = (t / fadeTime);
            yield return null;
        }
    }

    IEnumerator FadeOutCoroutine()
    {
        float t = fadeTime;
        float alpha = canvasGroup.alpha;

        while (t > 0)
        {
            t -= Time.deltaTime;
            if (alpha > 0) canvasGroup.alpha = (t / fadeTime) / alpha;
            else canvasGroup.alpha = (t / fadeTime);
            yield return null;
        }
    }
}
