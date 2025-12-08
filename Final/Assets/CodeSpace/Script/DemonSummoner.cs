using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonSummoner : MonoBehaviour
{
    [SerializeField] private float minDemonAppearTime;
    [SerializeField] private float maxDemonAppearTime;
    private DestroyedData[] destroyedDatas;
    private float randomDestroyTime;
    private float count = 0;

    private void Awake()
    {
        GameObject[] destroyedDataObjects = GameObject.FindGameObjectsWithTag("DestroyedData");
        destroyedDatas = new DestroyedData[destroyedDataObjects.Length];
        for (int i = 0; i < destroyedDataObjects.Length; i ++ )
        {
            destroyedDatas[i] = destroyedDataObjects[i].GetComponent<DestroyedData>();
        }
        randomDestroyTime = Random.Range(minDemonAppearTime, maxDemonAppearTime);
    }

    private void Update()
    {
        count += Time.deltaTime;
        if (count >= randomDestroyTime)
        {
            int fixedDataNum = 0; int[] fixedDataIndexSet = new int[destroyedDatas.Length];
            for (int i = 0; i < destroyedDatas.Length; i ++)
            {
                if (destroyedDatas[i].IsFilled())
                {
                    fixedDataIndexSet[fixedDataNum++] = i;
                }
            }
            if (fixedDataNum > 0)
            {
                int indexToBeDestroyed = fixedDataIndexSet[Random.Range(0, fixedDataNum)];
                destroyedDatas[indexToBeDestroyed].ReturnToDestroyed();
            }
            count = 0;
            randomDestroyTime = Random.Range(minDemonAppearTime, maxDemonAppearTime);
        }
    }

}
