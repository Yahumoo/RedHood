using UnityEngine;

public class ZoomInTest : MonoBehaviour
{
    public bool isReset;
    public float lerpSpeed;
    public Transform startPos;
    public Transform endPos;

    private void Start()
    {
        transform.position = startPos.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isReset)
        {
            transform.position = Vector3.Lerp(transform.position, endPos.position, Time.deltaTime * lerpSpeed);
        }
        else transform.position = startPos.position;
    }
}
