// DeathTransitionSceneController.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class DeathTransitionSceneController : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private CanvasGroup blackScreenCanvasGroup;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI deathReasonText;
    [SerializeField] private TextMeshProUGUI deathStatsText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI timeText;
    
    [Header("设置")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float countdownDuration = 3f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    
    private string checkpointSceneName;
    private string reason;
    private int deathCount;
    private int winCount;
    private string playerName;
    private int currentTime;
    
    void Start()
    {
        // 初始化UI
        if (blackScreenCanvasGroup != null)
        {
            blackScreenCanvasGroup.alpha = 0;
        }
        
        // 从PlayerPrefs或DataManager加载数据
        LoadDeathData();
        
        // 开始过渡流程
        StartCoroutine(TransitionProcess());
    }
    
    private void LoadDeathData()
    {
        // 从DataManager获取数据
        if (DataManager.Instance != null)
        {
            PlayerData data = DataManager.Instance.LoadCheckpoint();
            checkpointSceneName = data.checkpointSceneName;
            deathCount = data.deadCount;
            winCount = data.winCount;
            playerName = data.playerName;
            currentTime = data.currentTime;
        }
        
        // 获取死亡原因
        reason = PlayerPrefs.GetString("LastDeathReason", "Unknown");
        
        // 更新UI
        if (deathReasonText != null)
        {
            deathReasonText.text = $"Death Reason: {reason}";
        }
        
        if (deathStatsText != null)
        {
            deathStatsText.text = $"Deaths: {deathCount} | Wins: {winCount}";
        }

        if (playerNameText != null)
        {
            playerNameText.text = $"Player Name: {playerName}";
        }

        if (timeText != null)
        {
            timeText.text = $"Time: {currentTime} AD";
        }
    }
    
    private IEnumerator TransitionProcess()
    {
        // 步骤1: 淡入黑屏
        yield return StartCoroutine(FadeCanvasGroup(blackScreenCanvasGroup, 0, 1, fadeInDuration));
        
        // 步骤2: 显示倒计时
        float timer = countdownDuration;
        
        while (timer > 0)
        {
            if (countdownText != null)
            {
                int seconds = Mathf.CeilToInt(timer);
                countdownText.text = $"Respawn in {seconds}...";
            }
            
            timer -= Time.deltaTime;
            yield return null;
        }
        
        // 步骤3: 淡出黑屏
        yield return StartCoroutine(FadeCanvasGroup(blackScreenCanvasGroup, 1, 0, fadeOutDuration));
        
        // 步骤4: 加载存档点场景
        LoadCheckpointScene();
    }
    
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        if (canvasGroup == null) yield break;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        canvasGroup.alpha = endAlpha;
    }
    
    private void LoadCheckpointScene()
    {
        if (string.IsNullOrEmpty(checkpointSceneName))
        {
            Debug.LogError("No checkpoint scene found!");
            
            // 如果没有存档点，返回主菜单
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.LoadSceneWithSave("MainMenu");
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
            return;
        }
        
        // 使用SceneTransitionManager加载场景
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadSceneWithSave(checkpointSceneName);
        }
        else
        {
            // 直接加载场景
            SceneManager.LoadScene(checkpointSceneName);
        }
    }
    
    // 供其他脚本调用的静态方法
    public static void LoadDeathTransitionScene(string deathReason)
    {
        // 保存死亡原因到PlayerPrefs
        PlayerPrefs.SetString("LastDeathReason", deathReason);
        PlayerPrefs.Save();
        
        // 加载死亡过渡场景
        SceneManager.LoadScene("DeathTransitionScene");
    }
}