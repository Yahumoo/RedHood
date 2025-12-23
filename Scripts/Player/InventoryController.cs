using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static event Action<Item> OnItemPickedUp;
	public static event Action<Item> OnUsedItem;
    public static event Action<Transform> OnFindItem;
    public static event Action<bool, Item> OnUsedSucces;

    public Transform itemHolder;
	public Item currentItem;
    public LayerMask interactLayer;

    Transform nearestObject;
	Item nearestItem;
    Door nearestDoor;
	PlayerController player;

	Collider itemCollider;

    void Update()
	{
        CheckItems();
        if (Input.GetMouseButtonDown(0))
		{
			UseCurrentItem();
		}
		
		if (Input.GetKeyDown(KeyCode.F))
		{
            ItemPickup();
		}
	}
	
	void UseCurrentItem()
	{
        bool isSuccess = false;
		if (currentItem != null)
		{
            if (nearestDoor != null)
                currentItem.usage = nearestDoor.keyName;

            if (currentItem.Use())
			{
                nearestDoor.OpenDoor();
                Debug.Log("You used " + currentItem.itemName);
				OnUsedSucces?.Invoke(true, currentItem);
                OnUsedItem?.Invoke(currentItem);
                isSuccess = true;
            }
			else
			{
				Debug.Log("You can't use that.");
                OnUsedSucces?.Invoke(currentItem.useSuccessful, currentItem);
                if (!currentItem.useSuccessful)
                {
                    OnUsedItem?.Invoke(currentItem);
                }
                isSuccess = false;
            }
		}

        if (nearestDoor != null)
        {
            if (!isSuccess)
                SoundManager.instance.PlayAudioOnce(nearestDoor.doorName == "°¨¿Á¹®" ? "IronDoorLocked" : "DoorLocked");
        }
	}

	void DropCurrentItem()
	{
		if (currentItem != null)
		{
            Transform itemTransform = currentItem.transform;
			Rigidbody itemRigid = currentItem.GetComponent<Rigidbody>();

            Debug.Log("You dropped " + currentItem.itemName);

            if (currentItem.name == "Chair")
            {
                currentItem.GetComponent<Chair>().DeselectedChair();
            }
            itemCollider.isTrigger = false;
            itemTransform.SetParent(null);
            itemTransform.position = itemHolder.position;
            itemTransform.rotation = Quaternion.Euler(-90, 0, 0);
            itemRigid.isKinematic = false;
            itemRigid.isKinematic = false;
            itemRigid.useGravity = true;

            currentItem = null;
        }
	}

	void CheckItems()
	{
        int hitLength = Physics.OverlapSphere(transform.position, 3f, interactLayer).Length;

        if ((currentItem == null && hitLength > 0) || (currentItem != null && hitLength > 1))
        {
            foreach (Collider obj in Physics.OverlapSphere(transform.position, 3f, interactLayer))
            {
                Interactable curInteractable = obj.GetComponent<Interactable>();

                if (currentItem != null)
                {
                    if (currentItem.itemName == curInteractable.objectName) continue;
                    else
                    {
                        if (nearestObject == null) nearestObject = obj.transform;
                        else
                        {
                            if (Vector3.Distance(transform.position, obj.transform.position) <
                            Vector3.Distance(transform.position, nearestObject.transform.position))
                            {
                                nearestObject = obj.transform;
                            }
                        }
                    }
                }
                else
                {
                    if (nearestObject == null) nearestObject = obj.transform;
                    else
                    {
                        if (Vector3.Distance(transform.position, obj.transform.position) <
                        Vector3.Distance(transform.position, nearestObject.transform.position))
                        {
                            nearestObject = obj.transform;
                        }
                    }
                }
            }
        }
        else
        {
            nearestObject = null;
            nearestItem = null;
            nearestDoor = null;
        }

        if(nearestObject != null)
        {
            nearestItem = nearestObject.GetComponent<Item>();
            nearestDoor = nearestObject.GetComponent<Door>();
        }

        OnFindItem?.Invoke(nearestObject);
    }
	
	void ItemPickup()
	{
        DropCurrentItem();

        if (nearestItem == null) return;
        Item item = nearestItem;

        if(currentItem != null)
            if (currentItem.itemName == item.itemName) return;

        Debug.Log("Picked up " + item.itemName);

        currentItem = item;
        itemCollider = item.GetComponent<Collider>();
        itemCollider.isTrigger = true;

        item.transform.SetParent(itemHolder);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        item.GetComponent<Rigidbody>().isKinematic = true;
        item.GetComponent<Rigidbody>().useGravity = false;

        OnItemPickedUp?.Invoke(item);
        if (currentItem.name == "Chair")
        {
            currentItem.GetComponent<Chair>().SellectedChair();
        }
        
        if(currentItem.GetComponent<Key>() != null)
        {
            SoundManager.instance.PlayAudioOnce("KeyPickup");
        }

        nearestObject = null;
    }
}