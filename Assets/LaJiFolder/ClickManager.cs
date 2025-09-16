using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using HighlightPlus;

public class ClickManager : MonoBehaviour
{
    public List<ClickableObject> clickObjects;   // 不再叫 clickSequence
    private bool isCooldown = false;
    private Camera playerCamera;

    private void Start()
    {
        if (clickObjects == null || clickObjects.Count == 0)
        {
            Debug.LogError("Click objects list is empty!");
            return;
        }

        playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();

        // 激活所有对象（每个都能被点击，但只能点一次）
        foreach (var obj in clickObjects)
        {
            obj.Activate();
        }
    }

    //private void Update()
    //{
    //    if (isCooldown) return;

    //    // 检查是否点击了UI元素
    //    if (EventSystem.current.IsPointerOverGameObject())
    //    {
    //        return;  // 如果点击的是UI，跳过射线检测
    //    }

    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
    //        if (Physics.Raycast(ray, out RaycastHit hit))
    //        {
    //            GameObject clickedObject = hit.collider.gameObject;

    //            // 遍历列表找有没有匹配的 ClickableObject
    //            ClickableObject target = clickObjects.Find(obj => obj.gameObject == clickedObject);

    //            if (target != null && !target.HasBeenClicked)   // 确保没被点过
    //            {
    //                target.TriggerAction();
    //                target.HasBeenClicked = true; // 标记为已点击

    //                // 冷却
    //                isCooldown = true;
    //                StartCoroutine(Cooldown(target.animationDuration));
    //            }
    //            else
    //            {
    //                Debug.Log("点到无效物体，或者该物体已点击过");
    //            }
    //        }
    //    }
    //}

    public void TryClickFromUI(ClickableObject obj)
    {
        if (isCooldown) return;

        if (clickObjects.Contains(obj) && !obj.HasBeenClicked)
        {
            obj.TriggerAction();
            obj.HasBeenClicked = true;

            isCooldown = true;
            StartCoroutine(Cooldown(obj.animationDuration));
        }
        else
        {
            Debug.Log("UI对象不存在，或者已经点过了");
        }
    }

    IEnumerator Cooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isCooldown = false;
    }

    public void AddList(ClickableObject CO)
    {
        if (CO != null)
        {
            CO.Activate();
            clickObjects.Add(CO);
        }
    }
    public void ClearList()
    {
        foreach (var obj in clickObjects)
        {
            if (obj != null)
            {
                obj.HasBeenClicked = false;
            }
        }
        if (clickObjects != null)
        {
            clickObjects.Clear();
        }
    }
    public void ClearListAndSetHighlightOff()
    {
        if (clickObjects == null)
            return;

        foreach (var obj in clickObjects)
        {
            if (obj == null)
                continue;
            //新增关闭点击状态
            obj.HasBeenClicked = false;

            // 优化：只获取一次组件
            var highlight = obj.GetComponent<HighlightEffect>();
            if (highlight != null)
            {
                highlight.enabled = false;
            }
            else
            {
                // 如果没有找到，再尝试在父对象中查找
                var parentHighlight = obj.GetComponentInParent<HighlightEffect>();
                if (parentHighlight != null)
                {
                    parentHighlight.enabled = false;
                }
            }
        }

        clickObjects.Clear();
    }
    private void OnDisable()
    {
        foreach (var obj in clickObjects)
        {
            if (obj != null)
            {
                obj.HasBeenClicked = false;
            }
        }
        if (clickObjects != null)
        {
            clickObjects.Clear();
        }
    }
    private void OnDestroy()
    {
        foreach (var obj in clickObjects)
        {
            if (obj != null)
            {
                obj.HasBeenClicked = false;
            }
        }
        if (clickObjects != null)
        {
            clickObjects.Clear();
        }
    }
    public void RemoveList(ClickableObject CO)
    {
        if (CO != null)
        {
            CO.HasBeenClicked = false;
            clickObjects.Remove(CO);
        }
    }

}
