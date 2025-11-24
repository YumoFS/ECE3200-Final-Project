using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

public class InteractiveText : MonoBehaviour
{
    /********************** Variables **********************/
    private const float WIDTH_PADDING = 5f;
    private const float HEIGHT_PADDING = 3f;
    [SerializeField] private TextMeshProUGUI textDisplayer;
    [SerializeField] private BoxCollider2D textCollider;
    [SerializeField] private Color chosenColor;
    private float canvasScale;
    private float localClock;
    private Color initColor;

    /********************** System Calls **********************/
    private void Awake()
    {
        localClock = 0f;
        canvasScale = GetComponentInChildren<Canvas>().GetComponent<RectTransform>().localScale.x;
        AdaptColliderToText();
        initColor = textDisplayer.color;
    }

    private void Update()
    {
        localClock += Time.deltaTime;
        if (localClock >= 1f) localClock = 0f;
    }

    /********************** Public Methods **********************/
    public void ModifyTextContent(string newContent, int newFontSize)
    {
        textDisplayer.text = newContent;
        textDisplayer.fontSize = newFontSize;
        AdaptColliderToText();
    }
    public void ModifyTextColor(Color newColor)
    {
        textDisplayer.color = newColor;
    }
    public void ModifyColorToInit()
    {
        textDisplayer.color = initColor;
    }
    public void ModifyColorToChosen()
    {
        textDisplayer.color = chosenColor;
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    /********************** Functions **********************/
    private void AdaptColliderToText()
    {
        Vector2 textSiz = new Vector2(textDisplayer.preferredWidth * canvasScale, textDisplayer.preferredHeight * canvasScale);
        textCollider.size = textSiz + new Vector2(WIDTH_PADDING * canvasScale, HEIGHT_PADDING * canvasScale);
        textCollider.offset = new Vector2((textCollider.size.x - WIDTH_PADDING * canvasScale) / 2, -(textCollider.size.y - HEIGHT_PADDING * canvasScale) / 2);
    }
    
}
