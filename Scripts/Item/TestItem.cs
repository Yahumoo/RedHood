using UnityEngine;

public class TestItem : Item
{
	protected override bool UseItem()
	{
		Debug.Log("Used " + itemName);
		return false;
	}
}