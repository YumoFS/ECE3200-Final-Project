using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BackgroundDigit : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI backgroundText;
    private float timeCounter;
    private void GenerateRandomMatrix()
    {
        string res = "";
        for (int j = 0; j < 26; j ++)
        {
            for (int i = 0; i < 49; i++)
            {
                res += (int)(Random.value * 2);
            }
            res += '\n';
        }
        backgroundText.text = res;
    }
    void Start()
    {
        GenerateRandomMatrix();
        timeCounter = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timeCounter += Time.deltaTime;
        if (timeCounter >= 1f)
        {
            GenerateRandomMatrix();
            timeCounter = 0f;
        }
    }
}
