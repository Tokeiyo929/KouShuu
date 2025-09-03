using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System.Collections.Generic;

namespace QFramework.Example
{
	public class UITeaHistoryPanelData : UIPanelData
	{
	}
	public partial class UITeaHistoryPanel : UIPanel
	{
		private List<Button> contentButtons = new List<Button>();
		private List<Transform> imgBgChildren = new List<Transform>();
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UITeaHistoryPanelData ?? new UITeaHistoryPanelData();
			
			// 获取按钮和子物体
			GetButtons();
			GetImgBgChildren();
			BindButtons();
			
			// 初始化状态
			InitializeState();
		}
		
		private void GetButtons()
		{
			if (Content_Obj == null) return;
			
			Button[] buttons = Content_Obj.GetComponentsInChildren<Button>(true);
			contentButtons.AddRange(buttons);
			contentButtons.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
		}
		
		private void GetImgBgChildren()
		{
			if (Img_BG == null) return;
			
			for (int i = 0; i < Img_BG.transform.childCount; i++)
			{
				imgBgChildren.Add(Img_BG.transform.GetChild(i));
			}
		}
		
		private void BindButtons()
		{
			int count = Mathf.Min(contentButtons.Count, imgBgChildren.Count);
			
			for (int i = 0; i < count; i++)
			{
				int index = i;
				contentButtons[i].onClick.AddListener(() => ShowPage(index));
			}

			Btn_Exit.onClick.AddListener(() =>{Page_Little.Hide();});
		}
		
		private void InitializeState()
		{
			// 隐藏Page_Little
			if (Page_Little != null)
			{
				Page_Little.gameObject.SetActive(false);
			}
			
			// 隐藏Img_BG下的所有子物体（排除最后一个退出按钮）
			for (int i = 0; i < imgBgChildren.Count - 1; i++)
			{
				imgBgChildren[i].gameObject.SetActive(false);
			}
		}
		
		private void ShowPage(int index)
		{
			if (index < 0 || index >= imgBgChildren.Count - 1) return;
			
			// 首先打开Page_Little
			if (Page_Little != null)
			{
				Page_Little.gameObject.SetActive(true);
			}
			
			// 隐藏其他子物体（排除最后一个退出按钮）
			for (int i = 0; i < imgBgChildren.Count - 1; i++)
			{
				imgBgChildren[i].gameObject.SetActive(false);
			}
			
			// 显示对应的子物体
			imgBgChildren[index].gameObject.SetActive(true);
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
