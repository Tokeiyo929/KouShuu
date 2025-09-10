using QFramework.Example;
using UnityEngine;

public class ShaoJiTeShu : MonoBehaviour
{
    [SerializeField] private TeaSetInteraction teaSetInteraction;
    private BoxCollider boxCollider;

    void Start()
    {
        // 如果没有手动分配BoxCollider，自动获取
        if (boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        // 确保对象有BoxCollider组件
        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider组件缺失！", this);
        }
    }

    void Update()
    {
        // 检测鼠标点击
        if (Input.GetMouseButtonDown(0)) // 0代表左键
        {
            Camera camera = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();
            // 创建射线从相机到鼠标位置
            if (camera == null)
            {
                return;
            }
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 检测射线是否击中这个BoxCollider
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == boxCollider)
                {
                    OnBoxClicked();
                }
            }
        }
    }

    // 当BoxCollider被点击时调用的函数
    void OnBoxClicked()
    {
        teaSetInteraction.OnTeaSetClick();
    }
}