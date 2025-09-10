using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UISelectClosePanelData : UIPanelData
	{
	}
	public partial class UISelectClosePanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UISelectClosePanelData ?? new UISelectClosePanelData();
			// please add init code here
			// 加载任意场景（不需要在scenes列表中）
			//SceneManager.Instance.LoadExtraScene("YiMaoJian");
			TimeLineManager.Instance.LoadScene("YiMaoJian");
            OnClickButton();
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
			//SceneManager.Instance.UnloadCurrentScene();
			TimeLineManager.Instance.UnloadScene("YiMaoJian");
        }

		private void OnClickButton()
		{
			//TitlePanel.OnBackButtonClick =OnClickLastButton;
			Btn_NextStep.onClick.AddListener(OnClickNextButton);
			Btn_LastStep.onClick.AddListener(OnClickLastButton);
		}

		private void OnClickLastButton()
		{
			UIKit.ClosePanel<UISelectClosePanel>();
			UIKit.OpenPanel<UILevle2TransitionPanel>(UILevel.Common, null, null, "UIPrefabs/UILevle2TransitionPanel");
		}	

		private void OnClickNextButton()
		{
			UIKit.ClosePanel<UISelectClosePanel>();
			UIKit.OpenPanel<UIInvitedPanel>(UILevel.Common, null, null, "UIPrefabs/UIInvitedPanel");
		}
	}
}
