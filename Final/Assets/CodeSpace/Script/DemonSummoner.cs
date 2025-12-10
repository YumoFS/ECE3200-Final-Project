using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonSummoner : MonoBehaviour
{
    [SerializeField] private float minDemonAppearTime;
    [SerializeField] private float maxDemonAppearTime;
    [SerializeField] private DebugConsole debugConsole;
    [SerializeField] private GameObject demon;
    private Transform attackPoint;
    private DestroyedData[] destroyedDatas;
    private float randomDestroyTime;
    private float count = 0;

    private void Awake()
    {
        demon.SetActive(false);
        GameObject[] destroyedDataObjects = GameObject.FindGameObjectsWithTag("DestroyedData");
        destroyedDatas = new DestroyedData[destroyedDataObjects.Length];
        for (int i = 0; i < destroyedDataObjects.Length; i ++ )
        {
            destroyedDatas[i] = destroyedDataObjects[i].GetComponent<DestroyedData>();
        }
        randomDestroyTime = Random.Range(minDemonAppearTime, maxDemonAppearTime);
        attackPoint = demon.transform.Find("AttackPoint");
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
                debugConsole.InsertInterferenceLog();
                SummonDemonAndDestroy(destroyedDatas[indexToBeDestroyed]);
            }
            count = 0;
            randomDestroyTime = Random.Range(minDemonAppearTime, maxDemonAppearTime);
        }
    }

    private void SummonDemonAndDestroy(DestroyedData destroyedData)
    {
        Vector3 summonPoint = destroyedData.transform.localPosition - attackPoint.transform.localPosition*demon.transform.localScale.x;
        demon.transform.SetLocalPositionAndRotation(summonPoint, new Quaternion());
        demon.SetActive(true);
        Animator demonAnimator = demon.GetComponent<Animator>();
        demonAnimator.SetTrigger("ToAppearAndAttack");
        StartCoroutine(DestroyRepairedDataAfter90Frames(destroyedData));
        StartCoroutine(DisableDemonAfter210Frames(destroyedData));
    }
    IEnumerator DestroyRepairedDataAfter90Frames(DestroyedData destroyedData)
    {
        yield return new WaitForSeconds(1.5f);
        destroyedData.ReturnToDestroyed();
    }
    IEnumerator DisableDemonAfter210Frames(DestroyedData destroyedData)
    {
        yield return new WaitForSeconds(3.5f);
        demon.SetActive(false);
    }

}
