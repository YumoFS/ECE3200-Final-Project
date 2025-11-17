using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [Header("UI组件")]
    public GameObject dialoguePanel;
    public TMPro.TextMeshProUGUI speakerNameText;
    public TMPro.TextMeshProUGUI dialogueText;
    public UnityEngine.UI.Image speakerPortrait;
    public UnityEngine.UI.Image playerPortrait;
    public UnityEngine.UI.Image bossPortrait;
    public GameObject continueIndicator;
    
    [Header("动画设置")]
    public float fadeDuration = 0.3f;
    public float portraitHighlightIntensity = 1.2f;
    
    [Header("音效")]
    public AudioClip typeSound;
    public AudioClip openSound;
    public AudioClip closeSound;
    
    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;
    
    void Start()
    {
        canvasGroup = dialoguePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = dialoguePanel.AddComponent<CanvasGroup>();
        }
        
        // 初始隐藏
        dialoguePanel.SetActive(false);
        canvasGroup.alpha = 0f;
        
        // 注册事件
        DialogueManager.OnDialogueStart += OnDialogueStart;
        DialogueManager.OnDialogueEnd += OnDialogueEnd;
        DialogueManager.OnLineStart += OnLineStart;
        DialogueManager.OnLineComplete += OnLineComplete;
    }
    
    void OnDialogueStart(Dialogue dialogue)
    {
        // 显示UI
        dialoguePanel.SetActive(true);
        
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeUI(0f, 1f, fadeDuration));
        
        // 播放打开音效
        if (openSound != null)
            AudioSource.PlayClipAtPoint(openSound, Camera.main.transform.position);
    }
    
    void OnDialogueEnd(Dialogue dialogue)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeUI(1f, 0f, fadeDuration, true));
        
        // 播放关闭音效
        if (closeSound != null)
            AudioSource.PlayClipAtPoint(closeSound, Camera.main.transform.position);
    }
    
    void OnLineStart(Dialogue.DialogueLine line)
    {
        // 更新说话者信息
        speakerNameText.text = line.speakerName;
        dialogueText.text = "";
        
        // 高亮当前说话者的肖像
        if (line.isPlayerSpeaking)
        {
            HighlightPortrait(playerPortrait, true);
            HighlightPortrait(bossPortrait, false);
        }
        else
        {
            HighlightPortrait(playerPortrait, false);
            HighlightPortrait(bossPortrait, true);
        }
        
        // 设置肖像
        if (line.speakerPortrait != null)
        {
            if (line.isPlayerSpeaking)
                playerPortrait.sprite = line.speakerPortrait;
            else
                bossPortrait.sprite = line.speakerPortrait;
        }
        
        // 隐藏继续指示器
        if (continueIndicator != null)
            continueIndicator.SetActive(false);
    }
    
    void OnLineComplete(Dialogue.DialogueLine line)
    {
        // 显示继续指示器（如果可以跳过）
        if (continueIndicator != null && DialogueManager.Instance.currentDialogue.canBeSkipped)
            continueIndicator.SetActive(true);
    }
    
    void HighlightPortrait(UnityEngine.UI.Image portrait, bool highlight)
    {
        if (portrait != null)
        {
            portrait.color = highlight ? Color.white * portraitHighlightIntensity : Color.white;
        }
    }
    
    IEnumerator FadeUI(float from, float to, float duration, bool disableAfter = false)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        
        canvasGroup.alpha = to;
        
        if (disableAfter)
        {
            dialoguePanel.SetActive(false);
        }
    }
    
    // 这个方法由DialogueManager调用，用于更新文本
    public void UpdateDialogueText(string text)
    {
        dialogueText.text = text;
    }
    
    // 播放打字音效
    public void PlayTypeSound()
    {
        if (typeSound != null)
        {
            // 使用2D音效避免位置影响
            AudioSource.PlayClipAtPoint(typeSound, Vector3.zero);
        }
    }
}