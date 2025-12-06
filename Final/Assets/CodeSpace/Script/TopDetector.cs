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
        if (topCollider != null && !player.IsCarrying())
        {
            InteractiveText chosenText = topCollider.GetComponent<InteractiveText>();
            if (chosenText.isInteractive)
            {
                chosenText.SetColorToChosen();
                if (lastChosenText != chosenText && lastChosenText != null)
                {
                    lastChosenText.SetColorToInit();
                }
                lastChosenText = chosenText;
            }
        }
    }
    public InteractiveText PlayerChosenText()
    {
        return lastChosenText;
    }
}
