using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("子弹设置")]
    public float speed = 8f;
    public int damage = 10;
    public float lifeTime = 3f; // 子弹存在时间，避免无限飞行
    
    [Header("视觉效果")]
    public ParticleSystem hitEffect;
    public AudioClip hitSound;
    public TrailRenderer trailRenderer;
    
    private Vector2 direction;
    private Transform target;
    [SerializeField] GameObject player;
    
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        // 自动销毁，避免内存泄漏
        Destroy(gameObject, lifeTime);
        
        // 如果有拖尾效果，在销毁时保留
        if (trailRenderer != null)
        {
            trailRenderer.autodestruct = true;
        }
    }
    
    void Update()
    {
        // 移动子弹
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
    
    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
        if (target != null)
        {
            // 计算朝向玩家的方向
            direction = (target.position - transform.position).normalized;
            
            // 让子弹朝向移动方向
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    
    public void SetDirection(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
        
        // 让子弹朝向移动方向
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // 忽略敌人和子弹自身的碰撞
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyBullet")) 
            return;
            
        // 碰到玩家
        if (other.CompareTag("Player"))
        {
            Player playerComponent = player.GetComponent<Player>();
            if (playerComponent != null)
            {
                playerComponent.playerHitPoint -= damage;
            }
            
            PlayHitEffect(other.ClosestPoint(transform.position));
        }
        // 碰到墙壁或其他障碍物
        // else if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        // {
        //     PlayHitEffect(other.ClosestPoint(transform.position));
        // }
        
        // 销毁子弹
        Destroy(gameObject);
    }
    
    void PlayHitEffect(Vector2 position)
    {
        if (hitEffect != null)
        {
            ParticleSystem effect = Instantiate(hitEffect, position, Quaternion.identity);
            Destroy(effect.gameObject, 2f);
        }
        
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, position);
        }
    }
}