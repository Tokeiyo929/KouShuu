using UnityEngine;
using QFramework;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using MoonSharp.Interpreter;
using UnityEngine.UI;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
	public partial class UIHistoryPanel_PageLittle : ViewController
	{
		[Header("分页设置")]
		public int currentPageIndex = 0;
		
		private List<Transform> pages = new List<Transform>();
		private int totalPages = 0;
		
		void Start()
		{
			InitializePages();
			SetupButtons();
			ShowPage(0); // 默认显示第一页
		}

        /// <summary>
        /// 初始化页面列表
        /// </summary>
        private void InitializePages()
		{
			pages.Clear();
			
			// 获取AllPage下的所有子物体作为页面
			// 注意：包含所有子物体，无论其初始状态是否隐藏
			for (int i = 0; i < AllPage.childCount; i++)
			{
				Transform child = AllPage.GetChild(i);
				pages.Add(child);
				
				// 暂时隐藏所有页面，稍后会显示第一页
				child.gameObject.SetActive(false);
			}
			
			totalPages = pages.Count;			
			// 如果没有页面，禁用所有按钮
			if (totalPages == 0)
			{
				Btn_Last.interactable = false;
				Btn_Next.interactable = false;
				Debug.LogWarning("AllPage 下没有找到子页面！");
			}
		}
		
		/// <summary>
		/// 设置按钮事件
		/// </summary>
		private void SetupButtons()
		{
			// 上一页按钮
			Btn_Last.onClick.AddListener(() => {
				if (currentPageIndex > 0)
				{
					ShowPage(currentPageIndex - 1);
				}
			});
			
			// 下一页按钮
			Btn_Next.onClick.AddListener(() => {
				if (currentPageIndex < totalPages - 1)
				{
					ShowPage(currentPageIndex + 1);
				}
			});
			
			// 退出按钮
			Btn_Exit.onClick.AddListener(() => {
				ClosePanel();
			});
		}
		
		/// <summary>
		/// 显示指定页面
		/// </summary>
		/// <param name="pageIndex">页面索引（从0开始）</param>
		private void ShowPage(int pageIndex)
		{
			if (pageIndex < 0 || pageIndex >= totalPages)
			{
				Debug.LogError($"页面索引超出范围: {pageIndex}, 总页数: {totalPages}");
				return;
			}
			// 隐藏所有页面
			for (int i = 0; i < pages.Count; i++)
			{
				pages[i].gameObject.SetActive(false);
			}
			// 显示指定页面
			pages[pageIndex].gameObject.SetActive(true);
			
			// 更新当前页面索引
			currentPageIndex = pageIndex;
			
			// 更新按钮状态
			UpdateButtonStates();
		}
		
		/// <summary>
		/// 更新按钮的可交互状态
		/// </summary>
		private void UpdateButtonStates()
		{
			// 第一页时禁用上一页按钮
			Btn_Last.interactable = (currentPageIndex > 0);
			
			// 最后一页时禁用下一页按钮
			Btn_Next.interactable = (currentPageIndex < totalPages - 1);
		}
		
		/// <summary>
		/// 关闭面板
		/// </summary>
		private void ClosePanel()
		{
			Debug.Log("关闭历史面板");
			
			// 根据您的UI框架，选择合适的关闭方式
			// 如果是UIPanel，使用：
			// this.CloseSelf();
			
			// 如果是普通GameObject，使用：
			gameObject.SetActive(false);
			
			// 或者销毁对象：
			// Destroy(gameObject);
		}
		
		/// <summary>
		/// 跳转到指定页面（公共方法，供外部调用）
		/// </summary>
		/// <param name="pageIndex">页面索引（从0开始）</param>
		public void JumpToPage(int pageIndex)
		{
			ShowPage(pageIndex);
		}
		
		/// <summary>
		/// 获取当前页面信息
		/// </summary>
		/// <returns>当前页面信息</returns>
		public string GetCurrentPageInfo()
		{
			return $"第 {currentPageIndex + 1} 页，共 {totalPages} 页";
		}
		
		/// <summary>
		/// 获取总页数
		/// </summary>
		public int GetTotalPages()
		{
			return totalPages;
		}
		
		/// <summary>
		/// 获取当前页面索引
		/// </summary>
		public int GetCurrentPageIndex()
		{
			return currentPageIndex;
		}
	}
}
