using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RepairPercentage : MonoBehaviour
{
    [SerializeField] private InteractiveText percentageText;
    [SerializeField] private Color ascendingColor;
    [SerializeField] private Color descendingColor;
    private Color defaultColor;
    private DestroyedData[] destroyedDatas;
    private float repairPercentage = 0;
    private float recoverCount = -1f;


    private void Awake()
    {
        GameObject[] destroyedDataObjects = GameObject.FindGameObjectsWithTag("DestroyedData");
        destroyedDatas = new DestroyedData[destroyedDataObjects.Length];
        for (int i = 0; i < destroyedDataObjects.Length; i ++ )
        {
            destroyedDatas[i] = destroyedDataObjects[i].GetComponent<DestroyedData>();
        }
        defaultColor = percentageText.GetDefaultColor();
    }

    private void Update()
    {
        float currentRepairPercentage = GetCurrentRepairPercentage();
        if (currentRepairPercentage > repairPercentage)
        {
            IncreasePercentageText(currentRepairPercentage);
        }
        else if (currentRepairPercentage < repairPercentage)
        {
            DecreasePercentageText(currentRepairPercentage);
        }
        repairPercentage = currentRepairPercentage;

        if (currentRepairPercentage == 1f)
        {
            DataManager.Instance.SetEndingFlag("hasPassedCodeSpace", true);
            Debug.Log("Enter Start here");
            SceneManager.LoadScene("Start");
        }

        recoverCount -= Time.deltaTime;
        if (recoverCount <= 0f)
        {
            percentageText.SetNewInitColor(defaultColor);
            percentageText.SetTextColor(defaultColor);
            recoverCount = -1f;
        }
    }

    
    private float GetCurrentRepairPercentage()
    {
        int filledDestroyedData = 0;
        for (int i = 0; i < destroyedDatas.Length; i ++)
        {
            if (destroyedDatas[i].IsFilled())
            {
                filledDestroyedData ++;
            }
        }
        return filledDestroyedData / (float)destroyedDatas.Length;
    }

    private void IncreasePercentageText(float percentage)
    {
        float percentageDot2f = (int)(percentage * 10000) / (float)100f;
        percentageText.SetTextContent(percentageDot2f.ToString() + "%", InteractiveText.DEFAULT_FONTSIZE);
        percentageText.SetNewInitColor(ascendingColor);
        percentageText.SetTextColor(ascendingColor);
        recoverCount = 1f;
    }

    private void DecreasePercentageText(float percentage)
    {
        float percentageDot2f = (int)(percentage * 10000) / (float)100f;
        percentageText.SetTextContent(percentageDot2f.ToString() + "%", InteractiveText.DEFAULT_FONTSIZE);
        percentageText.SetNewInitColor(descendingColor);
        percentageText.SetTextColor(descendingColor);
        recoverCount = 1f;
    }

    



}
