using System;
using UnityEngine;

public class RoomDetectableObject : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Room>() != null)
        {
            Managers.instance.roomManager.enemyPos = other.GetComponent<Room>().pos;
        }
    }
}