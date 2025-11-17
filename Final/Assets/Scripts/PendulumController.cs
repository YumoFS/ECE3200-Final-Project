using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumController : MonoBehaviour
{
    [Header("摆动参数")]
    [SerializeField] private float maxAngle = 60f;
    [SerializeField] private float swingSpeed = 0.5f;
    [SerializeField] private float startAngle = 0f;
    
    [Header("伤害设置")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float knockbackForce = 15f;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private AudioClip hitSound;
    
    [Header("引用")]
    [SerializeField] private Transform pivotPoint;
    [SerializeField] private Collider2D damageCollider;
    
    private float timer = 0f;
    private float ropeLength;
    
    void Start()
    {
        // 计算绳子长度
        ropeLength = Vector3.Distance(transform.position, pivotPoint.position);
        
        // 设置初始角度
        timer = startAngle / (maxAngle * 2f);
        
        // 设置碰撞器为触发器
        if (damageCollider != null)
        {
            damageCollider.isTrigger = true;
        }
    }
    
    void Update()
    {
        // 更新计时器
        timer += Time.deltaTime * swingSpeed;
        
        // 计算当前角度
        float currentAngle = maxAngle * Mathf.Sin(timer * 2f * Mathf.PI);
        
        // 应用旋转
        ApplyPendulumRotation(currentAngle);
    }
    
    void ApplyPendulumRotation(float angle)
    {
        // 创建旋转
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        
        // 计算相对于悬挂点的位置
        Vector3 direction = rotation * Vector3.down;
        Vector3 newPosition = pivotPoint.position + direction * ropeLength;
        
        // 更新位置和旋转
        transform.position = newPosition;
        transform.rotation = rotation;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DealDamageToPlayer(other.gameObject, other.ClosestPoint(transform.position));
        }
    }
    
    void DealDamageToPlayer(GameObject player, Vector2 contactPoint)
    {
        Player playerComponent = player.GetComponent<Player>();
        // 播放效果
        if (hitEffect != null)
            Instantiate(hitEffect, contactPoint, Quaternion.identity);
            
        if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, contactPoint);
        
        // 击退玩家
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
            playerRb.velocity = Vector2.zero;
            playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
        
        playerComponent.deadReason = "Pendulum";
        playerComponent.isAlive = false;
        playerComponent.playerHitPoint -= 10;
    }
    
    // 在编辑器中可视化
    void OnDrawGizmosSelected()
    {
        if (pivotPoint == null) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pivotPoint.position, 0.1f);
        
        Gizmos.color = Color.red;
        float length = Application.isPlaying ? ropeLength : Vector3.Distance(transform.position, pivotPoint.position);
        
        Vector3 leftDirection = Quaternion.Euler(0, 0, maxAngle) * Vector3.down;
        Vector3 rightDirection = Quaternion.Euler(0, 0, -maxAngle) * Vector3.down;
        
        Gizmos.DrawLine(pivotPoint.position, pivotPoint.position + leftDirection * length);
        Gizmos.DrawLine(pivotPoint.position, pivotPoint.position + rightDirection * length);
    }
}
