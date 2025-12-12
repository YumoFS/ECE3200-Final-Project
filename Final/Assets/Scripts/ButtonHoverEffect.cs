using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("悬停效果")]
    public float hoverScale = 1.1f;
    public Color hoverColor = new Color(1f, 1f, 1f, 0.8f);
    public float transitionTime = 0.2f;
    
    [Header("引用")]
    public Image targetImage;
    public TextMeshPro targetText;
    
    private Vector3 originalScale;
    private Color originalColor;
    private Color originalTextColor;
    
    void Start()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
            
        if (targetText == null)
            targetText = GetComponentInChildren<TextMeshPro>();
            
        originalScale = transform.localScale;
        
        if (targetImage != null)
            originalColor = targetImage.color;
            
        if (targetText != null)
            originalTextColor = targetText.color;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 缩放效果
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, originalScale * hoverScale, transitionTime)
            .setEase(LeanTweenType.easeOutQuad);
            
        // 颜色效果
        if (targetImage != null)
        {
            LeanTween.color(targetImage.rectTransform, hoverColor, transitionTime);
        }
        
        if (targetText != null)
        {
            LeanTween.value(gameObject, UpdateTextColor, originalTextColor, 
                new Color(hoverColor.r, hoverColor.g, hoverColor.b, originalTextColor.a), 
                transitionTime);
        }
        
        // 播放声音（可选）
        // AudioManager.Instance.PlaySound("ButtonHover");
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        // 恢复缩放
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, originalScale, transitionTime)
            .setEase(LeanTweenType.easeOutQuad);
            
        // 恢复颜色
        if (targetImage != null)
        {
            LeanTween.color(targetImage.rectTransform, originalColor, transitionTime);
        }
        
        if (targetText != null)
        {
            LeanTween.value(gameObject, UpdateTextColor, targetText.color, originalTextColor, transitionTime);
        }
    }
    
    private void UpdateTextColor(Color color)
    {
        if (targetText != null)
            targetText.color = color;
    }
}