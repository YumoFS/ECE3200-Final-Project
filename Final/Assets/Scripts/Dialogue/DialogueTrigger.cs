using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("对话数据")]
    public Dialogue dialogue;
    
    [Header("触发条件")]
    public TriggerType triggerType = TriggerType.Manual;
    public float triggerDelay = 0f;
    
    [Header("一次性触发")]
    public bool triggerOnce = true;
    private bool hasTriggered = false;
    
    [Header("区域触发设置")]
    public Collider2D triggerArea;
    
    [Header("战斗相关")]
    public bool startBossFightAfterDialogue = false;
    public BossController bossController;
    
    public enum TriggerType
    {
        Manual,             // 手动触发
        OnTriggerEnter,     // 进入区域触发
        OnInteract,         // 交互触发
        OnBossEncounter,    // 遇到Boss时触发
        OnBossDefeated,     // 击败Boss后触发
        OnEvent             // 事件触发
    }
    
    void Start()
    {
        // 根据触发类型设置监听
        switch (triggerType)
        {
            case TriggerType.OnBossEncounter:
                BossController.OnBossEncounter += TriggerDialogueWithBoss;
                break;
            case TriggerType.OnBossDefeated:
                BossController.OnBossDefeated += TriggerDialogueWithBoss;
                break;
        }
        
        // 注册对话结束事件
        DialogueManager.OnDialogueEnd += OnDialogueCompleted;
    }
    
    void OnDestroy()
    {
        // 取消注册事件
        BossController.OnBossEncounter -= TriggerDialogueWithBoss;
        BossController.OnBossDefeated -= TriggerDialogueWithBoss;
        DialogueManager.OnDialogueEnd -= OnDialogueCompleted;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerType == TriggerType.OnTriggerEnter && 
            other.CompareTag("Player") && 
            (!triggerOnce || !hasTriggered))
        {
            TriggerDialogue();
        }
    }
    
    // 新的方法：接受BossController参数的触发方法
    private void TriggerDialogueWithBoss(BossController boss)
    {
        // 可选：检查是否是指定的Boss（如果指定了特定bossController）
        if (bossController != null && boss != bossController)
            return;
            
        TriggerDialogue();
    }
    
    // 原有的无参数触发方法
    public void TriggerDialogue()
    {
        if (hasTriggered && triggerOnce) return;
        
        StartCoroutine(TriggerWithDelay());
    }
    
    IEnumerator TriggerWithDelay()
    {
        yield return new WaitForSeconds(triggerDelay);
        
        if (DialogueManager.Instance != null && dialogue != null)
        {
            DialogueManager.Instance.StartDialogue(dialogue);
            hasTriggered = true;
        }
    }
    
    void OnDialogueCompleted(Dialogue completedDialogue)
    {
        if (completedDialogue == dialogue && startBossFightAfterDialogue)
        {
            StartBossFight();
        }
    }
    
    void StartBossFight()
    {
        if (bossController != null)
        {
            bossController.StartBossFight();
        }
    }
    
    // 手动调用此方法来触发对话
    [ContextMenu("触发对话")]
    public void ManualTrigger()
    {
        if (triggerType == TriggerType.Manual)
        {
            TriggerDialogue();
        }
    }
    
    // 交互方法，可由PlayerController调用
    public void OnInteract()
    {
        if (triggerType == TriggerType.OnInteract && 
            (!triggerOnce || !hasTriggered))
        {
            TriggerDialogue();
        }
    }
}