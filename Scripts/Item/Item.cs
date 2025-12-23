using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Item : MonoBehaviour, Interactable
{
    public string itemName;
    public string usage;
    public string interactMessage;
    public bool canUse = true;
    public bool useSuccessful = false;

    public string objectName { get { return itemName; } }

    string Interactable.interactMessage { get { return interactMessage; } }

    public bool Use()
    {
        if (canUse)
        {
            canUse = false;
            return UseItem();
        }
        return false;
    }

    protected virtual bool UseItem()
	{
        return false;
	}
}