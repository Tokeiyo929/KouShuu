using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Unity.VisualScripting;  // ����EventSystems�����UI

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

        // ����Ƿ�����UIԪ��
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;  // ����������UI���������߼��
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

                    // ������ȴ״̬����ֹ�����������������
                    isCooldown = true;
                    StartCoroutine(Cooldown(expected.animationDuration));
                }
                else
                {
                    Debug.Log("���������");
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
            Debug.Log("��� UI ������");
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
            Debug.Log("ȫ��������ɣ�");
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
