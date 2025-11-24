using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDetector : MonoBehaviour
{
    [SerializeField] CodeSpacePlayer player;
    private InteractiveText lastChosenText;
    void Awake()
    {
        lastChosenText = null;
    }
    void Update()
    {
        Collider2D topCollider = player.PlayerTopCollider();
        if (topCollider != null)
        {
            InteractiveText chosenText = topCollider.GetComponent<InteractiveText>();
            Debug.Log(chosenText);
            if (chosenText != null)
            {
                if (lastChosenText != chosenText && lastChosenText != null)
                {
                    lastChosenText.ModifyColorToInit();
                }
                chosenText.ModifyColorToChosen();
            }
            lastChosenText = chosenText;
        }
    }
    public InteractiveText PlayerChosenText()
    {
        return lastChosenText;
    }
}
