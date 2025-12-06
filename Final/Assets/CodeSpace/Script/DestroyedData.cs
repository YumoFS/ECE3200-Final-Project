using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedData : MonoBehaviour
{
    [SerializeField] private InteractiveText text;
    [SerializeField] private int randomCodeLen;
    [SerializeField] private OverlapManager overlapManager;
    [SerializeField] private Color filledColor;
    [SerializeField] private Color failedPromptColor;
    private float counter;
    private CodeSpacePlayer player;
    private bool isFilled = false;


    /******* Private Functions ******/
    private string GenerateRandomCode()
    {
        string randomCode = "";
        for (int i = 0; i < randomCodeLen; i ++)
        {
            char c = (char)UnityEngine.Random.Range(33, 127);
            randomCode += c;
        }
        return randomCode;
    }

    private void TimeHandler()
    {
        counter += Time.deltaTime;
        if (counter >= .15f)
        {
            counter = 0;
            text.SetTextContent(GenerateRandomCode(), InteractiveText.DEFAULT_FONTSIZE);
        }
    }

    public void TextCollideHandler()
    {
        if (overlapManager.IsContacting() && !isFilled)
        {
            Collider2D[] allContactTextColliders = overlapManager.GetAllContactColliders();
            int contactNum = overlapManager.GetContactColliderNum();
            Debug.Log(allContactTextColliders[0]);
            for (int i = 0; i < contactNum; i ++)
            {
                
                InteractiveText contactText = allContactTextColliders[i].GetComponent<InteractiveText>();
                if (contactText.password == text.password)
                {
                    text.gameObject.SetActive(false);
                    contactText.transform.SetParent(this.transform);
                    contactText.transform.SetLocalPositionAndRotation(Vector3.zero, new Quaternion());
                    contactText.isInteractive = false;
                    contactText.SetTextColor(filledColor);
                    contactText.SetNewInitColor(filledColor);
                    isFilled = true;
                }
                else
                {
                    contactText.OnWrongTextMatched(failedPromptColor);
                }
            }
        }

    }


    /******* System Calls *******/
    private void Awake()
    {
        counter = 0;
        text.isInteractive = false;
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("CodeSpacePlayer");
        player = playerGameObject.GetComponent<CodeSpacePlayer>();
        Debug.Log(player);
    }

    private void Update()
    {
        TimeHandler();
        if (!player.IsCarrying() && !isFilled)
        {
            TextCollideHandler();
        }
    }
}
