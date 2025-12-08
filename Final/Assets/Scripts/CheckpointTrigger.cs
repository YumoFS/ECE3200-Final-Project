// CheckpointTrigger.cs
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [Header("存档点设置")]
    [SerializeField] private Transform respawnPosition;
    [SerializeField] private GameObject activatedEffect;
    [SerializeField] private AudioClip activationSound;
    
    private bool isActivated = false;
    
    private void Start()
    {
        if (respawnPosition == null)
            respawnPosition = transform;
        
        if (activatedEffect != null)
            activatedEffect.SetActive(false);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated) return;
        
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            // 激活存档点
            isActivated = true;
            player.SetCheckpoint(respawnPosition);
            
            // 播放激活效果
            if (activatedEffect != null)
                activatedEffect.SetActive(true);
            
            if (activationSound != null)
                AudioSource.PlayClipAtPoint(activationSound, transform.position);
            
            Debug.Log($"存档点已激活: {gameObject.name}");
        }
    }
}