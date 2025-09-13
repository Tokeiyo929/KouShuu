using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;

public class CkObjMgr : MonoBehaviour
{
    public List<ClickableObject> clickableObjects;
    public TimelineController tlc;

    // ���ڸ���ÿ�������Э��
    private Dictionary<ClickableObject, Coroutine> activeInterruptCoroutines = new Dictionary<ClickableObject, Coroutine>();

    // �̰߳�ȫ�Ķ������ڴ����ж�����
    private ConcurrentQueue<System.Action> interruptQueue = new ConcurrentQueue<System.Action>();

    void Start()
    {
        // ��ѡ����ʼ������
    }

    void Update()
    {
        // �����߳��д�������е��ж�����
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
        // ���ж����������У�����Ƶ�����õ�����
        interruptQueue.Enqueue(() =>
        {
            ExecuteInterrupt();
        });
    }

    private void ProcessInterruptQueue()
    {
        // ÿ֡����һ���ж����󣬱������Ƶ��
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

        // ��������Ѿ����жϣ���ֹ֮ͣǰ��Э��
        if (activeInterruptCoroutines.ContainsKey(latestClickObj))
        {
            StopCoroutine(activeInterruptCoroutines[latestClickObj]);
            activeInterruptCoroutines.Remove(latestClickObj);
        }

        latestClickObj.isInterrupted = true;
        tlc.PauseTimeline();

        // �����ӳ�ʱ�䣬ʹ�ø����ܵĻ���
        float duration = CalculateInterruptDuration(latestClickObj);

        // ����Э�̲���¼
        var coroutine = StartCoroutine(ResetInterruptStatus(latestClickObj, duration));
        activeInterruptCoroutines[latestClickObj] = coroutine;
    }

    private float CalculateInterruptDuration(ClickableObject obj)
    {
        // ���ݶ������ͻ�״̬��̬���㻺��ʱ��
        float baseDuration = Mathf.Max(obj.delayTime, obj.cameraBiasTime);

        // ��Ӷ�̬���壨���磺����ʱ���20%��������1�룩
        //float buffer = Mathf.Max(baseDuration * 0.2f, 1f);

        return baseDuration; // + buffer;
    }

    private IEnumerator ResetInterruptStatus(ClickableObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);

        // ��ȫ��飺ȷ��������Ȼ����
        if (obj != null)
        {
            obj.isInterrupted = false;
        }

        // ���ֵ����Ƴ���¼
        if (activeInterruptCoroutines.ContainsKey(obj))
        {
            activeInterruptCoroutines.Remove(obj);
        }
    }

    // ����������ֹ�ڴ�й©
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

    // ����������ʱ������Դ
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