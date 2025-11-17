using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Dialogue
{
    [System.Serializable]
    public class DialogueLine
    {
        [Header("说话者")]
        public string speakerName;           // 说话者名字
        public Sprite speakerPortrait;       // 说话者头像
        public bool isPlayerSpeaking;        // 是否是玩家在说话
        
        [Header("对话内容")]
        [TextArea(3, 5)]
        public string dialogueText;          // 对话文本
        
        [Header("音频")]
        public AudioClip voiceClip;          // 语音片段
        public float textSpeed = 0.05f;      // 文字显示速度
        
        [Header("事件")]
        public UnityEvent onLineStart;       // 当该行开始时触发的事件
        public UnityEvent onLineComplete;    // 当该行完成时触发的事件
    }
    
    public string dialogueId;                // 对话ID，用于识别
    public DialogueLine[] dialogueLines;     // 对话行数组
    public bool canBeSkipped = true;         // 是否可以跳过
    public UnityEvent onDialogueStart;       // 对话开始事件
    public UnityEvent onDialogueEnd;         // 对话结束事件
}