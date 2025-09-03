using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System.Diagnostics;

namespace QFramework.Example
{
	public class UISelectScencePanelData : UIPanelData
	{
	}
	public partial class UISelectScencePanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UISelectScencePanelData ?? new UISelectScencePanelData();
			// please add init code here
			InitButton();

			// 监听场景变化
			Global.CurrentScene.RegisterWithInitValue(newValue=>
			{
				UpdateSceneText();
			}).UnRegisterWhenGameObjectDestroyed(gameObject);

			// 监听语言变化
			Global.CurrentLanguage.RegisterWithInitValue(newValue=>
			{
				UpdateSceneText();
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
		}

		/// <summary>
		/// 根据当前语言和场景更新文本显示
		/// </summary>
		private void UpdateSceneText()
		{
			string currentScene = Global.CurrentScene.Value;
			GlobalEnums.Language currentLanguage = Global.CurrentLanguage.Value;
			
			switch(currentScene)
			{
				case "QH":
					Text_CurrentScence.text = currentLanguage == GlobalEnums.Language.Chinese ? 
						"秦汉商粤环境布置" : "Qin-Han Commercial Guangdong Environment";
					break;
				case "LC":
					Text_CurrentScence.text = currentLanguage == GlobalEnums.Language.Chinese ? 
						"六朝商粤环境布置" : "Six Dynasties Commercial Guangdong Environment";
					break;
				case "ST":
					Text_CurrentScence.text = currentLanguage == GlobalEnums.Language.Chinese ? 
						"隋唐六代商粤环境布置" : "Sui-Tang Commercial Guangdong Environment";
					break;
				case "SY":
					Text_CurrentScence.text = currentLanguage == GlobalEnums.Language.Chinese ? 
						"宋元商粤环境布置" : "Song-Yuan Commercial Guangdong Environment";
					break;
				case "MD":
					Text_CurrentScence.text = currentLanguage == GlobalEnums.Language.Chinese ? 
						"明代商粤环境布置" : "Ming Dynasty Commercial Guangdong Environment";
					break;
				case "QD":
					Text_CurrentScence.text = currentLanguage == GlobalEnums.Language.Chinese ? 
						"清代商粤环境布置" : "Qing Dynasty Commercial Guangdong Environment";
					break;
				default:
					Text_CurrentScence.text = currentLanguage == GlobalEnums.Language.Chinese ? 
						"未记录场景" : "Unknown Scene";
					break;
			}
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

		//初始化按钮
		private void InitButton()
		{
			UITitlePanel.OnBackButtonClick = OnClickLastButton;
			Btn_LastScence.onClick.AddListener(() =>
			{
				SceneManager.Instance.LastScene();
			});
			Btn_NextScence.onClick.AddListener(() =>
			{
				SceneManager.Instance.NextScene();
			});
		}

		private void OnClickLastButton()
		{
			SceneManager.Instance.UnloadCurrentScene();
			UIKit.ClosePanel<UISelectScencePanel>();
			UIKit.OpenPanel<UISelectPanel>(UILevel.Common, null, null, "UIPrefabs/UISelectPanel");
		}
	}
}
