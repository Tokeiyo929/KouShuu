/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using QFramework.Example;
using QFramework; // 添加对自定义SceneManager的命名空间引用

namespace FancyScrollView.Example02
{
    class Example02 : MonoBehaviour
    {
        [SerializeField] ScrollView scrollView = default;
        //[SerializeField] Button prevCellButton = default;
        //[SerializeField] Button nextCellButton = default;
        //[SerializeField] Text selectedItemInfo = default;

        string[] strs_CN = new string[] { "秦\n汉", "六\n朝", "隋\n唐\n五\n代", "宋\n元", "明\n代", "清\n代" };
        string[] strs_EN = new string[] { "Qin\nHan", "Six\nDynasties", "Sui\nTang\nFive\nDynasties", "Song\nYuan", "Ming\nDynasty", "Qing\nDynasty" };

        string[] currentStrs;

        //字符串与场景对应字典
        // Dictionary<string, string> strToScene = new Dictionary<string, string>()
        // {
        //     {"秦\n汉", "QH"},
        //     {"六\n朝", "LC"},
        //     {"隋\n唐\n五\n代", "ST"},
        //     {"宋\n元", "SY"},
        //     {"明\n代", "MD"},
        //     {"清\n代", "QD"},
        // };

        string[] sceneNames = new string[] { "QH", "LC", "ST", "SY", "MD", "QD" };

        void Start()
        {
            currentStrs = Global.CurrentLanguage.Value == GlobalEnums.Language.Chinese ? strs_CN : strs_EN;

            //prevCellButton.onClick.AddListener(scrollView.SelectPrevCell);
            //nextCellButton.onClick.AddListener(scrollView.SelectNextCell);
            scrollView.OnSelectionChanged(OnSelectionChanged);
            Global.CurrentLanguage.RegisterWithInitValue(newValue=>
            {
                currentStrs = Global.CurrentLanguage.Value == GlobalEnums.Language.Chinese ? strs_CN : strs_EN;
                var items = Enumerable.Range(0, 6)
                .Select(i => new ItemData(currentStrs[i]))
                .ToArray();
                scrollView.UpdateData(items);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            // 添加中心Cell被点击时的事件处理
            scrollView.OnCenterCellClicked(OnCenterCellClicked);

            var items = Enumerable.Range(0, 6)
                .Select(i => new ItemData(currentStrs[i]))
                .ToArray();

            scrollView.UpdateData(items);
            scrollView.SelectCell(0);
        }

        void OnSelectionChanged(int index)
        {
            //selectedItemInfo.text = $"Selected item info: index {index}";
        }

        // 中心Cell被点击时的处理方法
        void OnCenterCellClicked(int index)
        {
            //Debug.Log($"点击的是{strToScene[strs[index]]}");
            
            // 获取对应的场景名称并加载
            string sceneName = sceneNames[index];
            Debug.Log($"加载场景: {sceneName}");
            
            UIKit.ClosePanel<UISelectPanel>();
            UIKit.OpenPanel<UISelectScencePanel>(UILevel.PopUI, null, null, "UIPrefabs/UISelectScencePanel");
            
            // 使用SceneManager加载场景
            SceneManager.Instance.LoadSceneByName(sceneName);
        }
    }
}
