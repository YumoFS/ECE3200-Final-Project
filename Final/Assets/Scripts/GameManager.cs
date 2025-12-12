// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("游戏状态")]
    public bool isGamePaused = false;
    public bool isGameOver = false;
    public bool isPlayerAlive = true;
    
    [Header("引用")]
    [SerializeField] private Player player;
    [SerializeField] private DataManager dataManager;
    // [SerializeField] private GameObject UI;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // DontDestroyOnLoad(UI);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // 确保DataManager存在
        if (DataManager.Instance == null)
        {
            Debug.LogWarning("DataManager未找到，创建一个");
            GameObject dmObj = new GameObject("DataManager");
            dmObj.AddComponent<DataManager>();
            DontDestroyOnLoad(dmObj);
        }
        
        // 查找玩家
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Escape))
    //     {
    //         UI.SetActive(!UI.activeSelf);
    //     }
    // }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"场景加载: {scene.name}");
        
        // 重新查找玩家
        player = FindObjectOfType<Player>();
        
        // 更新SceneSpawnManager
        if (SceneSpawnManager.Instance != null)
        {
            SceneSpawnManager.Instance.OnSceneChanged(scene.name);
        }
        
        // 确保玩家数据被加载
        if (player != null && DataManager.Instance != null)
        {
            // 给玩家一点时间初始化，然后加载数据
            Invoke(nameof(LoadPlayerData), 0.1f);
        }
    }
    
    private void LoadPlayerData()
    {
        if (player != null)
        {
            // 调用玩家的数据加载方法
            player.SendMessage("LoadPlayerData", SendMessageOptions.DontRequireReceiver);
        }
    }
    
    // 保存游戏
    public void SaveGame()
    {
        if (DataManager.Instance != null && player != null)
        {
            DataManager.Instance.SaveAllPlayerData(player);
            Debug.Log("游戏已保存");
        }
    }
    
    // 加载游戏
    public void LoadGame()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.LoadFromFile();
            
            PlayerData data = DataManager.Instance.LoadCheckpoint();
            if (!string.IsNullOrEmpty(data.checkpointSceneName))
            {
                SceneManager.LoadScene(data.checkpointSceneName);
            }
        }
    }
    
    // 开始新游戏
    public void StartNewGame()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.ResetToInitialState();
        }
        
        // 加载第一个场景
        SceneManager.LoadScene("CastleOutside"); // 修改为你的第一个场景名
    }
    
    // 暂停游戏
    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
    }
    
    // 继续游戏
    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
    }
    
    // 游戏结束
    public void GameOver(string reason = "Unknown")
    {
        isGameOver = true;
        isPlayerAlive = false;
        
        Debug.Log($"游戏结束 - 原因: {reason}");
        
        // 保存游戏状态
        SaveGame();
    }
    
    // 玩家死亡时调用
    public void OnPlayerDeath(string reason)
    {
        isPlayerAlive = false;
        
        // 更新死亡计数
        if (player != null)
        {
            player.deadCount++;
            
            // 保存数据
            if (DataManager.Instance != null)
            {
                DataManager.Instance.SaveDeathData(player.deadCount, reason);
                DataManager.Instance.SaveAllPlayerData(player);
            }
        }
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        // 确保游戏退出前保存数据
        if (DataManager.Instance != null && player != null)
        {
            SaveGame();
        }
    }
}