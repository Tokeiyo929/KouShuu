using QFramework.Example;
using UnityEngine;

public class ShaoJiTeShu : MonoBehaviour
{
    [SerializeField] private TeaSetInteraction teaSetInteraction;
    private BoxCollider boxCollider;

    void Start()
    {
        // ���û���ֶ�����BoxCollider���Զ���ȡ
        if (boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        // ȷ��������BoxCollider���
        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider���ȱʧ��", this);
        }
    }

    void Update()
    {
        // ��������
        if (Input.GetMouseButtonDown(0)) // 0�������
        {
            Camera camera = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();
            // �������ߴ���������λ��
            if (camera == null)
            {
                return;
            }
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ��������Ƿ�������BoxCollider
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == boxCollider)
                {
                    OnBoxClicked();
                }
            }
        }
    }

    // ��BoxCollider�����ʱ���õĺ���
    void OnBoxClicked()
    {
        teaSetInteraction.OnTeaSetClick();
    }
}