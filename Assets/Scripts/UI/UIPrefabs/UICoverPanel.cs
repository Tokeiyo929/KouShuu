using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UICoverPanelData : UIPanelData
	{
	}
	public partial class UICoverPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UICoverPanelData ?? new UICoverPanelData();
			// please add init code here
			OnClickClose();
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

		private void OnClickClose()
		{
			Btn_Start.onClick.AddListener(()=>
			{
				UIKit.OpenPanel<UILoginPanel>(UILevel.PopUI, null, null, "UIPrefabs/UILoginPanel");
			});
			Btn_Exit.onClick.AddListener(()=>
			{
				Application.Quit();
			});
		}
	}
}
