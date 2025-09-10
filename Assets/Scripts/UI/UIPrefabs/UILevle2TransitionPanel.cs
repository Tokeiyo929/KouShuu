using UnityEngine;
using UnityEngine.UI;
using QFramework;
using DialogueEditor;

namespace QFramework.Example
{
	public class UILevle2TransitionPanelData : UIPanelData
	{
	}
	public partial class UILevle2TransitionPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UILevle2TransitionPanelData ?? new UILevle2TransitionPanelData();
			// please add init code here

			// 设置当前步骤
			Global.CurrentStep.Value = 1;
			ConversationManager.Instance.EndConversation();
			OnClickButton();

			if (TimeLineManager.Instance.GetCurrentSceneName() != null)
			{
				TimeLineManager.Instance.UnloadScene(TimeLineManager.Instance.GetCurrentSceneName());
            }
        }
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
			AnimationManager.Instance.DeactivatePerson("John");
			AnimationManager.Instance.DeactivatePerson("MoLi");
			AnimationManager.Instance.DeactivatePerson("WangGuoXin");
			AnimationManager.Instance.DeactivatePerson("LiWenJun");
			AnimationManager.Instance.DeactivatePerson("LiTianRan");
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}

		//给按钮添加事件
		private void OnClickButton()
		{
			//TitlePanel.OnBackButtonClick = OnClickLastButton;
			Btn_Next.onClick.AddListener(OnClickNextButton);
		}

		private void OnClickLastButton()
		{
			UIKit.CloseAllPanel();
			UIKit.OpenPanel<UITitlePanel>(UILevel.PopUI, null, null, "UIPrefabs/UITitlePanel");
			UIKit.OpenPanel<UIDialoguePanel>(UILevel.PopUI, null, null, "UIPrefabs/UIDialoguePanel");
			UIKit.OpenPanel<UISelectPanel>(UILevel.Common, null, null, "UIPrefabs/UISelectPanel");
		}

		private void OnClickNextButton()
		{
			UIKit.OpenPanel<UISelectClosePanel>(UILevel.Common, null, null, "UIPrefabs/UISelectClosePanel");
			UIKit.ClosePanel<UILevle2TransitionPanel>();
		}
	}
}
