using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] Player player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        player.isAlive = false;
        Debug.Log("Is triggered");
    }
}
