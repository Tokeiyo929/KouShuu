using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework.Example;

namespace QFramework.Example
{
    [MonoSingletonPath("Game/CameraController")]
    public class CameraController : MonoBehaviour, ISingleton
    {
        public static CameraController Instance => MonoSingletonProperty<CameraController>.Instance;

        [Header("摄像头控制设置")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float mouseSensitivity = 100f;
        [SerializeField] private float clampAngle = 80f;

        [Header("摄像头查找设置")]
        [SerializeField] private string cameraTag = "PlayerCamera";
        [SerializeField] private string cameraName = "Player Camera";

        [Header("碰撞检测设置")]
        [SerializeField] private LayerMask collisionLayerMask = -1; // 可碰撞的层级
        [SerializeField] private float collisionCheckDistance = 0.1f; // 碰撞检测距离

        [Header("视角高度设置")]
        [SerializeField] private float fixedHeight = 1.7f; // 人视角高度
        [SerializeField] private bool useFixedHeight = true; // 是否使用固定高度

        [SerializeField]private Camera currentCamera;
        private Transform cameraTransform;
        private Rigidbody cameraRigidbody;
        private bool isControlActive = true;
        private bool isRightMousePressed = false;

        // 鼠标旋转相关
        private float rotY = 0f; // 垂直旋转
        private float rotX = 0f; // 水平旋转

        // 状态保存
        private Vector3 savedRotation;
        private bool hasSavedState = false;

        // 场景检测相关
        [SerializeField]private string lastSceneName = "";
        private float sceneCheckInterval = 0.5f; // 每0.5秒检查一次场景变化
        private float sceneCheckTimer = 0f;

        public static event System.Action<Camera> OnCameraBound;

        public void OnSingletonInit()
        {
            // 设置为不销毁
            DontDestroyOnLoad(gameObject);
            Debug.Log("CameraController初始化完成");
        }

        private void Start()
        {
            // 尝试查找当前场景的摄像头
            StartCoroutine(DelayedInitialFind());
        }

        /// <summary>
        /// 延迟初始查找，确保场景完全加载
        /// </summary>
        private IEnumerator DelayedInitialFind()
        {
            yield return new WaitForSeconds(0.5f);
            FindAndBindCamera();
            UpdateLastSceneName();
        }

        private void Update()
        {
            // 定期检查场景变化
            CheckSceneChange();

            if (!isControlActive || currentCamera == null) return;

            string currentSceneName = GetCurrentActiveSceneName();
            if(currentSceneName=="LC"||currentSceneName=="MD"||currentSceneName=="QD"||currentSceneName=="QH"||currentSceneName=="ST"||currentSceneName=="SY")
            {
                HandleMovementInput();
                HandleMouseInput();
            }
        }

        /// <summary>
        /// 检查场景是否发生变化
        /// </summary>
        private void CheckSceneChange()
        {
            sceneCheckTimer += Time.deltaTime;
            if (sceneCheckTimer >= sceneCheckInterval)
            {
                sceneCheckTimer = 0f;
                
                string currentSceneName = GetCurrentActiveSceneName();
                if (currentSceneName != lastSceneName && !string.IsNullOrEmpty(currentSceneName))
                {
                    Debug.Log($"CameraController: 检测到场景变化 - 从 '{lastSceneName}' 到 '{currentSceneName}'");
                    
                    // 保存状态
                    if (currentCamera != null)
                    {
                        SaveCameraState();
                    }
                    
                    // 查找新摄像头
                    StartCoroutine(DelayedCameraFind());
                    
                    lastSceneName = currentSceneName;
                }
            }
        }

        /// <summary>
        /// 获取当前活跃场景名称
        /// </summary>
        private string GetCurrentActiveSceneName()
        {
            // 获取SceneManager的当前场景
            if (SceneManager.Instance != null)
            {
                string sceneName = SceneManager.Instance.GetCurrentSceneName();
                if (!string.IsNullOrEmpty(sceneName))
                {
                    return sceneName;
                }
            }
            
            // 如果SceneManager没有信息，使用Unity的方法获取活跃场景
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            return activeScene.name;
        }

        /// <summary>
        /// 更新记录的场景名称
        /// </summary>
        private void UpdateLastSceneName()
        {
            lastSceneName = GetCurrentActiveSceneName();
        }

        /// <summary>
        /// 处理WASD移动输入
        /// </summary>
        private void HandleMovementInput()
        {
            if (cameraTransform == null) return;

            float horizontal = Input.GetAxis("Horizontal"); // A/D
            float vertical = Input.GetAxis("Vertical");     // W/S
            //float upDown = 0f;

            // 可选：添加QE上下移动
            //if (Input.GetKey(KeyCode.Q)) upDown = -1f;
            //if (Input.GetKey(KeyCode.E)) upDown = 1f;

            Vector3 direction = new Vector3(horizontal, 0f, vertical);
            
            // 转换为世界坐标系下的移动方向
            Vector3 worldDirection = cameraTransform.TransformDirection(direction);
            Vector3 moveVector = worldDirection * moveSpeed * Time.deltaTime;
            
            // 使用射线检测避免穿墙
            Vector3 safePosition = GetSafeMovementPosition(cameraTransform.position, moveVector);

            // 固定Y轴高度
            if (useFixedHeight)
            {
                safePosition.y = fixedHeight;
            }

            cameraTransform.position = safePosition;
        }

        /// <summary>
        /// 获取安全的移动位置，避免穿墙
        /// </summary>
        private Vector3 GetSafeMovementPosition(Vector3 currentPosition, Vector3 moveVector)
        {
            if (moveVector.magnitude < 0.001f) return currentPosition;

            // 获取摄像机的碰撞器半径
            SphereCollider sphereCollider = cameraTransform.GetComponent<SphereCollider>();
            float radius = sphereCollider != null ? sphereCollider.radius : 0.5f;

            // 检测移动方向上是否有障碍物
            RaycastHit hit;
            float checkDistance = moveVector.magnitude + collisionCheckDistance;
            
            if (Physics.SphereCast(currentPosition, radius, moveVector.normalized, out hit, checkDistance, collisionLayerMask))
            {
                // 计算安全距离
                float safeDistance = Mathf.Max(0, hit.distance - radius - collisionCheckDistance);
                Vector3 safeMove = moveVector.normalized * Mathf.Min(safeDistance, moveVector.magnitude);
                return currentPosition + safeMove;
            }

            // 没有碰撞，正常移动
            return currentPosition + moveVector;
        }

        /// <summary>
        /// 处理鼠标右键视角控制
        /// </summary>
        private void HandleMouseInput()
        {
            if (cameraTransform == null) return;

            // 检测鼠标右键
            if (Input.GetMouseButtonDown(1))
            {
                isRightMousePressed = true;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                isRightMousePressed = false;
                Cursor.lockState = CursorLockMode.None;
            }

            // 只有在按住右键时才控制视角
            if (isRightMousePressed)
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

                rotX += mouseX;
                rotY -= mouseY;
                rotY = Mathf.Clamp(rotY, -clampAngle, clampAngle);

                cameraTransform.localRotation = Quaternion.Euler(rotY, rotX, 0f);
            }
        }

        /// <summary>
        /// 延迟查找摄像头
        /// </summary>
        private IEnumerator DelayedCameraFind()
        {
            yield return new WaitForEndOfFrame();
            FindAndBindCamera();
        }

        /// <summary>
        /// 查找并绑定当前场景的摄像头
        /// </summary>
        private void FindAndBindCamera()
        {
            Camera foundCamera = null;

            // 方法1：通过标签查找
            if (!string.IsNullOrEmpty(cameraTag))
            {
                GameObject cameraObj = GameObject.FindGameObjectWithTag(cameraTag);
                if (cameraObj != null)
                {
                    foundCamera = cameraObj.GetComponent<Camera>();
                }
            }

            // 方法2：通过名称查找
            if (foundCamera == null && !string.IsNullOrEmpty(cameraName))
            {
                GameObject cameraObj = GameObject.Find(cameraName);
                if (cameraObj != null)
                {
                    foundCamera = cameraObj.GetComponent<Camera>();
                }
            }

            // 方法3：查找第一个非UI非主摄像头的Camera
            if (foundCamera == null)
            {
                Camera[] allCameras = FindObjectsOfType<Camera>();
                foreach (Camera cam in allCameras)
                {
                    // 跳过UI摄像头和主摄像头
                    if (cam.name.ToLower().Contains("ui") || cam.CompareTag("MainCamera"))
                        continue;
                    
                    foundCamera = cam;
                    break;
                }
            }

            if (foundCamera != null && foundCamera != currentCamera)
            {
                BindCamera(foundCamera);
            }
            else if (foundCamera == null && currentCamera == null)
            {
                Debug.LogWarning($"CameraController: 未找到可控制的摄像头！请确保场景中有标签为'{cameraTag}'或名称为'{cameraName}'的摄像头");
            }
        }

        /// <summary>
        /// 绑定摄像头并应用保存的状态
        /// </summary>
        private void BindCamera(Camera camera)
        {
            currentCamera = camera;
            ObjectHoverManager.Instance.raycastCamera=camera;
            cameraTransform = camera.transform;
            cameraRigidbody = camera.GetComponent<Rigidbody>();

            Debug.Log($"CameraController: 成功绑定摄像头 - {camera.name}");

            // 如果摄像机有Rigidbody组件，设置为运动学模式避免物理冲突
            if (cameraRigidbody != null)
            {
                cameraRigidbody.isKinematic = true;
                cameraRigidbody.useGravity = false;
                Debug.Log("CameraController: 摄像机Rigidbody已设置为运动学模式");
            }

            // 初始化旋转值
            Vector3 currentRotation = cameraTransform.localEulerAngles;
            rotX = currentRotation.y;
            rotY = currentRotation.x;

            // 应用保存的状态（可选）
            if (hasSavedState)
            {
                ApplySavedState();
            }

            OnCameraBound?.Invoke(camera);
        }

        /// <summary>
        /// 保存摄像头状态
        /// </summary>
        private void SaveCameraState()
        {
            if (cameraTransform != null)
            {
                savedRotation = cameraTransform.localEulerAngles;
                hasSavedState = true;
                
                Debug.Log($"CameraController: 保存摄像头旋转状态 - Rotation: {savedRotation}");
            }
        }

        /// <summary>
        /// 应用保存的状态
        /// </summary>
        private void ApplySavedState()
        {
            if (cameraTransform != null && hasSavedState)
            {
                // 只应用旋转，让每个场景有自己的起始位置
                cameraTransform.localEulerAngles = savedRotation;
                
                rotX = savedRotation.y;
                rotY = savedRotation.x;
                
                Debug.Log($"CameraController: 应用保存的摄像头旋转状态");
            }
        }

        /// <summary>
        /// 设置控制是否激活
        /// </summary>
        public void SetControlActive(bool active)
        {
            isControlActive = active;
            
            if (!active)
            {
                // 释放鼠标锁定
                Cursor.lockState = CursorLockMode.None;
                isRightMousePressed = false;
            }
            
            Debug.Log($"CameraController: 控制状态设置为 {active}");
        }

        /// <summary>
        /// 获取当前控制的摄像头
        /// </summary>
        public Camera GetCurrentCamera()
        {
            return currentCamera;
        }

        /// <summary>
        /// 手动设置摄像头
        /// </summary>
        public void SetCamera(Camera camera)
        {
            if (camera != null)
            {
                BindCamera(camera);
            }
        }

        /// <summary>
        /// 重置摄像头到场景默认位置
        /// </summary>
        public void ResetCameraToDefault()
        {
            hasSavedState = false;
            
            if (cameraTransform != null)
            {
                // 重置旋转
                rotX = 0f;
                rotY = 0f;
                cameraTransform.localRotation = Quaternion.identity;
                
                Debug.Log("CameraController: 摄像头已重置到默认状态");
            }
        }

        /// <summary>
        /// 强制重新查找摄像头
        /// </summary>
        public void RefreshCamera()
        {
            Debug.Log("CameraController: 强制刷新摄像头");
            FindAndBindCamera();
        }

        /// <summary>
        /// 设置碰撞检测层级
        /// </summary>
        public void SetCollisionLayerMask(LayerMask layerMask)
        {
            collisionLayerMask = layerMask;
            Debug.Log($"CameraController: 碰撞检测层级已设置为 {layerMask.value}");
        }

        /// <summary>
        /// 设置移动速度
        /// </summary>
        public void SetMoveSpeed(float speed)
        {
            moveSpeed = Mathf.Max(0, speed);
            Debug.Log($"CameraController: 移动速度已设置为 {moveSpeed}");
        }
    }
} 