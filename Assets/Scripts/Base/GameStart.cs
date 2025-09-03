using UnityEngine;
using QFramework;
using QFramework.Example;
using System.Collections;

public class GameStart : MonoBehaviour
{
    void Awake()
    {
        // 确保UIKit使用Resources加载器（多重保险）
        Debug.Log("[GameStart] 确保UIKit使用Resources加载器");
        UIKit.Config.PanelLoaderPool = new DefaultPanelLoaderPool();
        
        // 添加UIKitInitializer组件（如果还没有）
        if (FindObjectOfType<UIKitInitializer>() == null)
        {
            var initializerObj = new GameObject("UIKitInitializer");
            initializerObj.AddComponent<UIKitInitializer>();
            DontDestroyOnLoad(initializerObj);
        }
        
        // 测试Resources.Load是否能找到UICoverPanel
        Debug.Log("[GameStart] 测试Resources加载...");
        var testLoad1 = Resources.Load<GameObject>("UICoverPanel");
        Debug.Log($"[GameStart] Resources.Load('UICoverPanel'): {(testLoad1 ? "成功" : "失败")}");
        
        var testLoad2 = Resources.Load<GameObject>("UIPrefabs/UICoverPanel");
        Debug.Log($"[GameStart] Resources.Load('UIPrefabs/UICoverPanel'): {(testLoad2 ? "成功" : "失败")}");
        
        // 使用正确的prefab路径打开UI
        Debug.Log("[GameStart] 使用正确路径打开UICoverPanel...");
        UIKit.OpenPanel<UICoverPanel>(UILevel.Common, null, null, "UIPrefabs/UICoverPanel");
    }

    IEnumerator StartGame()
    {
        // 等待ResKit初始化完成
        yield return ResKit.InitAsync();
        
        Debug.Log("[GameStart] ResKit初始化完成，直接使用UIKit打开UI");
        
        // 现在可以直接使用UIKit.OpenPanel，因为已经设置为Resources加载器
        // UIKit会自动从Resources文件夹加载UI Prefab
        
        
        Debug.Log("[GameStart] UICoverPanel加载完成");
    }
}

