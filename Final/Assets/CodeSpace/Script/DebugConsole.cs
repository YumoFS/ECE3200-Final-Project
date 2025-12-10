using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    /****** Variables ******/
    [SerializeField] private string[] interferenceLog;
    [SerializeField] private int consoleFontSize;
    [SerializeField] private float verticalSpacing;
    [SerializeField] private Color consoleFontColor;
    [SerializeField] private InteractiveText interactiveTextPrefab;
    private const int MAXLOG = 6;
    private int consoleLogNum;
    private InteractiveText[] consoleLogs = new InteractiveText[MAXLOG];
    private float counter = 0f;
    private int[] displayedInterference = new int[MAXLOG];
    private int displayedInterferenceIndex = 0;



    /****** System Calls ******/

    private void Awake()
    {
        consoleLogNum = 0;
        for (int i = 0; i < MAXLOG; i ++)
        {
            displayedInterference[i] = -1;
        }
    }

    private void Update()
    {
        counter += Time.deltaTime;
        if (counter > 2f)
        {
            counter = 0f;
        }
    }

    /****** Public Methods ******/
    public void InsertLog(string strToInsert, bool useSystemTime = true)
    {
        string formalizedLog;
        if (useSystemTime)
        {
            formalizedLog = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + strToInsert;
        }
        else
        {
            formalizedLog = strToInsert;
        }
        InteractiveText insertText = CreateInteractiveTextChild(formalizedLog, consoleFontColor);
        if (consoleLogNum < MAXLOG)
        {
            consoleLogs[consoleLogNum] = insertText;
            float currentHeight = 0f;
            for (int i = 0; i < consoleLogNum; i ++)
            {
                currentHeight -= consoleLogs[i].GetCollider().size.y + verticalSpacing;
            }
            insertText.transform.localPosition = new Vector3(0, currentHeight, 0);
            consoleLogNum ++;
        }
        else
        {
            float offsetHeight = consoleLogs[0].GetCollider().size.y + verticalSpacing;
            consoleLogs[0].DestroySelf();
            for (int i = 0; i < MAXLOG-1; i ++)
            {
                consoleLogs[i] = consoleLogs[i+1];
                consoleLogs[i].transform.localPosition += new Vector3(0, offsetHeight, 0);
            }
            consoleLogs[MAXLOG-1] = insertText;
            float currentHeight = 0f;
            for (int i = 0; i < MAXLOG-1; i ++)
            {
                currentHeight -= consoleLogs[i].GetCollider().size.y + verticalSpacing;
            }
            insertText.transform.localPosition = new Vector3(0, currentHeight, 0);
        }

    }
    public void InsertInterferenceLog(bool useSystemTime = true)
    {
        InsertLog(GetRandomInterference(), useSystemTime);
    }


    /****** Private Functions ******/
    private string GetRandomInterference()
    {
        int logLength = interferenceLog.Length;
        if (logLength == 0) return "";
        if (interferenceLog.Length <= MAXLOG)
        {
            int randLogIndex = UnityEngine.Random.Range(0, logLength);
            return interferenceLog[randLogIndex];
        }
        else
        {
            while (true)
            {
                bool isSuccessfullyRandomize = true;
                int randLogIndex = UnityEngine.Random.Range(0, logLength);
                for (int i = 0; i < MAXLOG; i ++)
                {
                    if (randLogIndex == displayedInterference[i])
                        isSuccessfullyRandomize = false;
                }
                if (isSuccessfullyRandomize)
                {
                    displayedInterference[displayedInterferenceIndex] = randLogIndex;
                    if (displayedInterferenceIndex == MAXLOG - 1)
                        displayedInterferenceIndex = 0;
                    else
                        displayedInterferenceIndex += 1;
                    return interferenceLog[randLogIndex];
                }
            }
        }
    }
    private InteractiveText CreateInteractiveTextChild(string content, Color color)
    {
        InteractiveText newInteractiveText = Instantiate(interactiveTextPrefab, transform);
        newInteractiveText.SetTextContent(content, consoleFontSize);
        newInteractiveText.SetTextColor(color);
        newInteractiveText.SetNewInitColor(color);
        newInteractiveText.isInteractive = false;
        return newInteractiveText;
    }
    
}
