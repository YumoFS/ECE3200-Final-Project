using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class CodeSpacePlayer : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Canvas playerTexture;
    [SerializeField] private GameObject carryPoint;
    public OverlapManager overlapManager;
    private const float moveSpeed = 1f;
    private bool isCarrying;
    private void Start()
    {
        isCarrying = false;
        inputManager.OnInteractAction += InputManager_OnInteractAction;
    }
    private void InputManager_OnInteractAction(object sender, EventArgs e)
    {
        if (overlapManager.IsContacting())
        {
            if (!isCarrying)
                SetTextAsChild(overlapManager.GetTopContactCollider().GetComponent<InteractiveText>());
            else
                ReleaseTextFromChild();
            isCarrying = !isCarrying;
        }
    }
    private void Update()
    {
        PlayerMovementHandler();
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

    private void SetTextAsChild(InteractiveText interactiveText) {
        interactiveText.transform.SetParent(carryPoint.transform);
        BoxCollider2D textCollider = interactiveText.GetCollider();
        Vector3 colliderCenter = new Vector3(-textCollider.offset.x, -textCollider.offset.y, 0);
        interactiveText.transform.SetLocalPositionAndRotation(colliderCenter, new Quaternion());
    }

    private void ReleaseTextFromChild() {
        if (!isCarrying) return;
        InteractiveText carryingText = carryPoint.GetComponentInChildren<InteractiveText>();
        carryingText.transform.SetParent(null, true);
    }


    public Collider2D PlayerTopCollider()
    {
        return overlapManager.GetTopContactCollider();
    }
    public bool IsCarrying() {
        return isCarrying;
    }
}
