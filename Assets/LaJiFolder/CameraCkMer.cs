using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CameraCkMer : MonoBehaviour
{
    private bool isCooldown = false;
    [SerializeField]
    private Camera playerCamera;
    private ClickManager clickManager;

    private void Start()
    {
        clickManager = GameObject.FindObjectOfType<ClickManager>();
        if (clickManager == null)
        {
            Debug.LogError("[CameraCkMer] 场景中没有 ClickManager！");
            return;
        }

        if (clickManager.clickObjects == null || clickManager.clickObjects.Count == 0)
        {
            Debug.LogError("[CameraCkMer] Click objects list is空或未初始化！");
            return;
        }

        if (playerCamera == null)
        {
            Debug.LogError("[CameraCkMer] CameraCkMer 必须挂在 Camera 对象上！");
            return;
        }

        Debug.Log("[CameraCkMer] 初始化完成，激活点击对象...");

        foreach (var obj in clickManager.clickObjects)
        {
            if (obj != null)
            {
                obj.Activate();
                Debug.Log($"[CameraCkMer] 激活对象: {obj.gameObject.name}");
            }
        }
    }

    private void Update()
    {
        if (isCooldown)
        {
            // Debug：显示冷却中
            // Debug.Log("[CameraCkMer] 冷却中，无法点击");
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.Log("[CameraCkMer] 鼠标在UI上，跳过射线检测");
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                Debug.Log($"[CameraCkMer] 射线击中: {clickedObject.name}");

                ClickableObject target = clickedObject.GetComponentInParent<ClickableObject>();

                if (target != null && clickManager.clickObjects.Contains(target))
                {
                    if (!target.HasBeenClicked)
                    {
                        Debug.Log($"[CameraCkMer] 点击有效对象: {target.gameObject.name}");
                        target.TriggerAction();
                        target.HasBeenClicked = true;

                        isCooldown = true;
                        StartCoroutine(Cooldown(target.animationDuration));
                    }
                    else
                    {
                        Debug.Log($"[CameraCkMer] 对象已点击过: {target.gameObject.name}");
                    }
                }
                else
                {
                    Debug.Log("[CameraCkMer] 点击对象不在列表中或未找到 ClickableObject");
                }
            }
            else
            {
                Debug.Log("[CameraCkMer] 射线未击中任何物体");
            }
        }
    }

    public void TryClickFromUI(ClickableObject obj)
    {
        if (isCooldown)
        {
            Debug.Log("[CameraCkMer] 冷却中，UI点击被忽略");
            return;
        }

        if (clickManager.clickObjects.Contains(obj))
        {
            if (!obj.HasBeenClicked)
            {
                Debug.Log($"[CameraCkMer] UI点击对象: {obj.gameObject.name}");
                obj.TriggerAction();
                obj.HasBeenClicked = true;

                isCooldown = true;
                StartCoroutine(Cooldown(obj.animationDuration));
            }
            else
            {
                Debug.Log($"[CameraCkMer] UI对象已点击过: {obj.gameObject.name}");
            }
        }
        else
        {
            Debug.Log("[CameraCkMer] UI对象不存在于列表中");
        }
    }

    IEnumerator Cooldown(float duration)
    {
        Debug.Log($"[CameraCkMer] 冷却开始: {duration}s");
        yield return new WaitForSeconds(duration);
        isCooldown = false;
        Debug.Log("[CameraCkMer] 冷却结束，可以再次点击");
    }

    public void AddList(ClickableObject CO)
    {
        if (CO == null)
        {
            Debug.LogWarning("[CameraCkMer] AddList传入了null对象");
            return;
        }

        if (!clickManager.clickObjects.Contains(CO))
        {
            CO.Activate();
            clickManager.clickObjects.Add(CO);
            Debug.Log($"[CameraCkMer] 添加新对象: {CO.gameObject.name}");
        }
        else
        {
            Debug.Log($"[CameraCkMer] 对象已存在列表中: {CO.gameObject.name}");
        }
    }
}
