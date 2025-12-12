using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContractScript : Interactable
{
    [SerializeField] private GameObject contract;
    public override void Interact()
    {
        contract.SetActive(!contract.activeSelf);
        Debug.Log("与方块交互！");
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerEnter();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerExit();
        }
    }
}
