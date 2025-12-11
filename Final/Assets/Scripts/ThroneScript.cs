using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThroneScript : MonoBehaviour
{
    [SerializeField] private GameObject Enemy;
    

    void Start()
    {
        if (DataManager.Instance != null)
        {
            PlayerData currentPlayerData = DataManager.Instance.LoadCheckpoint();
            if (currentPlayerData.deadCount > 2)
            {
                Enemy.SetActive(true);
                Destroy(gameObject);
            }
        }
    }
}
