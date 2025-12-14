using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThroneScript : MonoBehaviour
{
    [SerializeField] private GameObject Enemy;
    [SerializeField] private GameObject gameInputs;
    [SerializeField] private GameObject dialogueCanvas;
    // [SerializeField] private DataManager dataManager;
    

    void Start()
    {
        if (DataManager.Instance != null)
        {
            PlayerData currentPlayerData = DataManager.Instance.LoadCheckpoint();
            if (currentPlayerData.deadCount > 2 || currentPlayerData.hasKilledBoss)
            {
                Enemy.SetActive(true);
                Destroy(gameObject);
            }
            else
            {
                if (DataManager.Instance != null)
                {
                    DataManager.Instance.SetEndingFlag("hasArrivedEmptyThrone", true);
                }
                DataManager.Instance.currentPlayerData.checkpointSceneName = "CastleOutside";
                DataManager.Instance.SaveToFile();
                gameInputs.SetActive(false);
                dialogueCanvas.SetActive(true);
            }
        }
    }
}
