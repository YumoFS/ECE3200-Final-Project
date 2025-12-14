using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GodScript : MonoBehaviour
{
    private float currentTime = 0f;
    private float targetTime = 15f;

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= targetTime)
        {
            DataManager.Instance.currentPlayerData.hasPassedHeaven = true;
            // SceneManager.LoadScene("CastleOutside");
            SceneTransitionManager.Instance.LoadSceneWithSave("CastleOutside");
        }
    }
}
