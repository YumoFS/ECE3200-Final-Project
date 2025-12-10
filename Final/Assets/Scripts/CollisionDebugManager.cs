// CollisionDebugManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CollisionDebugManager : MonoBehaviour
{
    public static CollisionDebugManager Instance { get; private set; }
    
    [Header("调试设置")]
    [SerializeField] private GameObject collisionPointPrefab; // 碰撞点标记预制体
    [SerializeField] private GameObject colliderBoundsPrefab; // 碰撞箱边框预制体
    [SerializeField] private Color playerColliderColor = Color.green;
    [SerializeField] private Color obstacleColliderColor = Color.red;
    [SerializeField] private float pauseDuration = 3f;
    [SerializeField] private float boundsDisplayDuration = 5f;
    [SerializeField] private bool enableDebug = true;
    
    [Header("UI引用")]
    [SerializeField] private UnityEngine.UI.Text debugInfoText;
    
    private bool isPaused = false;
    private List<GameObject> debugObjects = new List<GameObject>();
    private Vector3 lastCollisionPoint;
    private string lastCollisionInfo;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // 记录并显示碰撞信息
    public void LogCollision(Collision2D collision, string callerName = "")
    {
        if (!enableDebug) return;
        
        lastCollisionInfo = $"碰撞发生在: {callerName}\n" +
                           $"对象A: {collision.gameObject.name}\n" +
                           $"对象B: {collision.collider.gameObject.name}\n" +
                           $"接触点数量: {collision.contactCount}\n" +
                           $"相对速度: {collision.relativeVelocity.magnitude:F2}";
        
        if (debugInfoText != null)
        {
            debugInfoText.text = lastCollisionInfo;
            StartCoroutine(ClearTextAfter(5f));
        }
        
        Debug.Log(lastCollisionInfo);
        
        // 获取第一个接触点
        if (collision.contactCount > 0)
        {
            lastCollisionPoint = collision.contacts[0].point;
            ShowCollisionDebugInfo(collision);
        }
    }
    
    public void LogTrigger(Collider2D other, string callerName = "")
    {
        if (!enableDebug) return;
        
        lastCollisionInfo = $"触发器: {callerName}\n" +
                           $"触发对象: {other.gameObject.name}\n" +
                           $"位置: {other.bounds.center}";
        
        if (debugInfoText != null)
        {
            debugInfoText.text = lastCollisionInfo;
            StartCoroutine(ClearTextAfter(5f));
        }
        
        Debug.Log(lastCollisionInfo);
        
        // 显示触发器调试信息
        ShowTriggerDebugInfo(other);
    }
    
    // 显示碰撞调试信息
    private void ShowCollisionDebugInfo(Collision2D collision)
    {
        // 暂停游戏
        PauseGame();
        
        // 清理之前的调试对象
        ClearDebugObjects();
        
        // 标记所有接触点
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint2D contact = collision.contacts[i];
            MarkCollisionPoint(contact.point, $"Contact_{i}");
        }
        
        // 显示碰撞体的边界框
        ShowColliderBounds(collision.collider, obstacleColliderColor, "Obstacle");
        ShowColliderBounds(collision.otherCollider, playerColliderColor, "Player");
        
        // 自动恢复游戏
        StartCoroutine(ResumeAfterDelay(pauseDuration));
        
        // 自动清理调试物体
        StartCoroutine(ClearDebugAfter(boundsDisplayDuration));
    }
    
    // 显示触发器调试信息
    private void ShowTriggerDebugInfo(Collider2D other)
    {
        // 暂停游戏
        PauseGame();
        
        // 清理之前的调试对象
        ClearDebugObjects();
        
        // 标记触发器中心点
        MarkCollisionPoint(other.bounds.center, "Trigger_Center");
        
        // 显示碰撞体的边界框
        ShowColliderBounds(other, obstacleColliderColor, "Trigger");
        
        // 尝试获取并显示玩家的碰撞体
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                ShowColliderBounds(playerCollider, playerColliderColor, "Player");
            }
        }
        
        // 自动恢复游戏
        StartCoroutine(ResumeAfterDelay(pauseDuration));
        
        // 自动清理调试物体
        StartCoroutine(ClearDebugAfter(boundsDisplayDuration));
    }
    
    // 标记碰撞点
    private void MarkCollisionPoint(Vector2 point, string name = "CollisionPoint")
    {
        if (collisionPointPrefab != null)
        {
            GameObject marker = Instantiate(collisionPointPrefab, point, Quaternion.identity);
            marker.name = name;
            debugObjects.Add(marker);
        }
        else
        {
            // 如果没有预制体，创建简单的球体
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = point;
            sphere.transform.localScale = Vector3.one * 0.2f;
            sphere.name = name;
            
            // 移除碰撞体避免影响游戏
            Destroy(sphere.GetComponent<Collider>());
            
            // 设置颜色
            Renderer renderer = sphere.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.yellow;
            }
            
            debugObjects.Add(sphere);
        }
    }
    
    // 显示碰撞体边界框
    private void ShowColliderBounds(Collider2D collider, Color color, string label = "")
    {
        if (collider == null) return;
        
        Bounds bounds = collider.bounds;
        
        if (colliderBoundsPrefab != null)
        {
            GameObject boundsObj = Instantiate(
                colliderBoundsPrefab, 
                bounds.center, 
                Quaternion.identity
            );
            
            // 调整大小匹配碰撞体
            Vector3 scale = bounds.size;
            boundsObj.transform.localScale = scale;
            
            // 设置颜色
            Renderer renderer = boundsObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(color.r, color.g, color.b, 0.3f);
            }
            
            boundsObj.name = $"Bounds_{label}_{collider.gameObject.name}";
            debugObjects.Add(boundsObj);
        }
        else
        {
            // 如果没有预制体，创建线框
            GameObject lineObj = new GameObject($"Bounds_{label}_{collider.gameObject.name}");
            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
            
            // 配置LineRenderer
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.loop = true;
            
            // 创建矩形边界点
            Vector3[] corners = new Vector3[5];
            corners[0] = new Vector3(bounds.min.x, bounds.min.y, 0);
            corners[1] = new Vector3(bounds.max.x, bounds.min.y, 0);
            corners[2] = new Vector3(bounds.max.x, bounds.max.y, 0);
            corners[3] = new Vector3(bounds.min.x, bounds.max.y, 0);
            corners[4] = corners[0]; // 闭合
            
            lineRenderer.positionCount = 5;
            lineRenderer.SetPositions(corners);
            
            debugObjects.Add(lineObj);
        }
        
        // 在中心点添加标签
        GameObject labelObj = new GameObject($"Label_{label}");
        labelObj.transform.position = bounds.center + Vector3.up * 0.5f;
        
        #if UNITY_EDITOR
        // 在编辑器中显示文本标签
        UnityEditor.Handles.Label(labelObj.transform.position, label);
        #endif
        
        debugObjects.Add(labelObj);
    }
    
    // 暂停游戏
    private void PauseGame()
    {
        if (!isPaused)
        {
            Time.timeScale = 0f;
            isPaused = true;
            Debug.Log("游戏已暂停 - 碰撞调试模式");
        }
    }
    
    // 恢复游戏
    private void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("游戏已恢复");
    }
    
    // 清理调试物体
    public void ClearDebugObjects()
    {
        foreach (GameObject obj in debugObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        debugObjects.Clear();
    }
    
    // 协程：延迟后恢复游戏
    private IEnumerator ResumeAfterDelay(float delay)
    {
        float pauseEndTime = Time.realtimeSinceStartup + delay;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return null;
        }
        
        ResumeGame();
    }
    
    // 协程：延迟后清理调试物体
    private IEnumerator ClearDebugAfter(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        ClearDebugObjects();
    }
    
    // 协程：延迟后清除文本
    private IEnumerator ClearTextAfter(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (debugInfoText != null)
            debugInfoText.text = "";
    }
    
    // 手动恢复游戏（供UI按钮使用）
    public void ManualResume()
    {
        ResumeGame();
        ClearDebugObjects();
    }
    
    // 手动触发调试显示
    public void ManualDebugCollision(Collision2D collision)
    {
        ShowCollisionDebugInfo(collision);
    }
    
    public void ManualDebugTrigger(Collider2D other)
    {
        ShowTriggerDebugInfo(other);
    }
    
    void OnDestroy()
    {
        // 确保游戏状态被恢复
        if (isPaused)
        {
            Time.timeScale = 1f;
        }
    }
}