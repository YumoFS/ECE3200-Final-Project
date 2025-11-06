using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private Transform targetPlayer;
    private Vector3 offset;

    void Start()
    {
        offset = transform.position - targetPlayer.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = targetPlayer.position + offset;
    }
}
