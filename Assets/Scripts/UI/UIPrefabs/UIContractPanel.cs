using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System.Collections.Generic;

namespace QFramework.Example
{
	public class UIContractPanelData : UIPanelData
	{
	}
	public partial class UIContractPanel : UIPanel
	{
		private List<Button> contentButtons = new List<Button>();
		private List<Transform> pageItems = new List<Transform>();
		private int currentPageIndex = -1;
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIContractPanelData ?? new UIContractPanelData();
			
			// 获取按钮和页面
			GetButtons();
			GetPages();
			BindButtons();
			HideAllPages();

			BindExitButtons();

        }
		
		private void GetButtons()
		{
			if (Content_Obj == null) return;
			
			Button[] buttons = Content_Obj.GetComponentsInChildren<Button>(true);
			contentButtons.AddRange(buttons);
			contentButtons.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
			
			Debug.Log($"UIContractPanel: 找到 {contentButtons.Count} 个按钮");
		}
		
		private void GetPages()
		{
			if (Page_Little == null) return;
			
			for (int i = 0; i < Page_Little.childCount; i++)
			{
				pageItems.Add(Page_Little.GetChild(i));
			}
			
			Debug.Log($"UIContractPanel: 找到 {pageItems.Count} 个页面");
		}
		
		private void BindButtons()
		{
			int count = Mathf.Min(contentButtons.Count, pageItems.Count);
			
			for (int i = 0; i < count; i++)
			{
				int index = i;
				contentButtons[i].onClick.AddListener(() => ShowPage(index));
				Debug.Log($"UIContractPanel: 绑定按钮 {i} -> 页面 {index}");
			}
		}
        private void BindExitButtons()
        {
            // 查找所有名为"Btn_Exit"的Button组件
            Button[] allButtons = GetComponentsInChildren<Button>(true);
            List<Button> exitButtons = new List<Button>();

            foreach (Button button in allButtons)
            {
                if (button.gameObject.name == "Btn_Exit")
                {
                    exitButtons.Add(button);
                }
            }

            // 为每个退出按钮添加停止语音功能
            foreach (Button exitButton in exitButtons)
            {
                exitButton.onClick.AddListener(() => {
                    AudioManager.Instance.StopVoice();
                    Debug.Log("Btn_Exit点击 - 停止语音播放");
                });
            }

            Debug.Log($"找到并绑定了 {exitButtons.Count} 个Btn_Exit按钮");
        }

        private void HideAllPages()
		{
			foreach (Transform page in pageItems)
			{
				page.gameObject.SetActive(false);
			}
			currentPageIndex = -1;
			Debug.Log("UIContractPanel: 隐藏所有页面");
		}
		
		private void ShowPage(int index)
		{
			if (index < 0 || index >= pageItems.Count) return;
			
			// 隐藏当前页面
			if (currentPageIndex >= 0 && currentPageIndex < pageItems.Count)
			{
				pageItems[currentPageIndex].gameObject.SetActive(false);
			}
			
			// 显示新页面
			pageItems[index].gameObject.SetActive(true);
			currentPageIndex = index;
			
			Debug.Log($"UIContractPanel: 显示页面 {index}");
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
			HideAllPages();
		}
	}
}
