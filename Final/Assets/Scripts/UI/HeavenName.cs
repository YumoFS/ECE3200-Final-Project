using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeavenName : MonoBehaviour
{
    [SerializeField] private TMP_Text names;

    private void Start()
    {
        names.text = string.Join(",", DataManager.Instance.nameHistory);
    }
}
