using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    
    [Header("UI引用")]
    public GameObject dialoguePanel;
    public TMPro.TextMeshProUGUI speakerNameText;
    public TMPro.TextMeshProUGUI dialogueText;
    public UnityEngine.UI.Image speakerPortrait;
    public UnityEngine.UI.Button continueButton;
    
    [Header("设置")]
    public float autoAdvanceDelay = 2f;      // 自动前进延迟
    
    private Queue<Dialogue.DialogueLine> dialogueQueue;
    public Dialogue currentDialogue;
    private Dialogue.DialogueLine currentLine;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    
    // 事件
    public static System.Action<Dialogue> OnDialogueStart;
    public static System.Action<Dialogue> OnDialogueEnd;
    public static System.Action<Dialogue.DialogueLine> OnLineStart;
    public static System.Action<Dialogue.DialogueLine> OnLineComplete;
    
    void Awake()
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
        
        dialogueQueue = new Queue<Dialogue.DialogueLine>();
        
        // 绑定继续按钮
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(AdvanceDialogue);
        }
    }
    
    void Update()
    {
        // 键盘控制
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            AdvanceDialogue();
        }
        
        // 跳过对话（长按）
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Space))
        {
            SkipDialogue();
        }
    }
    
    public void StartDialogue(Dialogue dialogue)
    {
        if (dialoguePanel == null)
        {
            Debug.LogError("DialoguePanel未分配！");
            return;
        }
        
        currentDialogue = dialogue;
        dialogueQueue.Clear();
        
        // 将对话行加入队列
        foreach (var line in dialogue.dialogueLines)
        {
            dialogueQueue.Enqueue(line);
        }
        
        // 显示UI
        dialoguePanel.SetActive(true);
        
        // 触发事件
        OnDialogueStart?.Invoke(dialogue);
        dialogue.onDialogueStart?.Invoke();
        
        // 开始第一行对话
        AdvanceDialogue();
    }
    
    public void AdvanceDialogue()
    {
        if (isTyping)
        {
            // 如果正在打字，立即完成
            CompleteLine();
            return;
        }
        
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }
        
        currentLine = dialogueQueue.Dequeue();
        StartCoroutine(DisplayLine(currentLine));
    }
    
    IEnumerator DisplayLine(Dialogue.DialogueLine line)
    {
        isTyping = true;
        
        // 更新UI
        speakerNameText.text = line.speakerName;
        speakerPortrait.sprite = line.speakerPortrait;
        dialogueText.text = "";
        
        // 触发行开始事件
        OnLineStart?.Invoke(line);
        line.onLineStart?.Invoke();
        
        // 播放语音
        if (line.voiceClip != null)
        {
            AudioSource.PlayClipAtPoint(line.voiceClip, Camera.main.transform.position);
        }
        
        // 逐字显示
        foreach (char letter in line.dialogueText.ToCharArray())
        {
            dialogueText.text += letter;
            
            // 可以在这里添加打字音效
            // PlayTypeSound();
            
            yield return new WaitForSeconds(line.textSpeed);
        }
        
        CompleteLine();
        
        // 如果是自动前进，等待后自动下一句
        if (currentDialogue.canBeSkipped == false)
        {
            yield return new WaitForSeconds(autoAdvanceDelay);
            AdvanceDialogue();
        }
    }
    
    void CompleteLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        if (currentLine != null)
        {
            dialogueText.text = currentLine.dialogueText;
            
            // 触发行完成事件
            OnLineComplete?.Invoke(currentLine);
            currentLine.onLineComplete?.Invoke();
        }
        
        isTyping = false;
    }
    
    void SkipDialogue()
    {
        if (currentDialogue == null || !currentDialogue.canBeSkipped) return;
        
        EndDialogue();
    }
    
    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        
        // 触发事件
        OnDialogueEnd?.Invoke(currentDialogue);
        currentDialogue.onDialogueEnd?.Invoke();
        
        currentDialogue = null;
        isTyping = false;
    }
    
    public bool IsDialogueActive()
    {
        return dialoguePanel != null && dialoguePanel.activeInHierarchy;
    }
}