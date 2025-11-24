using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isAlive;
    public string deadReason;
    public int playerHitPoint = 1;
    public bool hasTorch;
    public int deadCount;
    public int winCount;

    [SerializeField] private GameInputs gameInputs;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private int jumpNumMax = 2;
    [SerializeField] private Vector3 playerInitialPosition;
    
    [Header("动画相关")]
    [SerializeField] private Animator animator;
    
    [Header("交互相关")]
    [SerializeField] private float interactionRange = 1.5f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("玩家数据相关")]
    private static int playerHitPointMax = 1;
    [SerializeField] private int playerAttackPower = 1;
    [SerializeField] private float playerAttackDistance = 2f;
    [SerializeField] private float attackCooldown = 0.5f;

    [SerializeField] private Transform attackPosition; 
    [SerializeField] private LayerMask enemyLayers;

    private Rigidbody2D rb;
    private int jumpNumCount;
    private bool isInAir;
    private Interactable currentInteractable;
    
    // 动画状态变量
    private bool isAttacking = false;
    private float attackCooldownTimer = 0f;
    private float lastHorizontalInput = 1f; // 默认朝右

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // 如果Animator没有在Inspector中分配，尝试获取
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        isAlive = true;
        playerInitialPosition = transform.position;
        playerHitPoint = playerHitPointMax;
        hasTorch = false;

        if (attackPosition == null) attackPosition = transform;
    }

    private void Update()
    {
        if (transform.position.y < -5f)
        {
            playerHitPoint = 0;
            isAlive = false;
            deadReason = "Falling";
        }

        if (playerHitPoint <= 0){
            DeadAction();
        }

        // 更新攻击冷却
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        HandleMovement();
        HandleJump();
        HandleAttack();
        CheckForInteractables();
        HandleInteraction();
        UpdateAnimations();

        if (Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            jumpNumCount = 0;
            isInAir = false;
        }
        else if (!isInAir)
        {
            jumpNumCount ++;
            isInAir = true;
        }
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        // 设置移动速度参数
        float horizontalSpeed = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("Speed", horizontalSpeed);
        
        // 设置跳跃/下落状态
        animator.SetBool("IsGrounded", !isInAir);
        animator.SetFloat("VerticalVelocity", rb.velocity.y);
        
        // 设置攻击状态
        animator.SetBool("IsAttacking", isAttacking);
        
        // 更新角色朝向
        Vector2 inputVector = gameInputs.GetmovementVectorNormalize();
        if (inputVector.x != 0 && !isAttacking)
        {
            lastHorizontalInput = Mathf.Sign(inputVector.x);
            transform.localScale = new Vector3(lastHorizontalInput, 1, 1);
        }
    }

    private void HandleAttack()
    {
        if (gameInputs.IsAttackPressed() && attackCooldownTimer <= 0 && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private System.Collections.IEnumerator PerformAttack()
    {
        isAttacking = true;
        attackCooldownTimer = attackCooldown;
        
        // 触发攻击动画
        animator.SetTrigger("Attack");
        
        // 等待动画的命中帧（你可以调整这个时间）
        yield return new WaitForSeconds(0.2f);
        
        // 执行攻击检测
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
        
        // 等待攻击动画完成（你可以通过动画事件来更精确地控制）
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }

    // 动画事件方法 - 可以在动画时间线中调用
    public void OnAttackHitFrame()
    {
        // 这个方法可以在攻击动画的特定帧被调用
        Debug.Log("攻击命中帧");
    }
    
    public void OnAttackEnd()
    {
        isAttacking = false;
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
        // 触发死亡动画
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        
        switch (deadReason)
        {
            case "Spikes":
                DeadBySpikes();
                break;
            case "IronVirgin":
                DeadByIronVirgin();
                break;
            case "Pendulum":
                DeadByPendulum();
                break;
            case "Falling":
                DeadByFalling();
                break;
        }
        
        // 等待一小段时间再重置（让死亡动画播放）
        StartCoroutine(RespawnAfterDeath());
    }

    private System.Collections.IEnumerator RespawnAfterDeath()
    {
        yield return new WaitForSeconds(1f);
        
        transform.position = playerInitialPosition;
        rb.velocity = Vector3.zero;
        isAlive = true;
        playerHitPoint = playerHitPointMax;
        
        // 重置动画状态
        if (animator != null)
        {
            animator.SetTrigger("Respawn");
        }
    }

    private void DeadByFalling() { }
    private void DeadByPendulum() { }
    private void DeadBySpikes() { }
    private void DeadByIronVirgin() { }
    
    private void HandleMovement()
    {
        if (isAttacking) return; // 攻击时不能移动
        
        Vector2 inputVector = gameInputs.GetmovementVectorNormalize();
        
        // 直接设置水平速度，让物理引擎处理碰撞
        rb.velocity = new Vector2(inputVector.x * moveSpeed, rb.velocity.y);
    }
    
    private void HandleJump()
    {
        if (isAttacking) return; // 攻击时不能跳跃
        
        if (gameInputs.IsJumpPressed() && jumpNumCount < jumpNumMax)
        {
            jumpNumCount++;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
    
    // 在Scene视图中显示交互范围和攻击范围（调试用）
    private void OnDrawGizmosSelected()
    {
        // 交互范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
        
        // 攻击范围
        if (attackPosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPosition.position, playerAttackDistance);
        }
    }
}