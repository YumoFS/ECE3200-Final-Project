using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("发射设置")]
    public GameObject bulletPrefab;
    public Transform firePoint;          // 子弹生成位置
    public float fireRate = 1f;          // 每秒发射次数
    public float bulletSpeed = 8f;       // 子弹速度
    public int bulletsPerShot = 1;       // 每次发射的子弹数量
    
    [Header("瞄准设置")]
    public bool aimAtPlayer = true;      // 是否瞄准玩家
    public float aimOffset = 0f;         // 瞄准偏移（用于散射）
    public float detectionRange = 10f;   // 检测玩家范围
    
    [Header("高级设置")]
    public bool burstFire = false;       // 是否连发模式
    public int burstCount = 3;           // 连发数量
    public float burstDelay = 0.1f;      // 连发间隔
    
    private Transform player;
    private float fireTimer = 0f;
    private bool canShoot = true;
    
    void Start()
    {
        // 查找玩家
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        // 如果没有指定发射点，使用自身位置
        if (firePoint == null)
            firePoint = transform;
    }
    
    void Update()
    {
        if (player == null) 
        {
            FindPlayer();
            return;
        }
        
        // 检查玩家是否在范围内
        if (IsPlayerInRange())
        {
            // 更新射击计时器
            fireTimer += Time.deltaTime;
            
            // 检查是否可以射击
            if (fireTimer >= 1f / fireRate && canShoot)
            {
                if (burstFire)
                {
                    StartCoroutine(BurstFire());
                }
                else
                {
                    Shoot();
                }
                
                fireTimer = 0f;
            }
        }
        else
        {
            // 玩家不在范围内时重置计时器
            fireTimer = 0f;
        }
    }
    
    bool IsPlayerInRange()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        return distanceToPlayer <= detectionRange;
    }
    
    void Shoot()
    {
        for (int i = 0; i < bulletsPerShot; i++)
        {
            // 创建子弹
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
            
            if (bulletScript != null)
            {
                // 设置子弹速度
                bulletScript.speed = bulletSpeed;
                
                if (aimAtPlayer)
                {
                    // 计算朝向玩家的方向（带偏移）
                    Vector2 directionToPlayer = (player.position - firePoint.position).normalized;
                    
                    // 应用随机偏移（如果有）
                    if (aimOffset > 0f)
                    {
                        float randomOffset = Random.Range(-aimOffset, aimOffset);
                        directionToPlayer = Quaternion.Euler(0, 0, randomOffset) * directionToPlayer;
                    }
                    
                    bulletScript.SetDirection(directionToPlayer);
                }
                else
                {
                    // 使用发射点的方向
                    bulletScript.SetDirection(firePoint.right);
                }
            }
        }
        
        // 播放射击音效/特效
        PlayShootEffects();
    }
    
    IEnumerator BurstFire()
    {
        canShoot = false;
        
        for (int i = 0; i < burstCount; i++)
        {
            Shoot();
            yield return new WaitForSeconds(burstDelay);
        }
        
        canShoot = true;
    }
    
    void PlayShootEffects()
    {
        // 在这里添加射击特效、音效等
        // 例如：
        // AudioSource.PlayClipAtPoint(shootSound, transform.position);
        // Instantiate(muzzleFlash, firePoint.position, firePoint.rotation);
    }
    
    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }
    
    // 在编辑器中可视化检测范围
    void OnDrawGizmosSelected()
    {
        // 绘制检测范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // 绘制发射方向
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(firePoint.position, firePoint.right * 2f);
        }
    }
}