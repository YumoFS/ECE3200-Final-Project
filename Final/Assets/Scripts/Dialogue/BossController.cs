using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("对话设置")]
    public DialogueTrigger encounterDialogue;
    public DialogueTrigger defeatDialogue;
    public DialogueTrigger phaseChangeDialogue;
    
    [Header("战斗设置")]
    public int maxHealth = 1000;
    public int currentHealth;
    
    // 修复：确保事件定义正确
    public static System.Action<BossController> OnBossEncounter;
    public static System.Action<BossController> OnBossDefeated;
    
    private bool hasEncountered = false;
    private bool isDefeated = false;
    
    void Start()
    {
        currentHealth = maxHealth;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasEncountered)
        {
            EncounterBoss();
        }
    }
    
    public void EncounterBoss()
    {
        hasEncountered = true;
        
        // 触发遭遇事件（传入this参数）
        OnBossEncounter?.Invoke(this);
        
        // 触发遭遇对话
        if (encounterDialogue != null)
        {
            encounterDialogue.TriggerDialogue();
        }
        else
        {
            StartBossFight();
        }
    }
    
    public void StartBossFight()
    {
        // 开始Boss战斗逻辑
        Debug.Log("Boss战斗开始！");
    }
    
    public void TakeDamage(int damage)
    {
        if (isDefeated) return;
        
        currentHealth -= damage;
        
        // 检查阶段转换
        CheckPhaseChange();
        
        // 检查是否被击败
        if (currentHealth <= 0)
        {
            DefeatBoss();
        }
    }
    
    void CheckPhaseChange()
    {
        // 根据血量触发阶段转换对话
        float healthPercent = (float)currentHealth / maxHealth;
        
        if (healthPercent <= 0.5f && phaseChangeDialogue != null)
        {
            phaseChangeDialogue.TriggerDialogue();
        }
    }
    
    void DefeatBoss()
    {
        isDefeated = true;
        
        // 触发击败事件（传入this参数）
        OnBossDefeated?.Invoke(this);
        
        // 触发击败对话
        if (defeatDialogue != null)
        {
            defeatDialogue.TriggerDialogue();
        }
        else
        {
            OnBossDefeatedComplete();
        }
    }
    
    void OnBossDefeatedComplete()
    {
        // Boss被击败后的逻辑
        Debug.Log("Boss被击败！");
    }
}