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
        StartCoroutine(TransitionToScene(sceneName));
    }
    
    // 重新加载当前场景（用于死亡后复活）
    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        LoadSceneWithSave(currentScene);
    }
    
    private System.Collections.IEnumerator TransitionToScene(string sceneName)
    {
        // 播放转场动画
        if (transitionAnimator != null)
            transitionAnimator.SetTrigger("Start");
        
        // 等待转场动画
        yield return new WaitForSeconds(transitionTime);
        
        // 加载新场景
        SceneManager.LoadScene(sceneName);
        
        // 恢复转场动画
        if (transitionAnimator != null)
            transitionAnimator.SetTrigger("End");
    }
}