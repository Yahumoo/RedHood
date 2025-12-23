using UnityEngine;

public class Lighter : MonoBehaviour
{
    [Header("Lighter")]
    public float maxGas = 100f;
    public float currentGas;
    public float gasCost;
    public float gasRegen;

    [Header("Light Setting")]
    public Light lighterLight;
    public GameObject flameObj;
    public float maxLightIntensity;
    public float lightingSpeed;
    private float lightIntensity;
    public Color defaultColor;
    public Color enemyDetectColor;
    public float colorChangingSpeed;
    private Color nowColor;

    public bool canUse;
    bool lighterOn;
    bool detectObject;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentGas = maxGas;
    }

    // Update is called once per frame
    void Update()
    {
        detectObject = Managers.instance.roomManager.CheckObjectNearPlayer();

        if (canUse)
        {
            if (Input.GetMouseButtonDown(1))
            {
                SoundManager.instance.PlayAudioForce("LighterOn");
                lighterOn = true;
            }

            if (Input.GetMouseButtonUp(1) || currentGas <= 0)
            {
                lighterOn = false;
            }
        }
        else lighterOn = false;
        flameObj.SetActive(lighterOn);

        if (lighterOn)
        {
            lightIntensity = maxLightIntensity;
            currentGas -= Time.deltaTime * gasCost;
        }
        else
        {
            lightIntensity = 0f;
            currentGas += Time.deltaTime * gasRegen;
        }

        nowColor = detectObject ? enemyDetectColor : defaultColor;
        
        currentGas = Mathf.Clamp(currentGas, 0, maxGas);
        lighterLight.intensity = Mathf.Lerp(lighterLight.intensity, lightIntensity, Time.deltaTime * lightingSpeed);
        lighterLight.color = Color.Lerp(lighterLight.color, nowColor, Time.deltaTime * colorChangingSpeed);
        if(lighterLight.intensity < 0.01f) lighterLight.intensity = 0f;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Room>() != null)
        {
            Managers.instance.roomManager.playerPos = other.GetComponent<Room>().pos;
        }
    }
}
