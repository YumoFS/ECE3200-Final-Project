using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumHammer : MonoBehaviour
{
    [SerializeField] private Player player;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.deadReason = "Pendulum";
            player.isAlive = false;
            player.playerHitPoint -= 10;
        }
    }
}
