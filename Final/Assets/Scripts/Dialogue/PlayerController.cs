using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("对话交互")]
    public float interactionRange = 2f;
    public LayerMask interactableLayer;
    
    private DialogueTrigger currentDialogueTrigger;
    
    void Update()
    {
        CheckForInteractables();
        
        // 交互输入
        if (Input.GetKeyDown(KeyCode.E) && currentDialogueTrigger != null)
        {
            currentDialogueTrigger.OnInteract();
        }
    }
    
    void CheckForInteractables()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position, 
            transform.right * Mathf.Sign(transform.localScale.x), 
            interactionRange, 
            interactableLayer
        );
        
        if (hit.collider != null)
        {
            DialogueTrigger trigger = hit.collider.GetComponent<DialogueTrigger>();
            if (trigger != null && trigger.triggerType == DialogueTrigger.TriggerType.OnInteract)
            {
                currentDialogueTrigger = trigger;
                // 显示交互提示UI
                ShowInteractionPrompt(true);
            }
            else
            {
                ClearInteractable();
            }
        }
        else
        {
            ClearInteractable();
        }
    }
    
    void ClearInteractable()
    {
        if (currentDialogueTrigger != null)
        {
            ShowInteractionPrompt(false);
            currentDialogueTrigger = null;
        }
    }
    
    void ShowInteractionPrompt(bool show)
    {
        // 在这里显示/隐藏交互提示UI
        // 例如：interactionPrompt.SetActive(show);
    }
}