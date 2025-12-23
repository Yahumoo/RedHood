using System;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public EnemyManager enemyManager;
    public RoomManager roomManager;
    
    public static Managers instance;

    private void Awake()
    {
        instance = this;
    }
}
