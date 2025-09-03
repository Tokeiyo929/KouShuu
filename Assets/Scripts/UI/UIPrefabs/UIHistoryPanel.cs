using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System.Collections.Generic;

namespace QFramework.Example
{
	public class UIHistoryPanelData : UIPanelData
	{
	}
	public partial class UIHistoryPanel : UIPanel
	{
		[Header("页面控制")]
		private List<Button> contentButtons = new List<Button>();
		private List<Transform> pageItems = new List<Transform>();
		private int currentActivePageIndex = -1;
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIHistoryPanelData ?? new UIHistoryPanelData();
			// please add init code here
			
			InitializeButtonsAndPages();
		}
		
		/// <summary>
		/// 初始化按钮和页面的对应关系
		/// </summary>
		private void InitializeButtonsAndPages()
		{
			// 清空列表
			contentButtons.Clear();
			pageItems.Clear();
			
			// 获取Content_Obj下的所有按钮
			GetButtonsFromContent();
			
			// 获取Page_Little下的所有子物体
			GetPageItemsFromPageLittle();
			
			// 绑定按钮事件
			BindButtonEvents();
			
			// 初始化页面状态（全部隐藏）
			InitializePageStates();
			
			Debug.Log($"历史面板初始化完成 - 找到 {contentButtons.Count} 个按钮，{pageItems.Count} 个页面");
		}
		
		/// <summary>
		/// 获取Content_Obj下的所有按钮
		/// </summary>
		private void GetButtonsFromContent()
		{
			if (Content_Obj == null)
			{
				Debug.LogError("Content_Obj 为空！请检查UI设置");
				return;
			}
			
			// 递归获取所有按钮组件
			Button[] buttons = Content_Obj.GetComponentsInChildren<Button>(true);
			
			foreach (Button button in buttons)
			{
				contentButtons.Add(button);
				Debug.Log($"找到按钮: {button.name}");
			}
			
			// 如果需要按层级顺序排序，可以使用以下代码：
			// contentButtons.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
		}
		
		/// <summary>
		/// 获取Page_Little下的所有子物体
		/// </summary>
		private void GetPageItemsFromPageLittle()
		{
			if (Page_Little == null)
			{
				Debug.LogError("Page_Little 为空！请检查UI设置");
				return;
			}
			
			// 按顺序获取所有子物体
			for (int i = 0; i < Page_Little.childCount; i++)
			{
				Transform child = Page_Little.GetChild(i);
				pageItems.Add(child);
				Debug.Log($"找到页面项: {child.name}");
			}
		}
		
		/// <summary>
		/// 绑定按钮事件
		/// </summary>
		private void BindButtonEvents()
		{
			int buttonCount = contentButtons.Count;
			int pageCount = pageItems.Count;
			
			// 取较小的数量进行绑定
			int bindCount = Mathf.Min(buttonCount, pageCount);
			
			if (buttonCount != pageCount)
			{
				Debug.LogWarning($"按钮数量({buttonCount})与页面数量({pageCount})不匹配！将绑定前{bindCount}个");
			}
			
			// 为每个按钮绑定对应的页面
			for (int i = 0; i < bindCount; i++)
			{
				int pageIndex = i; // 闭包变量
				
				Button button = contentButtons[i];
				button.onClick.AddListener(() => {
					ShowPage(pageIndex);
					Debug.Log($"点击按钮 {button.name}，显示页面 {pageItems[pageIndex].name}");
				});
				
				Debug.Log($"绑定按钮 {button.name} -> 页面 {pageItems[i].name}");
			}
		}
		
		/// <summary>
		/// 初始化页面状态（全部隐藏）
		/// </summary>
		private void InitializePageStates()
		{
			foreach (Transform pageItem in pageItems)
			{
				pageItem.gameObject.SetActive(false);
			}
			
			currentActivePageIndex = -1;
			Debug.Log("所有页面已隐藏");
		}
		
		/// <summary>
		/// 显示指定页面，隐藏其他页面
		/// </summary>
		/// <param name="pageIndex">页面索引</param>
		private void ShowPage(int pageIndex)
		{
			if (pageIndex < 0 || pageIndex >= pageItems.Count)
			{
				Debug.LogError($"页面索引超出范围: {pageIndex}，总页面数: {pageItems.Count}");
				return;
			}
			
			// 隐藏当前显示的页面
			if (currentActivePageIndex >= 0 && currentActivePageIndex < pageItems.Count)
			{
				pageItems[currentActivePageIndex].gameObject.SetActive(false);
			}
			
			// 显示指定页面
			pageItems[pageIndex].gameObject.SetActive(true);
			currentActivePageIndex = pageIndex;
			
			Debug.Log($"显示页面: {pageItems[pageIndex].name} (索引: {pageIndex})");
		}
		
		/// <summary>
		/// 隐藏所有页面
		/// </summary>
		public void HideAllPages()
		{
			foreach (Transform pageItem in pageItems)
			{
				pageItem.gameObject.SetActive(false);
			}
			
			currentActivePageIndex = -1;
			Debug.Log("隐藏所有页面");
		}
		
		/// <summary>
		/// 获取当前显示的页面索引
		/// </summary>
		/// <returns>当前页面索引，-1表示没有页面显示</returns>
		public int GetCurrentPageIndex()
		{
			return currentActivePageIndex;
		}
		
		/// <summary>
		/// 通过索引显示页面（公共方法）
		/// </summary>
		/// <param name="pageIndex">页面索引</param>
		public void ShowPageByIndex(int pageIndex)
		{
			ShowPage(pageIndex);
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
		}
	}
}
