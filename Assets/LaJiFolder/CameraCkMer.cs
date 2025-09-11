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
            Debug.LogError("[CameraCkMer] ������û�� ClickManager��");
            return;
        }

        if (clickManager.clickObjects == null || clickManager.clickObjects.Count == 0)
        {
            Debug.LogError("[CameraCkMer] Click objects list is�ջ�δ��ʼ����");
            return;
        }

        if (playerCamera == null)
        {
            Debug.LogError("[CameraCkMer] CameraCkMer ������� Camera �����ϣ�");
            return;
        }

        Debug.Log("[CameraCkMer] ��ʼ����ɣ�����������...");

        foreach (var obj in clickManager.clickObjects)
        {
            if (obj != null)
            {
                obj.Activate();
                Debug.Log($"[CameraCkMer] �������: {obj.gameObject.name}");
            }
        }
    }

    private void Update()
    {
        if (isCooldown)
        {
            // Debug����ʾ��ȴ��
            // Debug.Log("[CameraCkMer] ��ȴ�У��޷����");
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.Log("[CameraCkMer] �����UI�ϣ��������߼��");
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                Debug.Log($"[CameraCkMer] ���߻���: {clickedObject.name}");

                ClickableObject target = clickedObject.GetComponentInParent<ClickableObject>();

                if (target != null && clickManager.clickObjects.Contains(target))
                {
                    if (!target.HasBeenClicked)
                    {
                        Debug.Log($"[CameraCkMer] �����Ч����: {target.gameObject.name}");
                        target.TriggerAction();
                        target.HasBeenClicked = true;

                        isCooldown = true;
                        StartCoroutine(Cooldown(target.animationDuration));
                    }
                    else
                    {
                        Debug.Log($"[CameraCkMer] �����ѵ����: {target.gameObject.name}");
                    }
                }
                else
                {
                    Debug.Log("[CameraCkMer] ����������б��л�δ�ҵ� ClickableObject");
                }
            }
            else
            {
                Debug.Log("[CameraCkMer] ����δ�����κ�����");
            }
        }
    }

    public void TryClickFromUI(ClickableObject obj)
    {
        if (isCooldown)
        {
            Debug.Log("[CameraCkMer] ��ȴ�У�UI���������");
            return;
        }

        if (clickManager.clickObjects.Contains(obj))
        {
            if (!obj.HasBeenClicked)
            {
                Debug.Log($"[CameraCkMer] UI�������: {obj.gameObject.name}");
                obj.TriggerAction();
                obj.HasBeenClicked = true;

                isCooldown = true;
                StartCoroutine(Cooldown(obj.animationDuration));
            }
            else
            {
                Debug.Log($"[CameraCkMer] UI�����ѵ����: {obj.gameObject.name}");
            }
        }
        else
        {
            Debug.Log("[CameraCkMer] UI���󲻴������б���");
        }
    }

    IEnumerator Cooldown(float duration)
    {
        Debug.Log($"[CameraCkMer] ��ȴ��ʼ: {duration}s");
        yield return new WaitForSeconds(duration);
        isCooldown = false;
        Debug.Log("[CameraCkMer] ��ȴ�����������ٴε��");
    }

    public void AddList(ClickableObject CO)
    {
        if (CO == null)
        {
            Debug.LogWarning("[CameraCkMer] AddList������null����");
            return;
        }

        if (!clickManager.clickObjects.Contains(CO))
        {
            CO.Activate();
            clickManager.clickObjects.Add(CO);
            Debug.Log($"[CameraCkMer] ����¶���: {CO.gameObject.name}");
        }
        else
        {
            Debug.Log($"[CameraCkMer] �����Ѵ����б���: {CO.gameObject.name}");
        }
    }
}
