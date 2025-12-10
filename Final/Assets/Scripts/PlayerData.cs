using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //角色名字
    public string playerName = "Name";
    // 基础属性
    public int playerHitPoint = 1;
    public int playerHitPointMax = 1;
    public bool hasTorch = false; 
    public int deadCount = 0;
    public int winCount = 0;
    public int playerAttackPower = 1;
    
    // 存档点信息
    public Vector3 checkpointPosition;
    public string checkpointSceneName;
    
    // 其他可能需要保存的属性
    public List<string> collectedItems;
    public List<string> completedQuests;
    
    public PlayerData()
    {
        collectedItems = new List<string>();
        completedQuests = new List<string>();
    }
}