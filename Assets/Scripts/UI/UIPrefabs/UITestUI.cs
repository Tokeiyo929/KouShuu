using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UITestUIData : UIPanelData
	{
	}
	public partial class UITestUI : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UITestUIData ?? new UITestUIData();
			// please add init code here
			Btn_Test.onClick.AddListener(() =>{Debug.Log("Btn_Test");});
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
