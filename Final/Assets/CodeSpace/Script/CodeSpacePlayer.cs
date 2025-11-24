using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CodeSpacePlayer : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Canvas playerTexture;
    [SerializeField] private OverlapManager overlapManager;
    private const float moveSpeed = 1f;
    private void Start()
    {
        inputManager.OnInteractAction += InputManager_OnInteractAction;
    }
    private void InputManager_OnInteractAction(object sender, EventArgs e)
    {
        Debug.Log("F pressed");
    }
    private void Update()
    {
        PlayerMovementHandler();
    }

    public Collider2D PlayerTopCollider()
    {
        return overlapManager.GetTopContactCollider();
    }
    private void PlayerMovementHandler()
    {
        Vector2 deltaPosition = moveSpeed * Time.deltaTime * inputManager.GetMovementDirection();
        transform.position += (Vector3)deltaPosition;
        if (deltaPosition.x != 0)
        {
            if (Sgn(deltaPosition.x))
            {
                playerTexture.transform.rotation = new Quaternion(0, 0, 0, 0);
            }
            else
            {
                playerTexture.transform.rotation = new Quaternion(0, 180, 0, 0);
            }
        }
    }
    private bool Sgn(float x)
    {
        return x > 0;
    }
}
