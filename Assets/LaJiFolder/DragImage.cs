using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class DragImage : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 startPos;  // ��ק��ʼʱ��¼����ʼλ��
    private RectTransform rectTransform;
    private Canvas canvas;
    public UnityEvent onDragEvents;
    public UnityEvent onDelayEvents;

    // �ӳ����������� Inspector �����ã�
    [SerializeField] private float delaySeconds = 2f;

    // Ŀ�� Image�����ڼ���Ƿ��ص�
    [SerializeField] private RectTransform targetImage;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // ��¼��ק��ʼʱ��λ��
        startPos = rectTransform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas.renderMode == RenderMode.WorldSpace)
        {
            rectTransform.position = eventData.position;
        }
        else
        {
            Vector3 globalMousePos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos);
            rectTransform.position = globalMousePos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // �ж���קͼƬ�Ƿ���Ŀ��ͼƬ�ص�
        if (IsOverlap(targetImage))
        {
            Debug.Log("�ص��ˣ�");
            onDragEvents?.Invoke();

            // �ӳ�ִ���¼�
            StartCoroutine(DelayInvoke());
        }
        else
        {
            Debug.Log("û���ص���");
        }

        // �ָ�ͼƬλ��
        rectTransform.position = startPos;
    }

    private IEnumerator DelayInvoke()
    {
        yield return new WaitForSeconds(delaySeconds);
        Debug.Log($"�ӳ� {delaySeconds} ���ִ��");
        onDelayEvents?.Invoke();
    }

    // �ж����� RectTransform �Ƿ��ص�
    private bool IsOverlap(RectTransform other)
    {
        Vector3[] currentCorners = new Vector3[4];
        Vector3[] targetCorners = new Vector3[4];
        rectTransform.GetWorldCorners(currentCorners);
        other.GetWorldCorners(targetCorners);

        if (currentCorners[2].x > targetCorners[0].x && currentCorners[0].x < targetCorners[2].x &&
            currentCorners[2].y > targetCorners[0].y && currentCorners[0].y < targetCorners[2].y)
        {
            return true;
        }

        return false;
    }
}
