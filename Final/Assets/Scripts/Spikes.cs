using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] Player player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        player.deadReason = "Spikes";
        player.isAlive = false;
        player.playerHitPoint -= 10;
        Debug.Log("Is triggered");
    }
}
