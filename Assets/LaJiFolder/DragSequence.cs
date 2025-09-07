using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class DragSequence : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public float shrinkFactor = 1f;

    [System.Serializable]
    public class TargetImageData
    {
        public RectTransform targetImage;       // 目标 Image
        public UnityEvent onDragEvents;         // 立即触发事件
        public UnityEvent onDelayEvents;        // 延迟触发事件
        public float delaySeconds = 2f;         // 延迟时间

        [Header("相机修正")]
        public bool isBias = false;             // 是否修正相机
        public float cameraBiasTime = 0f;       // 相机修正时间
        public UnityEvent onBiasCameraEvents;   // 相机修正事件

        [HideInInspector] public bool hasTriggered = false; // 是否已经触发
    }

    private Vector3 startPos;
    private RectTransform rectTransform;
    private Canvas canvas;

    // 按顺序检查
    public List<TargetImageData> targetImages = new List<TargetImageData>();
    private int currentIndex = 0; // 当前要触发的索引

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
        // 拖拽跟随逻辑
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

        // 检查当前索引的目标
        if (currentIndex < targetImages.Count)
        {
            var target = targetImages[currentIndex];

            if (!target.hasTriggered && target.targetImage != null && IsOverlap(target.targetImage))
            {
                Debug.Log($"与 {target.targetImage.name} 重叠了！（顺序触发）");

                // 立即触发
                target.onDragEvents?.Invoke();

                // 延迟触发
                StartCoroutine(DelayInvoke(target));

                // 相机修正
                if (target.isBias)
                {
                    StartCoroutine(FixedBiasCamera(target));
                }

                // 标记为已触发，进入下一个
                target.hasTriggered = true;
                currentIndex++;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.position = startPos;  // 拖拽结束恢复初始位置
    }

    private IEnumerator DelayInvoke(TargetImageData target)
    {
        yield return new WaitForSeconds(target.delaySeconds);
        Debug.Log($"延迟 {target.delaySeconds} 秒后执行 {target.targetImage.name} 的事件");
        target.onDelayEvents?.Invoke();
    }

    private IEnumerator FixedBiasCamera(TargetImageData target)
    {
        yield return new WaitForSeconds(target.cameraBiasTime);
        target.onBiasCameraEvents?.Invoke();
    }

    private bool IsOverlap(RectTransform other)
    {
        Vector3[] currentCorners = new Vector3[4];
        Vector3[] targetCorners = new Vector3[4];
        rectTransform.GetWorldCorners(currentCorners);
        other.GetWorldCorners(targetCorners);

        // 缩小判定区域
        Vector3 center = (currentCorners[0] + currentCorners[2]) / 2;
        for (int i = 0; i < 4; i++)
        {
            currentCorners[i] = center + (currentCorners[i] - center) * shrinkFactor;
        }

        // 判断是否重叠
        return currentCorners[2].x > targetCorners[0].x &&
               currentCorners[0].x < targetCorners[2].x &&
               currentCorners[2].y > targetCorners[0].y &&
               currentCorners[0].y < targetCorners[2].y;
    }
}
