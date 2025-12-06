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
    private const float WRONG_MATCH_BLINKING = .25f;
    public const int DEFAULT_FONTSIZE = 12;
    [SerializeField] private TextMeshProUGUI textDisplayer;
    [SerializeField] private BoxCollider2D textCollider;
    [SerializeField] private Color chosenColor;
    private Color failPromptColor;
    private CodeSpacePlayer player;
    private float canvasScale;
    private Color initColor;
    private bool isPromptingWrong = false;
    private bool wrongPrompted = false;
    private float count;
    public string password = "";
    public bool isInteractive = true;

    /********************** System Calls **********************/
    private void Awake()
    {
        count = 0;
        canvasScale = GetComponentInChildren<Canvas>().GetComponent<RectTransform>().localScale.x;
        AdaptColliderToText();
        initColor = textDisplayer.color;
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("CodeSpacePlayer");
        player = playerGameObject.GetComponent<CodeSpacePlayer>();
    }

    private void Update()
    {
        if (isPromptingWrong && !wrongPrompted)
        {
            FailPromptHandler();
        }
        else
        {
            if (!player.overlapManager.IsContacting() || player.IsCarrying() || isInteractive) {
                SetColorToInit();
            }
        }
    }

    /********************** Public Methods **********************/
    public BoxCollider2D GetCollider()
    {
        return textCollider;
    }
    public void SetTextContent(string newContent, int newFontSize)
    {
        textDisplayer.text = newContent;
        textDisplayer.fontSize = newFontSize;
        AdaptColliderToText();
    }
    public void SetTextColor(Color newColor)
    {
        if (!isPromptingWrong) textDisplayer.color = newColor;
    }
    public void SetColorToInit()
    {
        if (!isPromptingWrong) textDisplayer.color = initColor;
    }
    public void SetColorToChosen()
    {
        if (!isPromptingWrong) textDisplayer.color = chosenColor;
    }
    public void SetNewInitColor(Color newColor)
    {
        initColor = newColor;
    }
    public void SetNewChosenColor(Color newColor)
    {
        chosenColor = newColor;
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    public void UpdateWrongPrompt()
    {
        wrongPrompted = false;
    }

    public void OnWrongTextMatched(Color matchFailPrompt)
    {
        Debug.Log(this + "on wrong text matched");
        if (!isPromptingWrong && !wrongPrompted)
        {
            count = 0;
            isPromptingWrong = true;
            failPromptColor = matchFailPrompt;
        }
    }

    /********************** Functions **********************/
    private void AdaptColliderToText()
    {
        Vector2 textSiz = new Vector2(textDisplayer.preferredWidth * canvasScale, textDisplayer.preferredHeight * canvasScale);
        textCollider.size = textSiz + new Vector2(WIDTH_PADDING * canvasScale, HEIGHT_PADDING * canvasScale);
        textCollider.offset = new Vector2((textCollider.size.x - WIDTH_PADDING * canvasScale) / 2, -(textCollider.size.y - HEIGHT_PADDING * canvasScale) / 2);
    }
    private void FailPromptHandler()
    {
        count += Time.deltaTime;
        if (count > 0 && count <= WRONG_MATCH_BLINKING)
        {
            textDisplayer.color = failPromptColor;
        }
        else if (count > WRONG_MATCH_BLINKING && count <= 2*WRONG_MATCH_BLINKING)
        {
            textDisplayer.color = initColor;
        }
        else if (count > 2*WRONG_MATCH_BLINKING && count <= 3*WRONG_MATCH_BLINKING)
        {
            textDisplayer.color = failPromptColor;
        }
        else
        {
            textDisplayer.color = initColor;
            count = 0;
            isPromptingWrong = false;
            wrongPrompted = true;
        }
    }
}
