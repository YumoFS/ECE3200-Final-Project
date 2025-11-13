using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("生命值设置")]
    public int maxHealth = 5;
    public int currentHealth;
    
    [Header("死亡效果")]
    public GameObject deathEffect;
    public bool isDead = false;
    
    void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        
        // 播放受伤动画或效果
        // if (animator != null)
        //     animator.SetTrigger("Hurt");
        
        // 检查是否死亡
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        isDead = true;
        
        // 播放死亡动画
        // if (animator != null)
        //     animator.SetBool("IsDead", true);
        
        // 生成死亡效果
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        
        // 禁用碰撞器和组件
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;
        
        // 销毁敌人（可延迟销毁以播放动画）
        Destroy(gameObject, 2f);
        
        Debug.Log(gameObject.name + " 被击败!");
    }
}