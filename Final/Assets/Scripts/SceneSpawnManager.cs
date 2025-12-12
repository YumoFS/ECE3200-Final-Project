// SceneSpawnManager.cs - 管理场景中的出生点
using UnityEngine;

public class SceneSpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class SceneSpawnInfo
    {
        public string sceneName;
        public Transform defaultSpawnPoint;
        public Transform[] spawnPoints; // 可以有多个出生点
    }
    
    public static SceneSpawnManager Instance { get; private set; }
    
    [Header("场景出生点配置")]
    [SerializeField] private SceneSpawnInfo[] sceneSpawnInfos;
    
    [Header("当前场景出生点")]
    [SerializeField] private Transform currentSpawnPoint;
    [SerializeField] private string currentSceneName;
    
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
        
        currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        InitializeSpawnPoint();
    }
    
    // 初始化出生点
    private void InitializeSpawnPoint()
    {
        // 查找当前场景的配置
        SceneSpawnInfo currentSceneInfo = null;
        foreach (var info in sceneSpawnInfos)
        {
            if (info.sceneName == currentSceneName)
            {
                currentSceneInfo = info;
                break;
            }
        }
        
        // 设置当前出生点
        if (currentSceneInfo != null)
        {
            if (currentSceneInfo.defaultSpawnPoint != null)
            {
                currentSpawnPoint = currentSceneInfo.defaultSpawnPoint;
            }
            else if (currentSceneInfo.spawnPoints.Length > 0)
            {
                currentSpawnPoint = currentSceneInfo.spawnPoints[0];
            }
        }
        
        // 如果配置中没有，尝试查找场景中的SpawnPoint对象
        if (currentSpawnPoint == null)
        {
            GameObject spawnObj = GameObject.Find("SpawnPoint");
            if (spawnObj == null)
                spawnObj = GameObject.FindWithTag("SpawnPoint");
            
            if (spawnObj != null)
                currentSpawnPoint = spawnObj.transform;
        }
        
        Debug.Log($"当前场景出生点: {currentSpawnPoint?.name ?? "未找到"}");
    }
    
    // 获取当前出生点位置
    public Vector3 GetSpawnPosition()
    {
        if (currentSpawnPoint != null)
            return currentSpawnPoint.position;
        
        return Vector3.zero;
    }
    
    // 获取当前出生点Transform
    public Transform GetSpawnTransform()
    {
        return currentSpawnPoint;
    }
    
    // 设置新的出生点
    public void SetSpawnPoint(Transform newSpawnPoint)
    {
        currentSpawnPoint = newSpawnPoint;
        Debug.Log($"出生点已更新为: {currentSpawnPoint.name}");
    }
    
    // 场景切换时调用
    public void OnSceneChanged(string sceneName)
    {
        currentSceneName = sceneName;
        InitializeSpawnPoint();
    }

    public Vector3 GetSpawnPositionForScene(string sceneName)
    {
        foreach (var info in sceneSpawnInfos)
        {
            if (info.sceneName == sceneName)
            {
                if (info.defaultSpawnPoint != null)
                    return info.defaultSpawnPoint.position;
                else if (info.spawnPoints.Length > 0)
                    return info.spawnPoints[0].position;
                else
                    return Vector3.zero;
            }
        }
        
        // 如果没有找到配置，尝试在场景中查找
        Debug.LogWarning($"未找到场景 {sceneName} 的出生点配置");
        return Vector3.zero;
    }
}