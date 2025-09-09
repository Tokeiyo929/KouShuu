using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
    public class TeaSetInteraction : MonoBehaviour
    {
        // 添加静态变量来跟踪全局检视状态
        private static bool isAnyObjectInspecting = false;
        private static TeaSetInteraction currentInspectingObject = null;

        private bool isInspecting = false;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Transform originalParent;
        private Transform inspectionArea;
        
        // 存储原始组件引用
        private SkinnedMeshRenderer originalSkinnedMeshRenderer;
        private MeshFilter originalMeshFilter;
        private MeshRenderer originalMeshRenderer;
        private Mesh originalMesh;
        private Material[] originalMaterials;

        [System.Serializable]
        public class PanelDataList
        {
            public List<PanelData> panels;
        }

        [System.Serializable]
        public class PanelData
        {
            public string title;
            public string content;
        }

        private void Start()
        {
           
        }

        private void OnEnable()
        {
            // 创建独立的检视区域，而不是修改父对象
            CreateInspectionArea();
            
            // 确保物体有必要的组件
            EnsureRequiredComponents();
            
            // 保存原始组件引用
            SaveOriginalComponents();
            
            Debug.Log($"TeaSetInteraction initialized on {gameObject.name}");
        }

        //组件禁用时，终止检视
        private void OnDisable()
        {
            if(isInspecting)
            {
                ReturnToOriginalPosition();

                //删除检视区域
                Destroy(inspectionArea.gameObject);
            }
            // 禁用鼠标事件响应
            enabled = false;
        }
        
        private void SaveOriginalComponents()
        {
            // 保存原始组件引用
            originalSkinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            originalMeshFilter = GetComponent<MeshFilter>();
            originalMeshRenderer = GetComponent<MeshRenderer>();
            
            if (originalSkinnedMeshRenderer != null)
            {
                originalMesh = originalSkinnedMeshRenderer.sharedMesh;
                originalMaterials = originalSkinnedMeshRenderer.materials;
                Debug.Log($"保存了原始SkinnedMeshRenderer: {originalSkinnedMeshRenderer.name}");
            }
        }
        
        private void CreateInspectionArea()
        {
            // 创建一个独立的检视区域对象
            GameObject inspectionAreaObj = new GameObject($"{gameObject.name}_InspectionArea");
            inspectionArea = inspectionAreaObj.transform;
            
            // 将检视区域放置在茶具原位置的上方，使用世界坐标
            Vector3 inspectionPosition = transform.position + Vector3.up * 0.15f;
            inspectionArea.position = inspectionPosition;
            
            // 确保检视区域的位置在后续操作中保持稳定
            inspectionAreaObj.name = $"{gameObject.name}_InspectionArea_Stable";
            inspectionAreaObj.transform.position=new Vector3(2.33f,-5.17f,0.56f);

            switch (inspectionAreaObj.name) 
            {
                case "泡茶-水盆_InspectionArea_Stable":
                    inspectionAreaObj.transform.position=new Vector3(2.25f,-5.27f,0.47f);
                    break;
                case "泡茶-烧水壶_InspectionArea_Stable":
                    inspectionAreaObj.transform.position=new Vector3(2.23f,-5.17f,0.49f);
                    break;
                case "泡茶-水勺_InspectionArea_Stable":
                    inspectionAreaObj.transform.position=new Vector3(2.32f,-0.85f,0.56f);
                    break;
            }


            Debug.Log($"Created inspection area at position: {inspectionArea.position}");
            Debug.Log($"Original tea set position: {transform.position}");
            Debug.Log($"Inspection area world position: {inspectionArea.position}");
        }
        
        private void EnsureRequiredComponents()
        {
            // 确保有Collider用于射线检测
            if (GetComponent<Collider>() == null)
            {
                Debug.LogWarning($"{gameObject.name} 缺少Collider组件，添加BoxCollider");
                gameObject.AddComponent<BoxCollider>();
            }
        }
        
        private void Update()
        {
            HandleMouseDrag();
        }
        
        /// <summary>
        /// 处理鼠标点击事件
        /// </summary>
        private void OnTeaSetClick()
        {
            // 检查是否有其他物体正在检视
            if (isAnyObjectInspecting && currentInspectingObject != null && currentInspectingObject != this)
            {
                Debug.Log($"无法检视 {gameObject.name}，因为 {currentInspectingObject.gameObject.name} 正在检视中");
                return;
            }
            
            if (isInspecting)
            {
                SetPanelActive(false);
                originalMeshRenderer.enabled = false;
                gameObject.tag="TeaSet";
                // 如果正在检视，则返回原位置
                ReturnToOriginalPosition();
            }
            else
            {
                SetPanelActive(true);
                // 如果不在检视，则开始检视
                originalMeshRenderer.enabled = true;
                gameObject.tag="Untagged";
                if (originalSkinnedMeshRenderer != null)
                {
                    // 将SkinnedMeshRenderer转换为普通MeshRenderer以便检视
                    Mesh bakedMesh = new Mesh();
                    originalSkinnedMeshRenderer.BakeMesh(bakedMesh);
                    
                    // 确保有MeshFilter组件
                    MeshFilter meshFilter = GetComponent<MeshFilter>();
                    if (meshFilter == null)
                    {
                        meshFilter = gameObject.AddComponent<MeshFilter>();
                    }
                    meshFilter.mesh = bakedMesh;
                    
                    // 确保有MeshRenderer组件
                    MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
                    if (meshRenderer == null)
                    {
                        meshRenderer = gameObject.AddComponent<MeshRenderer>();
                    }
                    meshRenderer.materials = originalMaterials;
                    
                    // 禁用SkinnedMeshRenderer
                    originalSkinnedMeshRenderer.enabled = false;
                    
                    Debug.Log("已转换SkinnedMeshRenderer为普通MeshRenderer");
                }

                StartInspection();
            }
        }

        /// <summary>
        /// 处理鼠标拖动事件
        /// </summary>
        // 保存鼠标上次位置
        // 保存鼠标上次位置
        private Vector3? lastMousePos = null;

        private void HandleMouseDrag()
        {
            if (!isInspecting) return;

            if (Input.GetMouseButtonDown(1)) // 右键按下
            {
                lastMousePos = Input.mousePosition;
            }

            if (Input.GetMouseButton(1) && lastMousePos.HasValue) // 右键拖动
            {
                Vector3 currentMousePos = Input.mousePosition;

                // 计算鼠标移动增量（归一化）
                Vector2 delta = (currentMousePos - lastMousePos.Value) / new Vector2(Screen.width, Screen.height);

                // 根据鼠标移动方向计算旋转
                Vector3 rotationAxis = Vector3.zero;
                float rotationSpeed = 200f; // 旋转速度系数

                // 水平移动控制左右旋转（绕Y轴）
                if (Mathf.Abs(delta.x) > 0.001f)
                {
                    rotationAxis += Vector3.up * delta.x * rotationSpeed;
                }

                // 垂直移动控制上下旋转（绕X轴）
                if (Mathf.Abs(delta.y) > 0.001f)
                {
                    rotationAxis -= Vector3.right * delta.y * rotationSpeed;
                }

                // 应用旋转
                if (rotationAxis.sqrMagnitude > 0.001f)
                {
                    Vector3 objectCenter = GetObjectCenter();
                    transform.RotateAround(objectCenter, rotationAxis.normalized, rotationAxis.magnitude);
                }

                lastMousePos = currentMousePos;
            }

            if (Input.GetMouseButtonUp(1)) // 右键松开
            {
                lastMousePos = null;
            }
        }


        /// <summary>
        /// 获取茶具的几何中心
        /// </summary>
        private Vector3 GetObjectCenter()
        {
            // 如果有SkinnedMeshRenderer，尝试获取其边界中心
            if (originalSkinnedMeshRenderer != null && originalSkinnedMeshRenderer.enabled)
            {
                return originalSkinnedMeshRenderer.bounds.center;
            }
            
            // 如果有MeshRenderer，使用其边界中心
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                return meshRenderer.bounds.center;
            }
            
            // 如果有Collider，使用其边界中心
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                return collider.bounds.center;
            }
            
            // 默认使用物体自身位置
            return transform.position;
        }
        
        /// <summary>
        /// 鼠标按下事件 - 使用Unity内置的OnMouseDown
        /// </summary>
        private void OnMouseDown()
        {
            if (!enabled) return; // 检查组件是否启用
            Debug.Log($"OnMouseDown: 左键点击茶具 {gameObject.name}");
            OnTeaSetClick();
        }

        /// <summary>
        /// 鼠标悬停时检测右键点击
        /// </summary>
        private void OnMouseOver()
        {
            if (!enabled) return; // 检查组件是否启用
            
            // 检测右键点击
            if (Input.GetMouseButtonDown(1)) // 右键
            {
                // 检查是否有其他物体正在检视，如果有则不允许打开提示面板
                if (isAnyObjectInspecting && currentInspectingObject != null && currentInspectingObject != this)
                {
                    Debug.Log($"无法为 {gameObject.name} 打开提示面板，因为 {currentInspectingObject.gameObject.name} 正在检视中");
                    return;
                }
                
                // 检查茶具是否在检视状态，如果是则不显示提示面板
                if (isInspecting)
                {
                    Debug.Log($"茶具 {gameObject.name} 正在检视中，不显示提示面板");
                    return;
                }

                Debug.Log($"右键点击茶具 {gameObject.name}");
                // 右键点击时调用提示面板相关代码
                Global.CurrentKeyword.Value = GameData.Instance.hintDatabase_SO.GetHint(gameObject.name, Global.CurrentLanguage.Value);
                //if(UIKit.GetPanel<UIHoverInfoPanel>()==null)
                //{
                //    UIKit.OpenPanel<UIHoverInfoPanel>(UILevel.PopUI, null, null, "UIPrefabs/UIHoverInfoPanel");
                //}
            }
        }
        
        // 开始检视模型
        private void StartInspection()
        {
            Debug.Log("StartInspection");
            isInspecting = true;
            
            // 设置全局检视状态
            isAnyObjectInspecting = true;
            currentInspectingObject = this;
            
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            originalParent = transform.parent;
            
            // 移动到检视区域，使用世界坐标
            transform.SetParent(inspectionArea);
            // 直接设置到检视区域的世界坐标位置
            transform.position = inspectionArea.position;
            // 保持原始旋转，不要重置
            // transform.localRotation = Quaternion.identity;

            
            Debug.Log($"Started inspecting {gameObject.name}");
            Debug.Log($"Tea set moved to inspection area at: {transform.position}");
            Debug.Log($"Inspection area position: {inspectionArea.position}");
        }
        
        // 返回原位置
        public void ReturnToOriginalPosition()
        {
            Debug.Log("ReturnToOriginalPosition");
            isInspecting = false;
            
            // 清除全局检视状态
            if (currentInspectingObject == this)
            {
                isAnyObjectInspecting = false;
                currentInspectingObject = null;
                Debug.Log("全局检视状态已清除");
            }
            
            transform.SetParent(originalParent);
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            
            // 恢复原始组件
            if (originalSkinnedMeshRenderer != null)
            {
                // 重新启用SkinnedMeshRenderer
                originalSkinnedMeshRenderer.enabled = true;
                
                // 移除临时添加的组件
                MeshFilter tempMeshFilter = GetComponent<MeshFilter>();
                if (tempMeshFilter != null && tempMeshFilter != originalMeshFilter)
                {
                    DestroyImmediate(tempMeshFilter);
                }
                
                MeshRenderer tempMeshRenderer = GetComponent<MeshRenderer>();
                if (tempMeshRenderer != null && tempMeshRenderer != originalMeshRenderer)
                {
                    DestroyImmediate(tempMeshRenderer);
                }
                
                Debug.Log("已恢复原始SkinnedMeshRenderer");
            }
            
            Debug.Log($"Returned {gameObject.name} to original position");
        }
        private void SetPanelActive(bool isActive)
        {
            if (isActive)
            {
                var panel = UIKit.OpenPanel<UICheckModelPanel>(UILevel.Common, null, null, "UIPrefabs/UICheckModelPanel");

                // 从 Resources 文件夹加载 JSON 配置
                TextAsset jsonFile = Resources.Load<TextAsset>("Config/uicheckmodelpanel");

                if (jsonFile != null)
                {
                    // 使用 JsonUtility 解析 JSON 内容
                    PanelDataList panelDataList = JsonUtility.FromJson<PanelDataList>("{\"panels\":" + jsonFile.text + "}");

                    string panelName = gameObject.name;
                    PanelData panelData = null;

                    // 查找与 panelName 匹配的面板数据
                    foreach (var data in panelDataList.panels)
                    {
                        if (data.title == panelName)
                        {
                            panelData = data;
                            break;
                        }
                    }

                    if (panelData != null)
                    {
                        string title = panelData.title;
                        string content = panelData.content;

                        // 延迟初始化内容
                        panel.DelayFrame(0, () => panel.InitText(title + "\r\n" + content));
                    }
                    else
                    {
                        Debug.LogError("未找到对应的面板配置！");
                    }
                }
                else
                {
                    Debug.LogError("JSON 配置文件未找到！");
                }
            }
            else
            {
                UIKit.ClosePanel<UICheckModelPanel>();
            }
        }
        // 添加鼠标悬停检测（可选，用于调试）
        //private void OnMouseEnter()
        //{
        //    if (!enabled) return; // 检查组件是否启用
        //    Debug.Log($"Mouse entered {gameObject.name}");
        //}

        //private void OnMouseExit()
        //{
        //    if (!enabled) return; // 检查组件是否启用
        //    Debug.Log($"Mouse exited {gameObject.name}");
        //    // 只有在非检视状态下才关闭提示面板
        //    //if (!isInspecting)
        //    //{
        //    //    CloseHoverInfoPanel();
        //    //}
        //}

        /// <summary>
        /// 关闭提示面板
        /// </summary>
        //private void CloseHoverInfoPanel()
        //{
        //    //var hoverPanel = UIKit.GetPanel<UIHoverInfoPanel>();
        //    //if (hoverPanel != null)
        //    //{
        //    //    UIKit.ClosePanel<UIHoverInfoPanel>();
        //    //    Debug.Log("提示面板已关闭");
        //    //}
        //}
    }
}
