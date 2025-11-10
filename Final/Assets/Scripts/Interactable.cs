using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected string interactionText = "Press 'F' to Interact";
    [SerializeField] protected GameObject interactionPrompt; // UI提示框
    
    protected bool isPlayerNear = false;
    
    public abstract void Interact();
    public virtual string GetInteractionText() => interactionText;
    
    // 显示/隐藏提示的方法
    public virtual void ShowPrompt(bool show)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(show);
        }
    }
    
    public virtual bool IsPlayerNear() => isPlayerNear;
    
    // 当玩家进入触发区域时调用
    public virtual void OnPlayerEnter()
    {
        isPlayerNear = true;
        Debug.Log("Is Player Near");
        ShowPrompt(true);
    }
    
    // 当玩家离开触发区域时调用
    public virtual void OnPlayerExit()
    {
        isPlayerNear = false;
        ShowPrompt(false);
    }
}