using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isAlive;
    public string deadReason;
    public int playerHitPoint = 1;

    [SerializeField] private GameInputs gameInputs;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private int jumpNumMax = 2;
    [SerializeField] private Vector3 playerInitialPosition;
    
    [Header("交互相关")]
    [SerializeField] private float interactionRange = 1.5f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("玩家数据相关")]
    private static int playerHitPointMax = 1;
    [SerializeField] private int playerAttackPower = 1;
    [SerializeField] private float playerAttackDistance = 2f;
    [SerializeField] private float attackColldown = .5f;

    [SerializeField] private Transform attackPosition; 
    [SerializeField] private LayerMask enemyLayers;

    private Rigidbody2D rb;
    private int jumpNumCount;
    private Interactable currentInteractable;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        isAlive = true;
        playerInitialPosition = transform.position;
        playerHitPoint = playerHitPointMax;

        if (attackPosition == null) attackPosition = transform;
    }

    private void Update()
    {
        if (playerHitPoint <= 0){
            DeadAction();
        }

        HandleMovement();
        HandleJump();
        HandleAttack();
        CheckForInteractables();
        HandleInteraction();

        if (Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            jumpNumCount = 0;
        }
    }

    private void HandleAttack()
    {
        if (gameInputs.IsAttackPressed())
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
                attackPosition.position, 
                playerAttackDistance, 
                enemyLayers
            );
        
            // 对每个命中的敌人造成伤害
            foreach (Collider2D enemy in hitEnemies)
            {
                Debug.Log("击中敌人: " + enemy.name);
                
                // 获取敌人的生命值组件并造成伤害
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(playerAttackPower);
                }
            }
            
        }
    }

    private void CheckForInteractables()
    {
        // 使用圆形检测周围的交互物体
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
            transform.position, 
            interactionRange, 
            interactableLayer
        );
        
        Interactable nearestInteractable = null;
        float nearestDistance = float.MaxValue;
        
        foreach (var hitCollider in hitColliders)
        {
            Interactable interactable = hitCollider.GetComponent<Interactable>();
            if (interactable != null)
            {
                float distance = Vector2.Distance(transform.position, hitCollider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestInteractable = interactable;
                }
            }
        }
        
        currentInteractable = nearestInteractable;
    }
    
    private void HandleInteraction()
    {
        if (gameInputs.IsInteractPressed() && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void DeadAction()
    {
        switch (deadReason)
        {
            case "Spikes":
                DeadBySpikes();
                break;
            case "IronVirgin":
                DeadByIronVirgin();
                break;
        }
        transform.position = playerInitialPosition;
        rb.velocity = Vector3.zero;
        isAlive = true;
        playerHitPoint = playerHitPointMax;
    }

    private void DeadBySpikes()
    {
        
    }

    private void DeadByIronVirgin()
    {

    }

    // private void HandleMovement()
    // {
    //     Vector2 inputVector = gameInputs.GetmovementVectorNormalize();
    //     Vector3 moveDir = new Vector3(inputVector.x, 0f, 0f);

    //     float moveDistance = moveSpeed * Time.deltaTime;
        
    //     // 修复碰撞检测逻辑
    //     Vector2 playerSize = GetComponent<Collider2D>().bounds.size * 0.9f; // 使用实际碰撞器大小，稍微缩小一点避免卡住
        
    //     // 只在水平方向检测碰撞
    //     if (moveDir.x != 0)
    //     {
    //         // 使用Raycast而不是CapsuleCast，更简单可靠
    //         RaycastHit2D hit = Physics2D.Raycast(
    //             transform.position, 
    //             moveDir, 
    //             playerSize.x / 2 + moveDistance, 
    //             GetGroundLayerMask()
    //         );
            
    //         // 如果没有碰撞，则可以移动
    //         if (hit.collider == null)
    //         {
    //             transform.position += moveDir * moveDistance;
    //         }
    //         else
    //         {
    //             Debug.Log($"碰撞阻碍移动，碰撞对象: {hit.collider.gameObject.name}");
    //         }
    //     }
    // }

    // // 获取地面图层掩码（排除玩家自身）
    // private LayerMask GetGroundLayerMask()
    // {
    //     // 返回所有图层除了玩家图层
    //     return ~(1 << gameObject.layer);
    // }
    
    private void HandleMovement()
    {
        Vector2 inputVector = gameInputs.GetmovementVectorNormalize();
        
        // 直接设置水平速度，让物理引擎处理碰撞
        rb.velocity = new Vector2(inputVector.x * moveSpeed, rb.velocity.y);
    }
    
    private void HandleJump()
    {
        if (gameInputs.IsJumpPressed() && jumpNumCount < jumpNumMax)
        {
            jumpNumCount++;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
    
    // 在Scene视图中显示交互范围（调试用）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}