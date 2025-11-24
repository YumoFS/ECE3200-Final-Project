using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchBlock : Interactable
{
    [SerializeField] private float floatHeight = 0.5f; // 文字框浮起高度
    [SerializeField] private float floatSpeed = 2f; // 浮起速度
    [SerializeField] private Player player;
    
    private Vector3 originalPosition;
    private bool isFloating = false;
    
    private void Start()
    {
        if (interactionPrompt != null)
        {
            originalPosition = interactionPrompt.transform.position;
            interactionPrompt.SetActive(false);
        }
    }
    
    private void Update()
    {
        // 控制提示框的浮动动画
        if (isFloating && interactionPrompt != null)
        {
            float newY = originalPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            interactionPrompt.transform.position = new Vector3(
                interactionPrompt.transform.position.x,
                newY,
                interactionPrompt.transform.position.z
            );
        }
    }
    
    public override void Interact()
    {
        player.hasTorch = true;
        Destroy(gameObject);
        Debug.Log("与方块交互！");
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerEnter();
            isFloating = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerExit();
            isFloating = false;
            
            // 重置位置
            if (interactionPrompt != null)
            {
                interactionPrompt.transform.position = originalPosition;
            }
        }
    }
}
