using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameInputs gameInputs;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private int jumpNumMax = 2;

    private Rigidbody2D rb;
    private int jumpNumCount;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();

        if (rb.velocity.y == 0)
        {
            jumpNumCount = 0;
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInputs.GetmovementVectorNormalize();
        Vector3 moveDir = new Vector3(inputVector.x, inputVector.y, 0f);

        float moveDistance = moveSpeed * Time.deltaTime;
        Vector2 playerSize = new Vector2(0.3f, 0.3f);
        bool canMove = !Physics2D.CapsuleCast(transform.position, playerSize, CapsuleDirection2D.Vertical, 0f, inputVector, 0.01f);

        if (!canMove)
        {
            transform.position += moveDir * moveDistance;
        }
    }
    
    private void HandleJump()
    {
        if (gameInputs.IsJumpPressed() && jumpNumCount < jumpNumMax)
        {
            jumpNumCount++;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}
