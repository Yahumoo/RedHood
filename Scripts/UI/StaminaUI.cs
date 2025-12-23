using System;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] private Image staminaFill;
    [SerializeField] private Image lighterFill;

    private PlayerController player;
    private Lighter lighter;
    
    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        lighter = player.GetComponent<Lighter>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        staminaFill.fillAmount = player.currentRunStamina / player.maxRunStamina;
        lighterFill.fillAmount = lighter.currentGas / lighter.maxGas;
    }
}
