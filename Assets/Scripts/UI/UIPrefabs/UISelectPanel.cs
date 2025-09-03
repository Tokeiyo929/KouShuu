using UnityEngine;
using UnityEngine.UI;
using QFramework;
using DialogueEditor;

namespace QFramework.Example
{
	public class UISelectPanelData : UIPanelData
	{
	}
	public partial class UISelectPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UISelectPanelData ?? new UISelectPanelData();

			Global.CurrentStep.Value=0;
			ConversationManager.Instance.EndConversation();
			// please add init code here
			Btn_AddOnClick();
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			UIKit.OpenPanel<UIHistoryPanel>(UILevel.PopUI, null, null, "UIPrefabs/UIHistoryPanel");
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
			if(UIKit.GetPanel<UIHistoryPanel>() != null) UIKit.ClosePanel<UIHistoryPanel>();
			if(UIKit.GetPanel<UIEnvironmentPanel>() != null) UIKit.ClosePanel<UIEnvironmentPanel>();
			if(UIKit.GetPanel<UITeaHistoryPanel>() != null) UIKit.ClosePanel<UITeaHistoryPanel>();
			if(UIKit.GetPanel<UICommunicationPanel>() != null) UIKit.ClosePanel<UICommunicationPanel>();
			if(UIKit.GetPanel<UIVocabularyPanel>() != null) UIKit.ClosePanel<UIVocabularyPanel>();
			if(UIKit.GetPanel<UILogicPanel>() != null) UIKit.ClosePanel<UILogicPanel>();
			if(UIKit.GetPanel<UIContractPanel>() != null) UIKit.ClosePanel<UIContractPanel>();
			if(UIKit.GetPanel<UITestPanel>() != null) UIKit.ClosePanel<UITestPanel>();
		}

		private void Btn_AddOnClick()
		{
			UITitlePanel.OnBackButtonClick=OnClickLastButton;
			// 使用Action委托 - 当前使用，已添加关闭前判空检查
			Btn_History.onValueChanged.AddListener((isOn) => OnValueChangedWithAction(isOn, 
			    () => UIKit.OpenPanel<UIHistoryPanel>(UILevel.PopUI, null, null, "UIPrefabs/UIHistoryPanel"), 
			    SafeClosePanel<UIHistoryPanel>()));

			Btn_Environment.onValueChanged.AddListener((isOn) => OnValueChangedWithAction(isOn, 
			    () => UIKit.OpenPanel<UIEnvironmentPanel>(UILevel.PopUI, null, null, "UIPrefabs/UIEnvironmentPanel"), 
			    SafeClosePanel<UIEnvironmentPanel>()));
			    
			Btn_Culture.onValueChanged.AddListener((isOn) => OnValueChangedWithAction(isOn, 
			    () => UIKit.OpenPanel<UITeaHistoryPanel>(UILevel.PopUI, null, null, "UIPrefabs/UITeaHistoryPanel"), 
			    SafeClosePanel<UITeaHistoryPanel>()));
			    
			Btn_Communication.onValueChanged.AddListener((isOn) => OnValueChangedWithAction(isOn, 
			    () => UIKit.OpenPanel<UICommunicationPanel>(UILevel.PopUI, null, null, "UIPrefabs/UICommunicationPanel"), 
			    SafeClosePanel<UICommunicationPanel>()));

			Btn_Vocabulary.onValueChanged.AddListener((isOn) => OnValueChangedWithAction(isOn, 
			    () => UIKit.OpenPanel<UIVocabularyPanel>(UILevel.PopUI, null, null, "UIPrefabs/UIVocabularyPanel"), 
			    SafeClosePanel<UIVocabularyPanel>()));
			
			Btn_Logic.onValueChanged.AddListener((isOn) => OnValueChangedWithAction(isOn, 
			    () => UIKit.OpenPanel<UILogicPanel>(UILevel.PopUI, null, null, "UIPrefabs/UILogicPanel"), 
			    SafeClosePanel<UILogicPanel>()));

			Btn_Contract.onValueChanged.AddListener((isOn) => OnValueChangedWithAction(isOn, 
			    () => UIKit.OpenPanel<UIContractPanel>(UILevel.PopUI, null, null, "UIPrefabs/UIContractPanel"), 
			    SafeClosePanel<UIContractPanel>()));

			Btn_Test.onValueChanged.AddListener((isOn) => OnValueChangedWithAction(isOn, 
			    () => UIKit.OpenPanel<UITestPanel>(UILevel.PopUI, null, null, "UIPrefabs/UITestPanel"), 
			    SafeClosePanel<UITestPanel>()));
		}

		private void OnClickLastButton()
		{
			UIKit.CloseAllPanel();
			//Kit.OpenPanel<UIDialoguePanel>(UILevel.PopUI, null, null, "UIPrefabs/UIDialoguePanel");
			UIKit.OpenPanel<UICoverPanel>(UILevel.Common, null, null, "UIPrefabs/UICoverPanel");
			//Kit.OpenPanel<UIExtraInvitationPanel>(UILevel.Common, null, null, "UIPrefabs/UIExtraInvitationPanel");
		}

		private void OnValueChangedWithAction(bool isOn, System.Action openAction, System.Action closeAction)
		{
			if(isOn)
			{
				openAction?.Invoke();
				Debug.Log("面板已打开");
			}
			else
			{
				closeAction?.Invoke();
				Debug.Log("面板关闭操作已执行");
			}
		}
		
		/// <summary>
		/// 安全关闭面板的辅助方法
		/// </summary>
		private System.Action SafeClosePanel<T>() where T : UIPanel
		{
			return () => {
				var panel = UIKit.GetPanel<T>();
				if(panel != null)
				{
					UIKit.ClosePanel<T>();
				}
				else
				{
					Debug.LogWarning($"面板 {typeof(T).Name} 不存在，无需关闭");
				}
			};
		}
	}
}
