using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;
using System;

namespace QFramework.Example
{
	public class UITitlePanelData : UIPanelData
	{
	}
	public partial class UITitlePanel : UIPanel
	{
		// 公共的返回按钮行为委托，其他脚本可以直接设置
		public static Action OnBackButtonClick;
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UITitlePanelData ?? new UITitlePanelData();

			// 先设置委托，确保在按钮事件注册时委托已经可用
			OnBackButtonClick=OnClickLastButton;

			RegisterEvents();
			RegisterButtonEvents();
			// please add init code here

			Global.CurrentStep.RegisterWithInitValue(newValue=>
			{
				RestBtnImg();
				GetComponentsInChildren<TitleLittleStepImgControl>()[newValue].SetHightLight(true);
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			UIKit.OpenPanel<UIDialoguePanel>(UILevel.PopUI, null, null, "UIPrefabs/UIDialoguePanel");
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

		//注册按钮的事件
		void RegisterButtonEvents()
		{
			Btn_Step1.onClick.AddListener(()=>
			{
				Global.CurrentStep.Value=0;
				UIKit.CloseAllPanel();
				UIKit.OpenPanel<UITitlePanel>(UILevel.PopUI, null, null, "UIPrefabs/UITitlePanel");
				UIKit.OpenPanel<UIDialoguePanel>(UILevel.PopUI, null, null, "UIPrefabs/UIDialoguePanel");
				UIKit.OpenPanel<UISelectPanel>(UILevel.Common, null, null, "UIPrefabs/UISelectPanel");
				
				SceneManager.Instance.UnloadCurrentScene();
			});

			Btn_Step2.onClick.AddListener(()=>
			{
				Global.CurrentStep.Value=1;
				SceneManager.Instance.UnloadCurrentScene();

				UIKit.CloseAllPanel();
				UIKit.OpenPanel<UITitlePanel>(UILevel.PopUI, null, null, "UIPrefabs/UITitlePanel");
				UIKit.OpenPanel<UIDialoguePanel>(UILevel.PopUI, null, null, "UIPrefabs/UIDialoguePanel");
                UIKit.OpenPanel<UILevle2TransitionPanel>(UILevel.Common, null, null, "UIPrefabs/UILevle2TransitionPanel");
		
				//UIKit.CloseAllPanel();
				//UIKit.OpenPanel<UITitlePanel>(UILevel.PopUI, null, null, "UIPrefabs/UITitlePanel");
				//UIKit.OpenPanel<UIDialoguePanel>(UILevel.PopUI, null, null, "UIPrefabs/UIDialoguePanel");
				//UIKit.OpenPanel<UISelectPanel>(UILevel.Common, null, null, "UIPrefabs/UILevle2TransitionPanel");
			});

			Btn_Step3.onClick.AddListener(()=>
			{
				Global.CurrentStep.Value=2;

				SceneManager.Instance.UnloadCurrentScene();

				UIKit.CloseAllPanel();
				UIKit.OpenPanel<UITitlePanel>(UILevel.PopUI, null, null, "UIPrefabs/UITitlePanel");
				UIKit.OpenPanel<UIDialoguePanel>(UILevel.PopUI, null, null, "UIPrefabs/UIDialoguePanel");
                UIKit.OpenPanel<UITeaMeetingPanel>(UILevel.Common, null, null, "UIPrefabs/UITeaMeetingPanel");

			});

			Btn_Step4.onClick.AddListener(()=>
			{
				Global.CurrentStep.Value=3;

				SceneManager.Instance.UnloadCurrentScene();

				UIKit.CloseAllPanel();
				UIKit.OpenPanel<UITitlePanel>(UILevel.PopUI, null, null, "UIPrefabs/UITitlePanel");
				UIKit.OpenPanel<UIDialoguePanel>(UILevel.PopUI, null, null, "UIPrefabs/UIDialoguePanel");
                UIKit.OpenPanel<UIExtraInvitationPanel>(UILevel.Common, null, null, "UIPrefabs/UIExtraInvitationPanel");
			});

			Btn_Step5.onClick.AddListener(()=>{
				Global.CurrentStep.Value=4;

				SceneManager.Instance.UnloadCurrentScene();

				UIKit.CloseAllPanel();
				UIKit.OpenPanel<UITitlePanel>(UILevel.PopUI, null, null, "UIPrefabs/UITitlePanel");
				UIKit.OpenPanel<UIDialoguePanel>(UILevel.PopUI, null, null, "UIPrefabs/UIDialoguePanel");
                UIKit.OpenPanel<UITipPanel_SignAgreement>(UILevel.Common, null, null, "UIPrefabs/UITipPanel_SignAgreement");
			});

			Btn_ChangText.onClick.AddListener(() =>
			{
				Global.CurrentLanguage.Value=Global.CurrentLanguage.Value==GlobalEnums.Language.Chinese?GlobalEnums.Language.English:GlobalEnums.Language.Chinese;
			});

			Btn_ChangAudio.onClick.AddListener(() =>
			{
				Global.CurrentAudioType.Value=Global.CurrentAudioType.Value==GlobalEnums.AudioType.Chinese?GlobalEnums.AudioType.Cantonese:GlobalEnums.AudioType.Chinese;
			});
			
			// 设置返回按钮事件
			Btn_Back.onClick.AddListener(() =>
			{
				Debug.Log("OnClickLastButton");
				// 执行委托中的方法，如果没有设置则什么都不做
				OnBackButtonClick?.Invoke();
				//ClickLastButton();
			});
			
			// 添加按钮状态检查
			if (Btn_Back != null)
			{
				Debug.Log($"Btn_Back 引用正常，Interactable: {Btn_Back.interactable}");
			}
			else
			{
				Debug.LogError("Btn_Back 引用为空！");
			}
		}

		private void RestBtnImg()
		{
			GetComponentsInChildren<TitleLittleStepImgControl>().ForEach(img=>
			{
				img.SetHightLight(false);
			});
		}

		//注册
		void RegisterEvents()
		{
			Global.CurrentLanguage.RegisterWithInitValue(newValue=>
			{
				Btn_ChangText.GetComponentInChildren<TextMeshProUGUI>().text=newValue
																			==GlobalEnums.Language.Chinese?
																			"英语\n切换":"汉语\n切换";
			}).UnRegisterWhenGameObjectDestroyed(gameObject);

			Global.CurrentAudioType.RegisterWithInitValue(newValue=>
			{
				Btn_ChangAudio.GetComponentInChildren<TextMeshProUGUI>().text=newValue
																			==GlobalEnums.AudioType.Chinese?
																			"粤语\n切换":"汉语\n切换";
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
		}

		private void OnClickLastButton()
		{
			UIKit.CloseAllPanel();
			UIKit.OpenPanel<UICoverPanel>(UILevel.Common, null, null, "UIPrefabs/UICoverPanel");
		}
	}
}
