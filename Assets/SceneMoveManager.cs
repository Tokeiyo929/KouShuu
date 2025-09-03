using System.Collections;
using UnityEngine;

namespace QFramework.Example
{
    [MonoSingletonPath("SceneMoveManager")]
    public class SceneMoveManager : MonoBehaviour, ISingleton
    {
        string targetSceneName="XD";
        private Camera sceneCamera;
        private SceneMove sceneMove;

        // 如果调用太早，临时缓存
        private bool pendingTransfer = false;
        private int pendingIndex = -1;

        void ISingleton.OnSingletonInit()
        {
            CameraController.OnCameraBound += HandleCameraBound;
        }

        private static SceneMoveManager instance;
        public static SceneMoveManager Instance
        {
            get
            {
                if (!instance)
                {
                    var uiRoot = UIRoot.Instance;
                    Debug.Log("currentUIRoot:" + uiRoot);
                    instance = MonoSingletonProperty<SceneMoveManager>.Instance;
                }
                return instance;
            }
        }

        private void HandleCameraBound(Camera camera)
        {
            if(SceneManager.Instance.GetCurrentExtraSceneName()!=targetSceneName)
            {
                return;
            }

            sceneCamera = camera;
            sceneMove = sceneCamera.GetComponent<SceneMove>();
            Debug.Log($"[SceneMoveManager] 收到摄像机绑定通知: {camera.name}");

            // 如果之前有等待执行的传送任务，现在可以执行了
            if (pendingTransfer)
            {
                pendingTransfer = false;
                TransferImmediately(pendingIndex);
                pendingIndex = -1;
            }
        }

        public void TransferImmediately(int index)
        {
            Debug.Log("TransferImmediately:"+index);
            if (sceneCamera == null)
            {
                Debug.LogWarning("[SceneMoveManager] 摄像机为空，延迟执行 TransferImmediately");
                pendingTransfer = true;
                pendingIndex = index;
                return;
            }

            if (sceneMove != null)
            {
                try
                {
                    sceneMove.TransferImmediately(index);
                    Debug.Log($"[SceneMoveManager] 成功调用 SceneMove.TransferImmediately({index})");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[SceneMoveManager] 调用 SceneMove.TransferImmediately({index}) 出错: {e.Message}");
                }
            }
            else
            {
                Debug.LogError("[SceneMoveManager] 无法找到 SceneMove 组件，请检查摄像机");
            }
        }
    }
}
