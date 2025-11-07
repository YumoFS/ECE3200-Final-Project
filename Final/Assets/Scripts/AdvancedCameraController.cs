using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedCameraController : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;
    
    [Header("跟随设置")]
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);
    
    [Header("边界限制")]
    public bool useBounds = false;
    public float minX = -10f, maxX = 10f, minY = -5f, maxY = 5f;
    
    [Header("预测跟随")]
    public bool usePrediction = false;
    public float predictionStrength = 0.5f;
    
    private Vector3 targetPreviousPosition;
    private Vector3 targetVelocity;
    
    void Start()
    {
        if (target != null)
        {
            targetPreviousPosition = target.position;
            // 初始化相机位置
            transform.position = target.position + offset;
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        Vector3 desiredPosition = CalculateDesiredPosition();
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
    
    Vector3 CalculateDesiredPosition()
    {
        Vector3 basePosition = target.position + offset;
        
        // 应用预测
        if (usePrediction)
        {
            targetVelocity = (target.position - targetPreviousPosition) / Time.deltaTime;
            targetPreviousPosition = target.position;
            basePosition += targetVelocity * predictionStrength;
        }
        
        // 应用边界
        if (useBounds)
        {
            basePosition.x = Mathf.Clamp(basePosition.x, minX, maxX);
            basePosition.y = Mathf.Clamp(basePosition.y, minY, maxY);
        }
        
        return basePosition;
    }
    
    // 在Scene视图中绘制边界（仅编辑器）
    #if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (useBounds)
        {
            Gizmos.color = Color.green;
            Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0);
            Vector3 size = new Vector3(maxX - minX, maxY - minY, 0.1f);
            Gizmos.DrawWireCube(center, size);
        }
    }
    #endif
}