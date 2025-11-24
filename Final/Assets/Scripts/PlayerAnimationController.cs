using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("引用组件")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInput playerInput;
    
    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float attackMoveSpeed = 2f;
    
    // 输入值
    private Vector2 moveInput;
    private bool attackInput;
    
    // 状态变量
    private bool isAttacking = false;
    private bool canMove = true;
    private float currentMoveSpeed;
    
    // 输入动作引用
    private InputAction moveAction;
    private InputAction attackAction;

    void Start()
    {
        // 获取输入动作
        moveAction = playerInput.actions["Move"];
        attackAction = playerInput.actions["Attack"];
        
        currentMoveSpeed = moveSpeed;
        
        // 注册输入回调
        attackAction.performed += OnAttackPerformed;
    }

    void Update()
    {
        GetInput();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void GetInput()
    {
        // 读取移动输入
        moveInput = moveAction.ReadValue<Vector2>();
        
        // 在攻击期间限制移动输入
        if (isAttacking)
        {
            moveInput *= 0.3f; // 攻击时可以轻微移动，或者设为0完全禁止
        }
    }

    void HandleMovement()
    {
        if (!canMove) return;
        
        // 应用移动
        Vector2 velocity = moveInput * currentMoveSpeed;
        rb.velocity = new Vector2(velocity.x, rb.velocity.y);
        
        // 更新角色朝向
        if (moveInput.x != 0 && !isAttacking)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput.x), 1, 1);
        }
    }

    void UpdateAnimations()
    {
        // 设置移动速度参数
        float horizontalSpeed = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("Speed", horizontalSpeed);
        
        // 设置攻击状态
        animator.SetBool("IsAttacking", isAttacking);
    }

    void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (!isAttacking && canMove)
        {
            StartAttack();
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        
        // 攻击时降低移动速度
        currentMoveSpeed = attackMoveSpeed;
    }

    // 动画事件 - 攻击开始
    public void OnAttackStart()
    {
        // 可以在这里设置完全禁止移动
        // canMove = false;
    }

    // 动画事件 - 攻击结束
    public void OnAttackEnd()
    {
        isAttacking = false;
        currentMoveSpeed = moveSpeed;
        // canMove = true;
    }

    // 动画事件 - 攻击命中帧
    public void OnAttackHit()
    {
        // 在这里处理攻击检测
        Debug.Log("Attack hit frame!");
        // CombatManager.Instance.DetectHit(transform);
    }

    void OnDestroy()
    {
        // 取消注册回调
        if (attackAction != null)
            attackAction.performed -= OnAttackPerformed;
    }
}