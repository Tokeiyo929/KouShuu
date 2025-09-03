using UnityEngine;
using QFramework;
using System.Collections;

namespace QFramework.Example
{
    /// <summary>
    /// UIKit初始化器 - 设置使用Resources加载UI
    /// </summary>
    public class UIKitInitializer : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitUIKit()
        {
            Debug.Log("[UIKit] 强制设置为使用Resources加载器（覆盖ResKit设置）");
            
            // 强制使用默认的Resources加载器，覆盖ResKit的设置
            UIKit.Config.PanelLoaderPool = new DefaultPanelLoaderPool();
            
            Debug.Log("[UIKit] Resources加载器设置完成，已覆盖ResKit加载器");
        }
        
        // 备用方案：在游戏开始时也设置一次
        void Awake()
        {
            Debug.Log("[UIKit] Awake阶段再次确保使用Resources加载器");
            UIKit.Config.PanelLoaderPool = new DefaultPanelLoaderPool();
        }
    }
} 