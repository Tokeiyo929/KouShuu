using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;

public class CkObjMgr : MonoBehaviour
{
    public List<ClickableObject> clickableObjects;
    public TimelineController tlc;

    // 用于跟踪每个对象的协程
    private Dictionary<ClickableObject, Coroutine> activeInterruptCoroutines = new Dictionary<ClickableObject, Coroutine>();

    // 线程安全的队列用于处理中断请求
    private ConcurrentQueue<System.Action> interruptQueue = new ConcurrentQueue<System.Action>();

    void Start()
    {
        // 可选：初始化代码
    }

    void Update()
    {
        // 在主线程中处理队列中的中断请求
        ProcessInterruptQueue();
    }

    public void AddClickableObject(ClickableObject obj)
    {
        if (clickableObjects == null)
        {
            clickableObjects = new List<ClickableObject>();
        }
        clickableObjects.Add(obj);
    }

    public void InterruptLatestClickObj()
    {
        // 将中断请求加入队列，避免频繁调用的问题
        interruptQueue.Enqueue(() =>
        {
            ExecuteInterrupt();
        });
    }

    private void ProcessInterruptQueue()
    {
        // 每帧处理一个中断请求，避免过于频繁
        if (interruptQueue.TryDequeue(out System.Action action))
        {
            action?.Invoke();
        }
    }

    private void ExecuteInterrupt()
    {
        if (clickableObjects == null || clickableObjects.Count == 0)
            return;

        var latestClickObj = clickableObjects[clickableObjects.Count - 1];

        // 如果对象已经被中断，先停止之前的协程
        if (activeInterruptCoroutines.ContainsKey(latestClickObj))
        {
            StopCoroutine(activeInterruptCoroutines[latestClickObj]);
            activeInterruptCoroutines.Remove(latestClickObj);
        }

        latestClickObj.isInterrupted = true;
        tlc.PauseTimeline();

        // 计算延迟时间，使用更智能的缓冲
        float duration = CalculateInterruptDuration(latestClickObj);

        // 启动协程并记录
        var coroutine = StartCoroutine(ResetInterruptStatus(latestClickObj, duration));
        activeInterruptCoroutines[latestClickObj] = coroutine;
    }

    private float CalculateInterruptDuration(ClickableObject obj)
    {
        // 根据对象类型或状态动态计算缓冲时间
        float baseDuration = Mathf.Max(obj.delayTime, obj.cameraBiasTime);

        // 添加动态缓冲（例如：基础时间的20%，但至少1秒）
        //float buffer = Mathf.Max(baseDuration * 0.2f, 1f);

        return baseDuration; // + buffer;
    }

    private IEnumerator ResetInterruptStatus(ClickableObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);

        // 安全检查：确保对象仍然存在
        if (obj != null)
        {
            obj.isInterrupted = false;
        }

        // 从字典中移除记录
        if (activeInterruptCoroutines.ContainsKey(obj))
        {
            activeInterruptCoroutines.Remove(obj);
        }
    }

    // 清理方法，防止内存泄漏
    public void RemoveClickableObject(ClickableObject obj)
    {
        if (clickableObjects.Contains(obj))
        {
            clickableObjects.Remove(obj);
        }

        if (activeInterruptCoroutines.ContainsKey(obj))
        {
            StopCoroutine(activeInterruptCoroutines[obj]);
            activeInterruptCoroutines.Remove(obj);
        }
    }

    // 当对象销毁时清理资源
    void OnDestroy()
    {
        foreach (var coroutine in activeInterruptCoroutines.Values)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        activeInterruptCoroutines.Clear();
        interruptQueue = new ConcurrentQueue<System.Action>();
    }
}