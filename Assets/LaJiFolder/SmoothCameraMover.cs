using UnityEngine;

public class SmoothCameraMover : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("移动总时间（秒）")]
    public float moveDuration = 2f;
    [Tooltip("旋转总时间（秒）")]
    public float rotationDuration = 2f;
    [Tooltip("是否在到达目标位置后停止")]
    public bool stopOnArrival = true;
    [Tooltip("到达目标的距离阈值")]
    public float arrivalDistance = 0.1f;
    [Tooltip("到达目标的旋转阈值")]
    public float arrivalAngle = 1f;

    private Transform targetTransform;
    private bool isMoving = false;

    private float moveTime = 0f;
    private float rotationTime = 0f;

    // 新增：记录起始状态
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Update()
    {
        if (isMoving && targetTransform != null)
        {
            // 更新已移动的时间
            moveTime += Time.deltaTime;
            rotationTime += Time.deltaTime;

            // 计算插值比例（0到1之间）
            float moveLerpFactor = Mathf.Clamp01(moveTime / moveDuration);
            float rotationLerpFactor = Mathf.Clamp01(rotationTime / rotationDuration);

            // 使用缓存的起始位置进行插值
            transform.position = Vector3.Lerp(startPosition, targetTransform.position, moveLerpFactor);
            transform.rotation = Quaternion.Slerp(startRotation, targetTransform.rotation, rotationLerpFactor);

            // 检查是否到达目标
            float distance = Vector3.Distance(transform.position, targetTransform.position);
            float angle = Quaternion.Angle(transform.rotation, targetTransform.rotation);

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
        moveTime = 0f;
        rotationTime = 0f;

        // 新增：缓存起始状态
        startPosition = transform.position;
        startRotation = transform.rotation;
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

        // 更新起始状态
        startPosition = transform.position;
        startRotation = transform.rotation;
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

    // 可选：停止移动
    public void StopMovement()
    {
        isMoving = false;
    }
}