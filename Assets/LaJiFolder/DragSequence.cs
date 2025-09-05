using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class DragSequence : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [System.Serializable]
    public class TargetImageData
    {
        public RectTransform targetImage;   // 目标 Image
        public UnityEvent onDragEvents;     // 立即触发事件
        public UnityEvent onDelayEvents;    // 延迟触发事件
        public float delaySeconds = 2f;     // 延迟时间

        [HideInInspector] public bool hasTriggered = false; // 是否已经触发
    }

    private Vector3 startPos;
    private RectTransform rectTransform;
    private Canvas canvas;

    // 按顺序检查
    public List<TargetImageData> targetImages = new List<TargetImageData>();

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
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
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out var globalMousePos))
            {
                rectTransform.position = globalMousePos;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bool triggered = false;

        foreach (var target in targetImages)
        {
            // 如果已经触发过，跳过
            if (target.hasTriggered) continue;

            if (target.targetImage != null && IsOverlap(target.targetImage))
            {
                Debug.Log($"与 {target.targetImage.name} 重叠了！");
                target.onDragEvents?.Invoke();
                StartCoroutine(DelayInvoke(target));

                target.hasTriggered = true; // 标记为已触发
                triggered = true;
                break; // 只触发第一个
            }
        }

        if (!triggered)
        {
            Debug.Log("没有重叠！");
        }

        rectTransform.position = startPos;
    }

    private IEnumerator DelayInvoke(TargetImageData target)
    {
        yield return new WaitForSeconds(target.delaySeconds);
        Debug.Log($"延迟 {target.delaySeconds} 秒后执行 {target.targetImage.name} 的事件");
        target.onDelayEvents?.Invoke();
    }

    private bool IsOverlap(RectTransform other)
    {
        Vector3[] currentCorners = new Vector3[4];
        Vector3[] targetCorners = new Vector3[4];
        rectTransform.GetWorldCorners(currentCorners);
        other.GetWorldCorners(targetCorners);

        return currentCorners[2].x > targetCorners[0].x &&
               currentCorners[0].x < targetCorners[2].x &&
               currentCorners[2].y > targetCorners[0].y &&
               currentCorners[0].y < targetCorners[2].y;
    }
}
