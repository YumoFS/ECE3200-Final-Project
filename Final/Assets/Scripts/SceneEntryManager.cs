// SceneEntryManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEntryManager : MonoBehaviour
{
    public static SceneEntryManager Instance { get; private set; }
    
    [Header("场景入口设置")]
    [SerializeField] private string entrySceneName = "MainMenu";
    [SerializeField] private bool loadSavedGameOnStart = true;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // 如果是首次启动，加载入口场景
        if (SceneManager.GetActiveScene().name != entrySceneName)
        {
            LoadEntryScene();
        }
        else
        {
            // 已经在入口场景，初始化新游戏或继续游戏
            InitializeGame();
        }
    }
    
    private void LoadEntryScene()
    {
        SceneManager.LoadScene(entrySceneName);
    }
    
    private void InitializeGame()
    {
        if (loadSavedGameOnStart && DataManager.Instance != null)
        {
            // 尝试加载保存的游戏
            PlayerData savedData = DataManager.Instance.LoadCheckpoint();
            
            if (!string.IsNullOrEmpty(savedData.checkpointSceneName))
            {
                Debug.Log($"继续游戏 - 场景: {savedData.checkpointSceneName}");
                
                // 加载保存的场景
                if (SceneTransitionManager.Instance != null)
                {
                    SceneTransitionManager.Instance.LoadSceneWithSave(savedData.checkpointSceneName);
                }
                else
                {
                    SceneManager.LoadScene(savedData.checkpointSceneName);
                }
            }
            else
            {
                Debug.Log("开始新游戏");
                StartNewGame();
            }
        }
        else
        {
            StartNewGame();
        }
    }
    
    public void StartNewGame()
    {
        // 重置DataManager中的数据
        if (DataManager.Instance != null)
        {
            DataManager.Instance.ResetToInitialState();
        }
        
        // 加载第一个游戏场景
        string firstScene = "Level1"; // 根据你的场景名修改
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadSceneWithSave(firstScene);
        }
        else
        {
            SceneManager.LoadScene(firstScene);
        }
    }
    
    public void ContinueGame()
    {
        if (DataManager.Instance != null)
        {
            PlayerData savedData = DataManager.Instance.LoadCheckpoint();
            
            if (!string.IsNullOrEmpty(savedData.checkpointSceneName))
            {
                if (SceneTransitionManager.Instance != null)
                {
                    SceneTransitionManager.Instance.LoadSceneWithSave(savedData.checkpointSceneName);
                }
                else
                {
                    SceneManager.LoadScene(savedData.checkpointSceneName);
                }
            }
            else
            {
                Debug.LogWarning("没有找到保存的游戏");
                StartNewGame();
            }
        }
        else
        {
            Debug.LogWarning("DataManager未找到");
            StartNewGame();
        }
    }
}