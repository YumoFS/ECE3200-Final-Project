using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommunicateWithContact : MonoBehaviour
{
    [SerializeField] private GameObject button; 

    private void Start()
    {
        ButtonHoverEffect hoverEffect = button.GetComponent<ButtonHoverEffect>();
        if (hoverEffect == null)
        {
            hoverEffect = button.AddComponent<ButtonHoverEffect>();
            hoverEffect.hoverScale = 1.1f;
            hoverEffect.hoverColor = new Color(1f, 1f, 1f, 0.8f);
        }
    }

    public void JumpToContractScene()
    {
        SceneManager.LoadScene("Contract");
    }
}
