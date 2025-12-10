using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGameButton : MonoBehaviour
{
    // 目标场景（在Inspector中分配）
    public string targetScene;
    
    // 按钮组件引用
    private Button button;
    
    void Start()
    {
        // 获取按钮组件
        button = GetComponent<Button>();
        
        // 添加点击事件监听
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogError("ButtonPressed脚本需要附加到Button对象上");
        }
    }
    
    // 按钮点击事件处理
    public void OnButtonClicked()
    {
        // 检查是否分配了目标场景
        if (targetScene == null)
        {
            Debug.LogError("未分配目标场景！请在Inspector中分配目标场景");
            return;
        }
        
        // 加载场景
        LoadTargetScene(targetScene);
    }
    
    // 加载目标场景
    public void LoadTargetScene(string sceneName)
    {
        // 检查场景是否存在
        if (IsSceneInBuildSettings(sceneName))
        {
            // 加载场景
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"场景 '{sceneName}' 不存在于Build Settings中");
        }
    }
    
    // 检查场景是否在Build Settings中
    private bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string scene = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (scene == sceneName)
                return true;
        }
        return false;
    }
    /*
    // 可选：在编辑器中直接跳转场景的方法
    public void LoadTargetSceneDirectly()
    {
        if (targetScene != null)
        {
            string sceneName = targetScene.name;
            LoadTargetScene(sceneName);
        }
    }
    */
}