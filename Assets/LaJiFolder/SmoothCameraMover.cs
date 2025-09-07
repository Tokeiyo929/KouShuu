using UnityEngine;

public class SmoothCameraMover : MonoBehaviour
{
    [Header("�ƶ�����")]
    [Tooltip("�ƶ���ʱ�䣨�룩")]
    public float moveDuration = 2f;
    [Tooltip("��ת��ʱ�䣨�룩")]
    public float rotationDuration = 2f;
    [Tooltip("�Ƿ��ڵ���Ŀ��λ�ú�ֹͣ")]
    public bool stopOnArrival = true;
    [Tooltip("����Ŀ��ľ�����ֵ")]
    public float arrivalDistance = 0.1f;
    [Tooltip("����Ŀ�����ת��ֵ")]
    public float arrivalAngle = 1f;

    private Transform targetTransform;
    private bool isMoving = false;

    private float moveTime = 0f;
    private float rotationTime = 0f;

    // ��������¼��ʼ״̬
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Update()
    {
        if (isMoving && targetTransform != null)
        {
            // �������ƶ���ʱ��
            moveTime += Time.deltaTime;
            rotationTime += Time.deltaTime;

            // �����ֵ������0��1֮�䣩
            float moveLerpFactor = Mathf.Clamp01(moveTime / moveDuration);
            float rotationLerpFactor = Mathf.Clamp01(rotationTime / rotationDuration);

            // ʹ�û������ʼλ�ý��в�ֵ
            transform.position = Vector3.Lerp(startPosition, targetTransform.position, moveLerpFactor);
            transform.rotation = Quaternion.Slerp(startRotation, targetTransform.rotation, rotationLerpFactor);

            // ����Ƿ񵽴�Ŀ��
            float distance = Vector3.Distance(transform.position, targetTransform.position);
            float angle = Quaternion.Angle(transform.rotation, targetTransform.rotation);

            if (stopOnArrival && HasArrived(distance, angle))
            {
                isMoving = false;
                // ȷ����ȫƥ��Ŀ��λ�ú���ת
                transform.position = targetTransform.position;
                transform.rotation = targetTransform.rotation;
            }
        }
    }

    // ����Ŀ��Transform����ʼ�ƶ�
    public void MoveToTransform(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("Ŀ��TransformΪnull!");
            return;
        }

        targetTransform = target;
        isMoving = true;
        moveTime = 0f;
        rotationTime = 0f;

        // ������������ʼ״̬
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    // ������ת��Ŀ��Transform
    public void SnapToTransform(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("Ŀ��TransformΪnull!");
            return;
        }

        targetTransform = target;
        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
        isMoving = false;

        // ������ʼ״̬
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    // ����Ƿ񵽴�Ŀ��
    private bool HasArrived(float distance, float angle)
    {
        return distance <= arrivalDistance && angle <= arrivalAngle;
    }

    // ����Ƿ������ƶ�
    public bool IsMoving()
    {
        return isMoving;
    }

    // ��ѡ��ֹͣ�ƶ�
    public void StopMovement()
    {
        isMoving = false;
    }
}