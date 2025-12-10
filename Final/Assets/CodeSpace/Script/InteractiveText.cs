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
    public const int DEFAULT_FONTSIZE = 8;
    [SerializeField] private TextMeshProUGUI textDisplayer;
    [SerializeField] private BoxCollider2D textCollider;
    [SerializeField] private Color chosenColor;
    private Color failPromptColor;
    private CodeSpacePlayer player;
    private DebugConsole debugConsole;
    private float canvasScale;
    private Color initColor = new(192, 192, 192, 255);
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
        GameObject debugConsoleGameObject = GameObject.FindGameObjectWithTag("DebugConsole");
        debugConsole = debugConsoleGameObject.GetComponent<DebugConsole>();
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
    public void ReleaseAndMoveToRandomPlace()
    {
        transform.SetParent(null, true);
        float randomX = UnityEngine.Random.Range(-6.9f, 3.9f);
        float randomY = UnityEngine.Random.Range(-1.4f, 3.9f);
        Debug.Log(randomX + ", " + randomY);
        Vector3 randomPos = new(randomX, randomY, 0);
        SetNewInitColor(new Color(192, 192, 192, 255));
        transform.SetLocalPositionAndRotation(randomPos, new Quaternion());
        StartCoroutine(SetActiveAfter1s());
    }
    public void UpdateWrongPrompt()
    {
        wrongPrompted = false;
    }

    public void OnWrongTextMatched(Color textFailPrompt, Color consoleFailPrompt)
    {
        Debug.Log(this + "on wrong text matched");
        if (!isPromptingWrong && !wrongPrompted)
        {
            count = 0;
            isPromptingWrong = true;
            failPromptColor = textFailPrompt;
            debugConsole.InsertLog($"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGBA(consoleFailPrompt)}>{password} should not be placed here...</color>");
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

    IEnumerator SetActiveAfter1s()
    {
        yield return new WaitForSeconds(1f);
        isInteractive = true;
    }
}
