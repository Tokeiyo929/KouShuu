using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class DragImage : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 startPos;  // 拖拽开始时记录的起始位置
    private RectTransform rectTransform;
    private Canvas canvas;
    public UnityEvent onDragEvents;
    public UnityEvent onDelayEvents;

    // 延迟秒数（可在 Inspector 中设置）
    [SerializeField] private float delaySeconds = 2f;

    // 目标 Image，用于检查是否重叠
    [SerializeField] private RectTransform targetImage;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 记录拖拽开始时的位置
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
        // 判断拖拽图片是否与目标图片重叠
        if (IsOverlap(targetImage))
        {
            Debug.Log("重叠了！");
            onDragEvents?.Invoke();

            // 延迟执行事件
            StartCoroutine(DelayInvoke());
        }
        else
        {
            Debug.Log("没有重叠！");
        }

        // 恢复图片位置
        rectTransform.position = startPos;
    }

    private IEnumerator DelayInvoke()
    {
        yield return new WaitForSeconds(delaySeconds);
        Debug.Log($"延迟 {delaySeconds} 秒后执行");
        onDelayEvents?.Invoke();
    }

    // 判断两个 RectTransform 是否重叠
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
