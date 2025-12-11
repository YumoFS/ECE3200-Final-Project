using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    [SerializeField] private Image enemyHPBar;
    [SerializeField] private EnemyHealth enemyHealth;
    private float HPPercent; 

    private void Start()
    {
        enemyHPBar.fillAmount = 1f;
    }

    private void Update()
    {
        HPPercent = (float) enemyHealth.currentHealth / enemyHealth.maxHealth;
        enemyHPBar.fillAmount = HPPercent;
        // if (HPPercent < 0.01f)
        // {
        //     Destroy(gameObject);
        // }
    }
}
