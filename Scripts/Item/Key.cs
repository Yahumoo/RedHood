using UnityEngine;

public class Key : Item
{
    protected override bool UseItem()
    {
        if(usage == itemName)
        {
            Debug.Log("열쇠 사용");
            canUse = false;
            useSuccessful = true;
            Destroy(this.gameObject, 0.5f);
            return true;
        }
        else
        {
            Debug.Log("열쇠가 맞지 않습니다.");
            canUse = true;
        }
        return false;
    }
}
