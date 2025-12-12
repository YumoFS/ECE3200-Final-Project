// DataManager.cs - 玩家数据管理器
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    
    public PlayerData currentPlayerData;
    
    // 新增：名字池配置
    [Header("随机名字配置")]
    [SerializeField] private bool useTitles = false;
    [SerializeField] private int nameStyle = 0; // 0: 简单, 1: 完整, 2: 带称号
    private NamePool namePool = new NamePool();
 
    [Header("自动保存设置")]
    [SerializeField] private float autoSaveInterval = 60f; // 自动保存间隔（秒）
    private float autoSaveTimer = 0f;

    private List<string> nameHistory = new List<string>();
    
    private void Awake()
    {
        // 单例模式，确保只有一个DataManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 从文件加载数据
            LoadFromFile();
            
            // 加载名字历史
            LoadNameHistory();
            
            Debug.Log("DataManager初始化完成");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 自动保存计时
        autoSaveTimer += Time.deltaTime;
        if (autoSaveTimer >= autoSaveInterval)
        {
            AutoSave();
            autoSaveTimer = 0f;
        }
    }

    // 自动保存
    private void AutoSave()
    {
        if (currentPlayerData != null)
        {
            SaveToFile();
            Debug.Log($"自动保存完成 - {DateTime.Now:HH:mm:ss}");
        }
    }
    
    // 新增：生成随机名字
    public string GenerateRandomName()
    {
        if (namePool == null)
        {
            namePool = new NamePool();
        }
        
        string firstName = GetRandomElement(namePool.firstNamePool);
        string lastName = GetRandomElement(namePool.lastNamePool);
        string title = useTitles ? GetRandomElement(namePool.titlePool) : "";
        
        string fullName = "";
        
        switch (nameStyle)
        {
            case 0: // 简单：First Last
                fullName = $"{firstName} {lastName}";
                break;
                
            case 1: // 完整：First of the Last
                string middle = GetRandomElement(namePool.middleNamePool);
                fullName = $"{firstName} {middle} {lastName}";
                break;
                
            case 2: // 带称号
                if (!string.IsNullOrEmpty(title))
                    fullName = $"{firstName} {lastName} {title}";
                else
                    fullName = $"{firstName} {lastName}";
                break;
                
            default:
                fullName = $"{firstName} {lastName}";
                break;
        }
        
        // 避免重复名字（可选）
        if (nameHistory.Contains(fullName) && nameHistory.Count < 30)
        {
            // 如果名字重复且历史记录不多，尝试重新生成
            return GenerateRandomName();
        }
        
        // 添加到历史记录
        nameHistory.Add(fullName);
        if (nameHistory.Count > 50) // 限制历史记录长度
        {
            nameHistory.RemoveAt(0);
        }
        
        SaveNameHistory();
        
        return fullName;
    }
    
    // 新增：设置玩家名字
    public void SetPlayerName(string newName)
    {
        if (currentPlayerData != null)
        {
            currentPlayerData.playerName = newName;
            SaveToFile();
        }
    }
    
    // 新增：获取玩家名字
    public string GetPlayerName()
    {
        return currentPlayerData?.playerName ?? "Unknown";
    }
    
    // 保存玩家数据到存档点
    public void SaveCheckpoint(Vector3 position, string sceneName, Player player)
    {
        if (player == null) return;
        
        currentPlayerData.checkpointPosition = position;
        currentPlayerData.checkpointSceneName = sceneName;
        
        // 保存玩家属性
        currentPlayerData.playerName = player.playerName; // 新增
        currentPlayerData.playerHitPoint = player.playerHitPoint;
        currentPlayerData.playerHitPointMax = player.playerHitPointMax;
        currentPlayerData.hasTorch = player.hasTorch;
        currentPlayerData.deadCount = player.deadCount;
        currentPlayerData.winCount = player.winCount;
        currentPlayerData.playerAttackPower = player.playerAttackPower;
        currentPlayerData.currentTime = player.currentTime;
        
        // 可以添加保存到文件的功能
        SaveToFile();
    
        Debug.Log($"存档点已保存 - 场景: {sceneName}, 位置: {position}, 生命值: {player.playerHitPoint}");
    }
    
    public void SaveAllPlayerData(Player player)
    {
        if (player == null || currentPlayerData == null) return;
        
        // 保存基础属性
        currentPlayerData.playerName = player.playerName;
        currentPlayerData.playerHitPoint = player.playerHitPoint;
        currentPlayerData.playerHitPointMax = player.playerHitPointMax;
        currentPlayerData.hasTorch = player.hasTorch;
        currentPlayerData.deadCount = player.deadCount;
        currentPlayerData.winCount = player.winCount;
        currentPlayerData.playerAttackPower = player.playerAttackPower;
        currentPlayerData.currentTime = player.currentTime;
        
        // 保存到文件
        SaveToFile();
        
        Debug.Log($"玩家所有数据已保存: {player.playerName}");
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
        currentPlayerData.currentTime = UnityEngine.Random.Range(1000, 1200);
        
        // 生成初始随机名字
        currentPlayerData.playerName = GenerateRandomName();
    }
    
    // 辅助方法：从数组中随机选择元素
    private string GetRandomElement(string[] array)
    {
        if (array == null || array.Length == 0)
            return "";
        
        return array[UnityEngine.Random.Range(0, array.Length)];
    }
    
    // 新增：保存名字历史到PlayerPrefs
    private void SaveNameHistory()
    {
        string historyJson = JsonUtility.ToJson(new StringListWrapper { list = nameHistory });
        PlayerPrefs.SetString("NameHistory", historyJson);
        PlayerPrefs.Save();
    }
    
    // 新增：从PlayerPrefs加载名字历史
    private void LoadNameHistory()
    {
        if (PlayerPrefs.HasKey("NameHistory"))
        {
            string historyJson = PlayerPrefs.GetString("NameHistory");
            StringListWrapper wrapper = JsonUtility.FromJson<StringListWrapper>(historyJson);
            if (wrapper != null)
            {
                nameHistory = wrapper.list;
            }
        }
    }
    
    // 新增：包装类用于序列化List<string>
    [System.Serializable]
    private class StringListWrapper
    {
        public List<string> list;
    }
    
    // 保存到文件（如果需要持久化存档）
    private void SaveToFile()
    {
        try
        {
            string json = JsonUtility.ToJson(currentPlayerData, true);
            PlayerPrefs.SetString("PlayerSaveData", json);
            PlayerPrefs.Save();
            
            // 同时保存一份到本地文件（可选）
            string filePath = Path.Combine(Application.persistentDataPath, "player_save.json");
            File.WriteAllText(filePath, json);
            
            Debug.Log($"数据已保存到: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存数据时出错: {e.Message}");
        }
    }
    
    // 从文件加载（如果需要）
    public void LoadFromFile()
    {
        try
        {
            // 先尝试从PlayerPrefs加载
            if (PlayerPrefs.HasKey("PlayerSaveData"))
            {
                string json = PlayerPrefs.GetString("PlayerSaveData");
                currentPlayerData = JsonUtility.FromJson<PlayerData>(json);
                Debug.Log("从PlayerPrefs加载玩家数据");
            }
            else
            {
                // 尝试从本地文件加载
                string filePath = Path.Combine(Application.persistentDataPath, "player_save.json");
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    currentPlayerData = JsonUtility.FromJson<PlayerData>(json);
                    Debug.Log($"从文件加载玩家数据: {filePath}");
                }
                else
                {
                    // 没有保存文件，创建新数据
                    currentPlayerData = new PlayerData();
                    ResetToInitialState();
                    Debug.Log("创建新的玩家数据");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载数据时出错: {e.Message}");
            currentPlayerData = new PlayerData();
            ResetToInitialState();
        }
    }

    public void SaveDeathData(int newDeathCount, string deathReason)
    {
        if (currentPlayerData == null) return;
        
        currentPlayerData.deadCount = newDeathCount;
        
        // 可以保存更多的死亡相关信息
        PlayerPrefs.SetString("LastDeathReason", deathReason);
        PlayerPrefs.SetInt("TotalDeaths", newDeathCount);
        PlayerPrefs.Save();
        
        SaveToFile();
    }

    public void SetEndingFlag(string flagName, bool value)
    {
        if (currentPlayerData == null) return;
        
        switch (flagName)
        {
            case "hasArrivedEmptyThrone":
                currentPlayerData.hasArrivedEmptyThrone = value;
                break;
            case "hasDeadByTraps":
                currentPlayerData.hasDeadByTraps = value;
                break;
            case "hasDeadbyIronVirgin":
                currentPlayerData.hasDeadbyIronVirgin = value;
                break;
            case "hasInteractedWithTorch":
                currentPlayerData.hasInteractedWithTorch = value;
                break;
            case "hasKilledBoss":
                currentPlayerData.hasKilledBoss = value;
                break;
            case "hasFoundTheCandleHole":
                currentPlayerData.hasFoundTheCandleHole = value;
                break;
            case "hasPassedCodeSpace":
                currentPlayerData.hasPassedCodeSpace = value;
                break;
            case "hasKilledBossByTorch":
                currentPlayerData.hasKilledBossByTorch = value;
                break;
            case "hasPassedHeaven":
                currentPlayerData.hasPassedHeaven = value;
                break;
            default:
                Debug.LogWarning($"未知的结局标志: {flagName}");
                return;
        }
        
        SaveToFile();
        Debug.Log($"结局标志已更新: {flagName} = {value}");
    }

    // 获取结局相关属性
    public bool GetEndingFlag(string flagName)
    {
        if (currentPlayerData == null) return false;
        
        switch (flagName)
        {
            case "hasArrivedEmptyThrone":
                return currentPlayerData.hasArrivedEmptyThrone;
            case "hasDeadByTraps":
                return currentPlayerData.hasDeadByTraps;
            case "hasDeadbyIronVirgin":
                return currentPlayerData.hasDeadbyIronVirgin;
            case "hasInteractedWithTorch":
                return currentPlayerData.hasInteractedWithTorch;
            case "hasKilledBoss":
                return currentPlayerData.hasKilledBoss;
            case "hasFoundTheCandleHole":
                return currentPlayerData.hasFoundTheCandleHole;
            case "hasPassedCodeSpace":
                return currentPlayerData.hasPassedCodeSpace;
            case "hasKilledBossByTorch":
                return currentPlayerData.hasKilledBossByTorch;
            case "hasPassedHeaven":
                return currentPlayerData.hasPassedHeaven;
            default:
                Debug.LogWarning($"未知的结局标志: {flagName}");
                return false;
        }
    }

    // 重置所有结局标志（开始新游戏时使用）
    public void ResetEndingFlags()
    {
        if (currentPlayerData == null) return;
        
        currentPlayerData.hasArrivedEmptyThrone = false;
        currentPlayerData.hasDeadByTraps = false;
        currentPlayerData.hasDeadbyIronVirgin = false;
        currentPlayerData.hasInteractedWithTorch = false;
        currentPlayerData.hasKilledBoss = false;
        currentPlayerData.hasFoundTheCandleHole = false;
        currentPlayerData.hasPassedCodeSpace = false;
        currentPlayerData.hasKilledBossByTorch = false;
        currentPlayerData.hasPassedHeaven = false;
        
        SaveToFile();
        Debug.Log("所有结局标志已重置");
    }
}