// DataManager.cs - 玩家数据管理器
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
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

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    
    [SerializeField] private PlayerData currentPlayerData;
    
    private void Awake()
    {
        // 单例模式，确保只有一个DataManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 如果还没有数据，初始化新数据
            if (currentPlayerData == null)
            {
                currentPlayerData = new PlayerData();
                ResetToInitialState();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // 保存玩家数据到存档点
    public void SaveCheckpoint(Vector3 position, string sceneName, Player player)
    {
        if (player == null) return;
        
        currentPlayerData.checkpointPosition = position;
        currentPlayerData.checkpointSceneName = sceneName;
        
        // 保存玩家属性
        currentPlayerData.playerHitPoint = player.playerHitPoint;
        currentPlayerData.playerHitPointMax = 1; // 根据你的代码，这是静态变量
        currentPlayerData.hasTorch = player.hasTorch;
        currentPlayerData.deadCount = player.deadCount;
        currentPlayerData.winCount = player.winCount;
        currentPlayerData.playerAttackPower = player.playerAttackPower;
        
        // 可以添加保存到文件的功能
        SaveToFile();
        
        Debug.Log($"存档点已保存 - 场景: {sceneName}, 位置: {position}");
    }
    
    // 加载存档点数据
    public PlayerData LoadCheckpoint()
    {
        // 这里可以添加从文件加载的功能
        return currentPlayerData;
    }
    
    // 重置为初始状态
    public void ResetToInitialState()
    {
        currentPlayerData = new PlayerData();
        currentPlayerData.playerHitPoint = 1;
        currentPlayerData.playerHitPointMax = 1;
        currentPlayerData.hasTorch = false;
        currentPlayerData.deadCount = 0;
        currentPlayerData.winCount = 0;
        currentPlayerData.playerAttackPower = 1;
    }
    
    // 保存到文件（如果需要持久化存档）
    private void SaveToFile()
    {
        // 可以使用PlayerPrefs或JSON文件
        string json = JsonUtility.ToJson(currentPlayerData);
        PlayerPrefs.SetString("PlayerSaveData", json);
        PlayerPrefs.Save();
    }
    
    // 从文件加载（如果需要）
    public void LoadFromFile()
    {
        if (PlayerPrefs.HasKey("PlayerSaveData"))
        {
            string json = PlayerPrefs.GetString("PlayerSaveData");
            currentPlayerData = JsonUtility.FromJson<PlayerData>(json);
        }
    }
}