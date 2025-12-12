// SceneTransitionManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }
    
    [Header("场景过渡")]
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float transitionTime = 1f;
    
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
    
    // 加载场景并保存当前状态
    public void LoadSceneWithSave(string sceneName)
    {
        SavePlayerDataBeforeTransition();
        
        // 重置任何死亡状态
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.isAlive = true;
            player.deadReason = "";
        }
        
        StartCoroutine(TransitionToScene(sceneName));
    }

    private void SavePlayerDataBeforeTransition()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null && DataManager.Instance != null)
        {
            player.SavePlayerDataToDataManager();
            Debug.Log("场景切换前已保存玩家数据");
        }
    }
    
    // 重新加载当前场景（用于死亡后复活）
    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SavePlayerDataBeforeTransition();
        LoadSceneWithSave(currentScene);
    }
    
    private System.Collections.IEnumerator TransitionToScene(string sceneName)
    {
        // 播放转场动画
        if (transitionAnimator != null)
            transitionAnimator.SetTrigger("Start");
        
        // 等待转场动画
        yield return new WaitForSeconds(transitionTime);
        SavePlayerDataBeforeTransition();
        
        // 加载新场景
        SceneManager.LoadScene(sceneName);
        
        // 恢复转场动画
        if (transitionAnimator != null)
            transitionAnimator.SetTrigger("End");
    }
}