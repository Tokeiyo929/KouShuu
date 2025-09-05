using UnityEngine;

public class SmoothCameraMover : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("移动速度")]
    public float moveSpeed = 5f;
    [Tooltip("旋转速度")]
    public float rotationSpeed = 5f;
    [Tooltip("是否在到达目标位置后停止")]
    public bool stopOnArrival = true;
    [Tooltip("到达目标的距离阈值")]
    public float arrivalDistance = 0.1f;
    [Tooltip("到达目标的旋转阈值")]
    public float arrivalAngle = 1f;

    private Transform targetTransform;
    private bool isMoving = false;

    void Update()
    {
        if (isMoving && targetTransform != null)
        {
            // 计算接近目标时的插值比例
            float distance = Vector3.Distance(transform.position, targetTransform.position);
            float angle = Quaternion.Angle(transform.rotation, targetTransform.rotation);

            // 动态调整移动和旋转的插值量，接近目标时更小
            float moveLerpFactor = Mathf.Clamp01(distance / arrivalDistance);
            float rotationLerpFactor = Mathf.Clamp01(angle / arrivalAngle);

            // 平滑移动位置
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, moveLerpFactor * moveSpeed * Time.deltaTime);

            // 平滑旋转
            transform.rotation = Quaternion.Slerp(transform.rotation, targetTransform.rotation, rotationLerpFactor * rotationSpeed * Time.deltaTime);

            // 检查是否到达目标
            if (stopOnArrival && HasArrived(distance, angle))
            {
                isMoving = false;
                // 确保完全匹配目标位置和旋转
                transform.position = targetTransform.position;
                transform.rotation = targetTransform.rotation;
            }
        }
    }

    // 设置目标Transform并开始移动
    public void MoveToTransform(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("目标Transform为null!");
            return;
        }

        targetTransform = target;
        isMoving = true;
    }

    // 立即跳转到目标Transform
    public void SnapToTransform(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("目标Transform为null!");
            return;
        }

        targetTransform = target;
        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
        isMoving = false;
    }

    // 检查是否到达目标
    private bool HasArrived(float distance, float angle)
    {
        return distance <= arrivalDistance && angle <= arrivalAngle;
    }

    // 检查是否正在移动
    public bool IsMoving()
    {
        return isMoving;
    }
}
