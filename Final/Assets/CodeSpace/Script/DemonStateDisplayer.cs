using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class DemonStateDisplayer : MonoBehaviour
{

    [SerializeField] private InteractiveText displayerText;
    [SerializeField] private DebugConsole debugConsole;
    [SerializeField] private DemonSummoner demonSummoner;
    [SerializeField] private Color demonUserColor;
    private const string LUCIUS_USERNAME = "Lucius";
    private const string DEMON_USERNAME = "4m87_9ntr7td (Admin)";
    private string LUCIUS_IP;
    private const string DEMON_IP = "666.666.666.666";
    private bool isDemonLoginPrinted = false;
    private string highlightedDemonIP;
    private string highlightedDemonUser;
    private void Awake()
    {
        LUCIUS_IP = GetLocalIPv4Address();
        PrintCodeSpaceEntranceLog(LUCIUS_IP, LUCIUS_USERNAME);
        highlightedDemonIP = $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGBA(demonUserColor)}>" + DEMON_IP + "</color>";
        highlightedDemonUser = $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGBA(demonUserColor)}>" + DEMON_USERNAME + "</color>";
    }
    private void Update()
    {
        if (demonSummoner.IsDemonBeingAwaked())
        {
            if(!isDemonLoginPrinted)
            {
                isDemonLoginPrinted = true; 
                PrintCodeSpaceEntranceLog(highlightedDemonIP, highlightedDemonUser);
            }
            
            displayerText.SetTextContent(LUCIUS_USERNAME + ", " + highlightedDemonUser, InteractiveText.DEFAULT_FONTSIZE);
        }
        else
        {
            if(isDemonLoginPrinted) isDemonLoginPrinted = false;
            displayerText.SetTextContent(LUCIUS_USERNAME, InteractiveText.DEFAULT_FONTSIZE);
        }
    }

    private void PrintCodeSpaceEntranceLog(string ip, string username)
    {
        debugConsole.InsertLog("Remote Connection at " + ip + " is received.");
        debugConsole.InsertLog("Connection Accepted, welcome, " + username + ".");
    }





    private string GetLocalIPv4Address()
    {
        string hostName = Dns.GetHostName();
        IPHostEntry hostEntry = Dns.GetHostEntry(hostName);


        foreach (IPAddress IP in hostEntry.AddressList)
        {
            if (IP.AddressFamily == AddressFamily.InterNetwork)
                return IP.ToString();
        }

        return "";
    }
}
