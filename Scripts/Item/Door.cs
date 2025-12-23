using Unity.VisualScripting;
using UnityEngine;
using System;

public class Door : MonoBehaviour, Interactable
{
    public bool isOpen = false;
    public string interactMessage;
    public string doorName;
    public string keyName;
    Animator doorAnimation;
    public bool isCheckVisualSight = false;
    public GameObject KeyPrefab;
    public Transform spawnPos;

    string Interactable.interactMessage { get { return interactMessage; } }
    public string objectName { get { return doorName; } }

    private void Start()
    {
        doorAnimation = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        isOpen = true;
        Debug.Log("Door is Opened");
        doorAnimation.SetBool("isOpen", true);
        if (doorName == "°¨¿Á¹®") SoundManager.instance.PlayAudioOnce("IronDoorOpen");
        else SoundManager.instance.PlayAudioOnce("DoorOpen");

        if (isCheckVisualSight)
        {
            Instantiate(KeyPrefab, spawnPos.position, transform.rotation, null);
        }
    }
}