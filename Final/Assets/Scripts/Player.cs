using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public bool isAlive;
    public string deadReason;
    private bool _hasTorch;
    public bool hasTorch
    {
        get { return _hasTorch; }
        set
        {
            if (_hasTorch != value)
            {
                _hasTorch = value;
                
                // 自动保存
                if (DataManager.Instance != null)
                {
                    DataManager.Instance.SetEndingFlag("hasInteractedWithTorch", value);
                    SavePlayerDataToDataManager();
                }
            }
        }
    }
    private int _playerHitPoint;
    public int playerHitPoint
    {
        get { return _playerHitPoint; }
        set
        {
            if (_playerHitPoint != value)
            {
                _playerHitPoint = value;
                
                // 生命值变化时自动保存
                if (DataManager.Instance != null && Mathf.Abs(_playerHitPoint - value) > 0)
                {
                    SavePlayerDataToDataManager();
                }
            }
        }
    }
    public int deadCount;
    public int winCount;
    public int currentTime;

    [SerializeField] private GameInputs gameInputs;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private int jumpNumMax = 2;
    [SerializeField] private Vector3 playerInitialPosition;
    
    [Header("动画相关")]
    [SerializeField] private Animator animator;
    
    [Header("交互相关")]
    [SerializeField] private float interactionRange = .3f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("玩家数据相关")]
    public int playerAttackPower = 1;
    [SerializeField] private float playerAttackDistance = 2f;
    [SerializeField] private float attackCooldown = 0.5f;
    public string playerName;
    public int playerHitPointMax = 1;

    [SerializeField] private Transform attackPosition; 
    [SerializeField] private LayerMask enemyLayers;

    [Header("存档点相关")]
    [SerializeField] private Transform currentCheckpoint;
    private string currentSceneName;

    [Header("死亡设置")]
    [SerializeField] private bool useDeathTransitionScene = true;

    private Rigidbody2D rb;
    private int jumpNumCount;
    private bool isInAir;
    private Interactable currentInteractable;
    
    // 动画状态变量
    private bool isAttacking = false;
    private float attackCooldownTimer = 0f;
    private float lastHorizontalInput = 1f; // 默认朝右

    private bool shouldRespawnFromCheckpoint = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = GetComponent<Animator>();
        
        // 初始化属性
        _hasTorch = false;
        _playerHitPoint = playerHitPointMax;
    }

    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        LoadPlayerData();

        SceneManager.sceneLoaded += OnSceneLoaded;

        if (attackPosition == null) attackPosition = transform;
    }

    private void LoadPlayerData()
    {
        if (DataManager.Instance == null)
        {
            Debug.LogWarning("DataManager 未找到，使用默认值初始化");
            InitializeWithDefaultValues();
            ResetPlayerState(); // 确保状态重置
            return;
        }
        
        // 从 DataManager 加载数据
        PlayerData savedData = DataManager.Instance.LoadCheckpoint();
        
        // 检查是否需要从存档点复活
        if (!string.IsNullOrEmpty(savedData.checkpointSceneName) && 
            savedData.checkpointSceneName == currentSceneName)
        {
            // 从存档点复活
            RespawnFromCheckpoint(savedData);
        }
        else
        {
            // 进入新场景，但保留玩家数据
            ApplyPlayerData(savedData);
            
            // 设置位置为场景出生点
            Vector3 spawnPosition = GetSceneSpawnPosition();
            transform.position = spawnPosition;
            playerInitialPosition = spawnPosition;
            
            // 重置状态
            ResetPlayerState();
            
            Debug.Log($"进入新场景，保留玩家数据，出生点: {spawnPosition}");
        }
    }

    private void ApplyPlayerData(PlayerData data)
    {
        // 恢复属性
        playerHitPoint = data.playerHitPoint;
        playerHitPointMax = data.playerHitPointMax;
        hasTorch = data.hasTorch;
        deadCount = data.deadCount;
        winCount = data.winCount;
        playerAttackPower = data.playerAttackPower;
        playerName = data.playerName;
        currentTime = data.currentTime;
        
        // 恢复结局相关属性
        if (DataManager.Instance != null)
        {
            // 这里逐个恢复结局标志，或者可以在PlayerData中添加方法
            // 暂时只恢复基础数据，结局标志在需要时从DataManager查询
        }
    }

    // 使用默认值初始化
    private void InitializeWithDefaultValues()
    {
        isAlive = true;
        playerInitialPosition = transform.position;
        playerHitPoint = playerHitPointMax;
        hasTorch = false;
        deadCount = 0;
        winCount = 0;
        playerAttackPower = 1;
        playerName = "Player";
        currentTime = UnityEngine.Random.Range(1000, 1200);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        currentSceneName = scene.name;
        
        // 确保玩家数据在场景切换后仍然保留
        if (DataManager.Instance != null)
        {
            // 保存当前状态到DataManager
            SavePlayerDataToDataManager();
        }
        
        // 初始化位置
        InitializePlayerPosition();
    }

    public void SavePlayerDataToDataManager()
    {
        if (DataManager.Instance == null) return;
        
        // 使用当前的检查点或玩家位置
        Vector3 savePosition = currentCheckpoint != null ? 
            currentCheckpoint.position : transform.position;
        
        // 保存到DataManager
        DataManager.Instance.SaveCheckpoint(
            savePosition, 
            currentSceneName, 
            this
        );
        
        Debug.Log($"玩家数据已保存到DataManager: {playerName}, HP: {playerHitPoint}");
    }

    private void InitializePlayerPosition()
    {
        // 检查是否需要从存档点复活
        CheckRespawnFromCheckpoint();
        
        // 如果不需要从存档点复活，使用场景出生点或初始位置
        if (!shouldRespawnFromCheckpoint)
        {
            isAlive = true;
            playerHitPoint = playerHitPointMax;
            hasTorch = false;
            
            // 尝试从SceneSpawnManager获取出生点
            if (SceneSpawnManager.Instance != null)
            {
                Vector3 spawnPosition = SceneSpawnManager.Instance.GetSpawnPosition();
                if (spawnPosition != Vector3.zero)
                {
                    transform.position = spawnPosition;
                    playerInitialPosition = spawnPosition;
                    Debug.Log($"从场景出生点复活: {spawnPosition}");
                }
                else
                {
                    // 使用Inspector中设置的初始位置
                    transform.position = playerInitialPosition;
                }
            }
            else
            {
                // 使用Inspector中设置的初始位置
                transform.position = playerInitialPosition;
            }
        }
    }

    private void CheckRespawnFromCheckpoint()
    {
        if (DataManager.Instance == null) return;
        
        PlayerData savedData = DataManager.Instance.LoadCheckpoint();
        
        // 如果存档点场景与当前场景相同，则从存档点复活
        if (!string.IsNullOrEmpty(savedData.checkpointSceneName) && 
            savedData.checkpointSceneName == currentSceneName)
        {
            // 从存档点复活
            RespawnFromCheckpoint(savedData);
        }
        else
        {
            // 场景不同，使用DataManager中的数据来设置属性
            ApplyPlayerData(savedData);
            
            // 设置位置为新场景的出生点
            Vector3 spawnPosition = GetSceneSpawnPosition();
            transform.position = spawnPosition;
            playerInitialPosition = spawnPosition;
            
            isAlive = true;
            
            Debug.Log($"进入新场景，使用DataManager数据，出生点: {spawnPosition}");
        }
    }

    private Vector3 GetSceneSpawnPosition()
    {
        // 尝试从SceneSpawnManager获取出生点
        if (SceneSpawnManager.Instance != null)
        {
            Vector3 spawnPosition = SceneSpawnManager.Instance.GetSpawnPosition();
            if (spawnPosition != Vector3.zero)
            {
                return spawnPosition;
            }
        }
        
        // 否则使用Inspector中设置的初始位置
        return playerInitialPosition;
    }

    private void RespawnFromCheckpoint(PlayerData savedData)
    {
        shouldRespawnFromCheckpoint = true;
        
        // 恢复属性（从存档数据）
        playerHitPoint = savedData.playerHitPoint;
        playerHitPointMax = savedData.playerHitPointMax;
        hasTorch = savedData.hasTorch;
        deadCount = savedData.deadCount;
        winCount = savedData.winCount;
        playerAttackPower = savedData.playerAttackPower;
        currentTime = savedData.currentTime;
        
        // 重要：生成新的随机名字，而不是使用存档中的名字
        if (DataManager.Instance != null)
        {
            // 生成新的随机名字
            playerName = DataManager.Instance.GenerateRandomName();
            DataManager.Instance.SetPlayerName(playerName);
        }
        else
        {
            playerName = "New Adventurer";
        }
        
        // 设置位置
        transform.position = savedData.checkpointPosition;
        
        // 触发复活动画
        if (animator != null)
        {
            animator.SetTrigger("Respawn");
        }
        
        // 重置状态
        ResetPlayerState();
        
        Debug.Log($"从存档点复活 - 新名字: {playerName}, 生命值: {playerHitPoint}, 火炬: {hasTorch}");
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        currentCheckpoint = checkpoint;
        
        // 保存到DataManager
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SaveCheckpoint(
                checkpoint.position, 
                currentSceneName, 
                this
            );
        }
        
        Debug.Log($"存档点已设置: {checkpoint.position}");
    }

    private void Update()
    {
        if (transform.position.y < -5f)
        {
            playerHitPoint = 0;
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
        
        if (playerHitPoint > 0)
        {
            HandleMovement();
            HandleJump();
            HandleAttack();
            CheckForInteractables();
            HandleInteraction();
        }

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

        if (Time.time % 30f < Time.deltaTime && DataManager.Instance != null)
        {
            SavePlayerDataToDataManager();
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
        if (inputVector.x != 0 && playerHitPoint > 0)
        {
            lastHorizontalInput = Mathf.Sign(inputVector.x);
            transform.localScale = new Vector3(lastHorizontalInput * 0.2f, 0.2f, 0.2f);
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
        if (!isAlive) return; // 防止重复触发
        
        isAlive = false;
        deadCount++;
        currentTime += UnityEngine.Random.Range(20, 25);

        if (deadCount >= 4 && DataManager.Instance != null)
        {
            DataManager.Instance.ResetAfterFourDeaths(this);
            // 注意：deadCount在ResetAfterFourDeaths中已经被清零
        }

        // 触发死亡动画
        if (animator != null)
        {
            animator.SetBool("IsByVirgin", deadReason == "IronVirgin");
            animator.SetTrigger("Die");
        }
        
        rb.velocity = Vector2.zero;

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

    private void SetEndingFlag(string flagName)
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SetEndingFlag(flagName, true);
        }
        else
        {
            Debug.LogWarning("DataManager未找到，无法设置结局标志");
        }
    }

    private IEnumerator RespawnAfterDeath()
    {
        // 等待死亡动画播放
        yield return new WaitForSeconds(1.5f);
        
        if (useDeathTransitionScene)
        {
            // 保存当前场景信息（如果需要）
            if (DataManager.Instance != null)
            {
                // 更新死亡次数
                PlayerData data = DataManager.Instance.LoadCheckpoint();
                data.deadCount = deadCount;
                data.currentTime = currentTime;
                
                // 如果需要，可以在这里保存其他数据
                // DataManager.Instance.SaveToFile();
            }
            
            // 加载死亡过渡场景
            DeathTransitionSceneController.LoadDeathTransitionScene(deadReason);
        }
        else
        {
            // 使用原来的逻辑（在当前场景复活）
            StartCoroutine(RespawnInCurrentSceneOld());
        }
    }

    private IEnumerator RespawnInCurrentSceneOld()
    {
        yield return new WaitForSeconds(3f);
        
        if (DataManager.Instance != null)
        {
            PlayerData savedData = DataManager.Instance.LoadCheckpoint();
            
            if (savedData.checkpointSceneName != currentSceneName)
            {
                // 加载存档点场景
                if (SceneTransitionManager.Instance != null)
                {
                    SceneTransitionManager.Instance.LoadSceneWithSave(savedData.checkpointSceneName);
                }
                else
                {
                    SceneManager.LoadScene(savedData.checkpointSceneName);
                }
            }
            else
            {
                // 在当前场景从存档点复活
                RespawnFromCheckpoint(savedData);
            }
        }
        else
        {
            // 没有DataManager，使用旧的复活逻辑
            RespawnInCurrentScene();
        }
    }

    private void RespawnInCurrentScene()
    {
        if (DataManager.Instance != null)
        {
            playerName = DataManager.Instance.GenerateRandomName();
            DataManager.Instance.SetPlayerName(playerName);
        }

        transform.position = playerInitialPosition;
        rb.velocity = Vector3.zero;
        isAlive = true;
        playerHitPoint = playerHitPointMax;

        if (DataManager.Instance != null)
        {
            playerName = DataManager.Instance.GenerateRandomName();
            DataManager.Instance.SetPlayerName(playerName);
        }
        
        // 重置动画状态
        if (animator != null)
        {
            animator.SetTrigger("Respawn");
        }
    }

    public string GetDisplayName()
    {
        return playerName;
    }

    private void DeadByFalling()
    {
        SetEndingFlag("hasDeadByTraps");
    }
    private void DeadByPendulum()
    {
        SetEndingFlag("hasDeadByTraps");
    }
    private void DeadBySpikes()
    {
        SetEndingFlag("hasDeadByTraps");
    }
    private void DeadByIronVirgin()
    {
        SetEndingFlag("hasDeadByIronVirgin");
    }
    
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

    // 新增方法：完全重置玩家状态
    public void ResetPlayerState()
    {
        Debug.Log("重置玩家状态");
        
        // 重置所有与死亡相关的状态
        isAlive = true;
        deadReason = "";
        isAttacking = false;
        attackCooldownTimer = 0f;
        
        // 确保生命值有效
        if (playerHitPoint <= 0)
        {
            playerHitPoint = playerHitPointMax;
        }
        
        // 重置动画状态
        if (animator != null)
        {
            animator.ResetTrigger("Die");
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Respawn");
            animator.SetBool("IsByVirgin", false);
            animator.SetBool("IsAttacking", false);
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsGrounded", true);
            
            // 强制切换到Idle状态
            animator.Play("Idle");
        }
        
        // 重置物理状态
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = false;
        }
        
        Debug.Log($"玩家状态重置完成: 生命值={playerHitPoint}/{playerHitPointMax}, isAlive={isAlive}");
    }

    // 保存当前数据并返回第一个场景
    public void SaveAndReturnToFirstScene()
    {
        Debug.Log("保存数据并返回第一个场景");
        
        // 1. 保存当前玩家数据到DataManager
        SavePlayerDataToDataManager();
        
        // 2. 设置检查点到第一个场景
        SetCheckpointToFirstScene();
        
        // 3. 重置玩家状态，避免死亡循环
        ResetPlayerState();
        
        // 4. 加载第一个场景
        LoadFirstScene();
    }

    // 设置检查点到第一个场景
    private void SetCheckpointToFirstScene()
    {
        // 获取第一个场景的名称（根据您的游戏设定）
        string firstSceneName = "CastleOutside"; // 您可以根据需要修改
        
        // 获取第一个场景的出生点位置
        Vector3 firstSceneSpawnPosition = GetFirstSceneSpawnPosition(firstSceneName);
        
        // 更新当前场景名为第一个场景
        currentSceneName = firstSceneName;
        
        // 保存到DataManager
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SaveCheckpoint(
                firstSceneSpawnPosition,
                firstSceneName,
                this
            );
            Debug.Log($"检查点已设置为: {firstSceneName}, 位置: {firstSceneSpawnPosition}");
        }
        else
        {
            Debug.LogError("DataManager 实例为空，无法保存检查点");
        }
    }

    // 获取第一个场景的出生点位置
    private Vector3 GetFirstSceneSpawnPosition(string sceneName)
    {
        // 优先从SceneSpawnManager获取
        if (SceneSpawnManager.Instance != null)
        {
            // 如果有按场景名获取出生点的方法
            if (HasMethod(SceneSpawnManager.Instance, "GetSpawnPositionForScene"))
            {
                System.Reflection.MethodInfo method = SceneSpawnManager.Instance.GetType().GetMethod("GetSpawnPositionForScene");
                if (method != null)
                {
                    return (Vector3)method.Invoke(SceneSpawnManager.Instance, new object[] { sceneName });
                }
            }
            
            // 否则使用当前方法
            Vector3 spawnPos = SceneSpawnManager.Instance.GetSpawnPosition();
            if (spawnPos != Vector3.zero)
            {
                return spawnPos;
            }
        }
        
        // 默认出生点（根据您的第一个场景调整）
        return new Vector3(0f, 1f, 0f);
    }

    // 加载第一个场景
    private void LoadFirstScene()
    {
        string firstSceneName = "CastleOutside"; // 与上面保持一致
        
        Debug.Log($"正在加载第一个场景: {firstSceneName}");
        
        // 如果有场景过渡管理器，使用它
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadSceneWithSave(firstSceneName);
        }
        else
        {
            // 直接加载场景
            UnityEngine.SceneManagement.SceneManager.LoadScene(firstSceneName);
        }
        
        // 重置玩家状态（场景加载后会再次调用）
        ResetPlayerState();
    }

    // 辅助方法：检查对象是否有某个方法
    private bool HasMethod(object objectToCheck, string methodName)
    {
        var type = objectToCheck.GetType();
        return type.GetMethod(methodName) != null;
    }
}