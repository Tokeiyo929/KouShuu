using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System.Collections.Generic;
using System.Linq;

namespace QFramework.Example
{
	public class UICommunicationPanelData : UIPanelData
	{
	}
	public partial class UICommunicationPanel : UIPanel
	{
		private List<Button> contentButtons = new List<Button>();
		private List<Transform> pageItems = new List<Transform>();
		private int currentPageIndex = -1;
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UICommunicationPanelData ?? new UICommunicationPanelData();
			
			// 获取按钮和页面
			GetButtons();
			GetPages();
			BindButtons();
			HideAllPages();
			
			// 设置Scrollbar共享
			SetupScrollbarSharing();
			
			// 绑定退出按钮
			BindExitButtons();
		}
		
		private void GetButtons()
		{
			if (Content_Obj == null) return;
			
			Button[] buttons = Content_Obj.GetComponentsInChildren<Button>(true);
			contentButtons.AddRange(buttons);
			contentButtons.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
		}
		
		private void GetPages()
		{
			if (Page_Little == null) return;
			
			for (int i = 0; i < Page_Little.childCount; i++)
			{
				pageItems.Add(Page_Little.GetChild(i));
			}
		}
		
		private void BindButtons()
		{
			int count = Mathf.Min(contentButtons.Count, pageItems.Count);
			
			for (int i = 0; i < count; i++)
			{
				int index = i;
				contentButtons[i].onClick.AddListener(() => ShowPage(index));
			}
		}
		
		private void HideAllPages()
		{
			foreach (Transform page in pageItems)
			{
				page.gameObject.SetActive(false);
			}
			currentPageIndex = -1;
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
		}
		
		private void SetupScrollbarSharing()
		{
			if (Page_Little == null) return;
			
			// 获取Page_Little下的所有Scrollbar
			Scrollbar[] scrollbars = Page_Little.GetComponentsInChildren<Scrollbar>(true);
			
			// 按父物体的父物体的父物体分组
			var groups = scrollbars.GroupBy(s => s.transform.parent.parent.parent).ToList();
			
			foreach (var group in groups)
			{
				var scrollbarList = group.ToList();
				
				// 如果同一父物体下有多个Scrollbar，则共享Value
				if (scrollbarList.Count > 1)
				{
					ShareScrollbarValues(scrollbarList);
				}
			}
		}
		
		private void ShareScrollbarValues(List<Scrollbar> scrollbars)
		{
			// 为每个Scrollbar绑定事件，当值改变时同步其他Scrollbar
			foreach (var scrollbar in scrollbars)
			{
				scrollbar.onValueChanged.AddListener(value => {
					foreach (var otherScrollbar in scrollbars)
					{
						if (otherScrollbar != scrollbar)
						{
							otherScrollbar.value = value;
						}
					}
				});
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
