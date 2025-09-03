using UnityEngine;
using QFramework.Example;

/// <summary>
/// CameraController测试脚本
/// 可以挂载到任意GameObject上进行测试
/// </summary>
public class CameraControllerTest : MonoBehaviour
{
    [Header("测试按键")]
    [SerializeField] private KeyCode toggleControlKey = KeyCode.T;
    [SerializeField] private KeyCode resetCameraKey = KeyCode.R;
    [SerializeField] private KeyCode refreshCameraKey = KeyCode.F;

    void Update()
    {
        // 测试控制开关
        if (Input.GetKeyDown(toggleControlKey))
        {
            var controller = CameraController.Instance;
            // 切换控制状态
            controller.SetControlActive(!IsControlActive());
        }

        // 测试重置摄像头
        if (Input.GetKeyDown(resetCameraKey))
        {
            CameraController.Instance.ResetCameraToDefault();
        }

        // 测试刷新摄像头
        if (Input.GetKeyDown(refreshCameraKey))
        {
            CameraController.Instance.RefreshCamera();
        }
    }

    /// <summary>
    /// 检查控制是否激活
    /// </summary>
    private bool IsControlActive()
    {
        var currentCamera = CameraController.Instance.GetCurrentCamera();
        return currentCamera != null;
    }

    void OnGUI()
    {
        // 显示使用说明
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("=== CameraController 测试 ===");
        GUILayout.Label($"当前摄像头: {(CameraController.Instance.GetCurrentCamera()?.name ?? "未找到")}");
        GUILayout.Label("");
        GUILayout.Label("控制说明:");
        GUILayout.Label("• WASD - 移动摄像头");
        GUILayout.Label("• 鼠标右键拖拽 - 转向");
        GUILayout.Label("• Q/E - 上下移动");
        GUILayout.Label("");
        GUILayout.Label("测试按键:");
        GUILayout.Label($"• {toggleControlKey} - 切换控制开关");
        GUILayout.Label($"• {resetCameraKey} - 重置摄像头");
        GUILayout.Label($"• {refreshCameraKey} - 刷新摄像头");
        GUILayout.EndArea();
    }
} 