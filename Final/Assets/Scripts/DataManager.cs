// DataManager.cs - 玩家数据管理器
using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    
    [SerializeField] private PlayerData currentPlayerData;
    
    // 新增：名字池配置
    [Header("随机名字配置")]
    [SerializeField] private NamePool namePool;
    [SerializeField] private bool useTitles = true;
    [SerializeField] private int nameStyle = 0; // 0: 简单, 1: 完整, 2: 带称号

    private List<string> nameHistory = new List<string>();
    
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
            
            // 初始化名字池（如果为空）
            if (namePool == null)
            {
                namePool = new NamePool();
            }
            
            // 加载名字历史
            LoadNameHistory();
        }
        else
        {
            Destroy(gameObject);
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
        if (nameHistory.Contains(fullName) && nameHistory.Count < 50)
        {
            // 如果名字重复且历史记录不多，尝试重新生成
            return GenerateRandomName();
        }
        
        // 添加到历史记录
        nameHistory.Add(fullName);
        if (nameHistory.Count > 100) // 限制历史记录长度
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
        
        // 生成初始随机名字
        currentPlayerData.playerName = GenerateRandomName();
    }
    
    // 辅助方法：从数组中随机选择元素
    private string GetRandomElement(string[] array)
    {
        if (array == null || array.Length == 0)
            return "";
        
        return array[Random.Range(0, array.Length)];
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
}