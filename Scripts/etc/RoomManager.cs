using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
	public Vector2Int playerPos;
	public Vector2Int enemyPos;

	public bool CheckObjectNearPlayer()
	{
		if (Mathf.Abs(playerPos.x - enemyPos.x) <= 3 && Mathf.Abs(playerPos.y - enemyPos.y) <= 1)
		{
			return true;
		}
		return false;
	}

	public bool CheckDistance()
	{
		if(Mathf.Abs(playerPos.x - enemyPos.x) <= 3 && Mathf.Abs(playerPos.y - enemyPos.y) <= 0)
		{
			return true;
		}
		return false;
	}
}
