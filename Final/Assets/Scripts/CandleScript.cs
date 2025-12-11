using UnityEngine;

public class CandleScript : Interactable
{
    [SerializeField] private float floatHeight = 0.5f; // 文字框浮起高度
    [SerializeField] private float floatSpeed = 2f; // 浮起速度
    
    private Vector3 originalPosition;
    private bool isFloating = false;

    [SerializeField] private GameObject candleWithHole;
    [SerializeField] private GameObject candleWithoutHole;
    [SerializeField] private GameObject candleCollider;
    private PlayerData playerData = DataManager.Instance.LoadCheckpoint();
    
    private void Start()
    {
        if (interactionPrompt != null)
        {
            originalPosition = interactionPrompt.transform.position;
            interactionPrompt.SetActive(false);
        }

        if (playerData.hasArrivedEmptyThrone && playerData.hasDeadbyIronVirgin && playerData.hasDeadByTraps
         && playerData.hasInteractedWithTorch && playerData.hasKilledBoss)
        {
            if (!playerData.hasFoundTheCandleHole)
            {
                gameObject.GetComponent<Collider2D>().enabled = true;
                candleCollider.SetActive(true);
                candleWithHole.SetActive(true);
                candleWithoutHole.SetActive(false);
            }
            else
            {
                gameObject.GetComponent<Collider2D>().enabled = true;
                candleCollider.SetActive(true);
                candleWithHole.SetActive(false);
                candleWithoutHole.SetActive(true);
            }
        }
        else
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            candleCollider.SetActive(false);
            candleWithHole.SetActive(false);
            candleWithoutHole.SetActive(true);
        }
    }
    
    private void Update()
    {
        // 控制提示框的浮动动画
        if (isFloating && interactionPrompt != null)
        {
            float newY = originalPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            interactionPrompt.transform.position = new Vector3(
                interactionPrompt.transform.position.x,
                newY,
                interactionPrompt.transform.position.z
            );
        }
    }
    
    public override void Interact()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SetEndingFlag("hasFoundTheCandleHole", true);
        }

        candleWithHole.SetActive(false);
        candleWithoutHole.SetActive(true);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerEnter();
            isFloating = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerExit();
            isFloating = false;
            
            // 重置位置
            if (interactionPrompt != null)
            {
                interactionPrompt.transform.position = originalPosition;
            }
        }
    }
}