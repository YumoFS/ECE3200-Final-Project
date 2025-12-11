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
    public int currentTime = Random.Range(1000, 1200);

    //结局对话相关
    public bool hasArrivedEmptyThrone = false;
    public bool hasDeadByTraps = false;
    public bool hasDeadbyIronVirgin = false;
    public bool hasInteractedWithTorch = false;
    public bool hasKilledBoss = false;
    public bool hasFoundTheCandleHole = false;
    public bool hasPassedCodeSpace = false;
    public bool hasKilledBossByTorch = false;
    public bool hasPassedHeaven = false;
    
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