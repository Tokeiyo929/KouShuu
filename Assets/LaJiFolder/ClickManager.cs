using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Unity.VisualScripting;  // 引入EventSystems来检测UI

public class ClickManager : MonoBehaviour
{
    public List<ClickableObject> clickSequence;

    private int currentIndex = 0;
    private bool isCooldown = false;
    private Camera playerCamera;
    private bool isStart = true;

    private void Start()
    {
        if (clickSequence == null || clickSequence.Count == 0)
        {
            Debug.LogError("Click sequence is empty!");
            return;
        }

        ActivateCurrent();
        playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();
    }

    private void Update()
    {

        if (isCooldown) return;

        // 检查是否点击了UI元素
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;  // 如果点击的是UI，跳过射线检测
        }

        if (Input.GetMouseButtonDown(0))
        {
            
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                ClickableObject expected = clickSequence[currentIndex];

                if (clickedObject == expected.gameObject)
                {
                    expected.TriggerAction();

                    // 进入冷却状态，防止动画过程中连续点击
                    isCooldown = true;
                    StartCoroutine(Cooldown(expected.animationDuration));
                }
                else
                {
                    Debug.Log("点错物体了");
                }
            }
        }
    }

    public void TryClickFromUI(ClickableObject obj)
    {
        if (isCooldown) return;

        ClickableObject expected = clickSequence[currentIndex];
        if (obj == expected)
        {
            obj.TriggerAction();
            isCooldown = true;
            StartCoroutine(Cooldown(obj.animationDuration));
        }
        else
        {
            Debug.Log("点错 UI 对象了");
        }
    }

    IEnumerator Cooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isCooldown = false;
    }

    void ActivateCurrent()
    {
        if (currentIndex < clickSequence.Count)
        {
            var obj = clickSequence[currentIndex];
            obj.OnClickCompleted += OnCurrentClickCompleted;
            obj.Activate();
        }
        else
        {
            Debug.Log("全部流程完成！");
        }
    }

    void OnCurrentClickCompleted(ClickableObject obj)
    {
        obj.OnClickCompleted -= OnCurrentClickCompleted;
        currentIndex++;
        ActivateCurrent();
    }
    public void AddList(ClickableObject CO)
    {
        if(CO != null)
            clickSequence.Add(CO);
    }
}
