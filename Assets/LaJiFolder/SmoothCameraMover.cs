using UnityEngine;

public class SmoothCameraMover : MonoBehaviour
{
    [Header("�ƶ�����")]
    [Tooltip("�ƶ��ٶ�")]
    public float moveSpeed = 5f;
    [Tooltip("��ת�ٶ�")]
    public float rotationSpeed = 5f;
    [Tooltip("�Ƿ��ڵ���Ŀ��λ�ú�ֹͣ")]
    public bool stopOnArrival = true;
    [Tooltip("����Ŀ��ľ�����ֵ")]
    public float arrivalDistance = 0.1f;
    [Tooltip("����Ŀ�����ת��ֵ")]
    public float arrivalAngle = 1f;

    private Transform targetTransform;
    private bool isMoving = false;

    void Update()
    {
        if (isMoving && targetTransform != null)
        {
            // ����ӽ�Ŀ��ʱ�Ĳ�ֵ����
            float distance = Vector3.Distance(transform.position, targetTransform.position);
            float angle = Quaternion.Angle(transform.rotation, targetTransform.rotation);

            // ��̬�����ƶ�����ת�Ĳ�ֵ�����ӽ�Ŀ��ʱ��С
            float moveLerpFactor = Mathf.Clamp01(distance / arrivalDistance);
            float rotationLerpFactor = Mathf.Clamp01(angle / arrivalAngle);

            // ƽ���ƶ�λ��
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, moveLerpFactor * moveSpeed * Time.deltaTime);

            // ƽ����ת
            transform.rotation = Quaternion.Slerp(transform.rotation, targetTransform.rotation, rotationLerpFactor * rotationSpeed * Time.deltaTime);

            // ����Ƿ񵽴�Ŀ��
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
}
