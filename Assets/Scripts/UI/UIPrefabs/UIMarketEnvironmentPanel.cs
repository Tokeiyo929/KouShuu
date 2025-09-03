using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIMarketEnvironmentPanelData : UIPanelData
	{
	}
	public partial class UIMarketEnvironmentPanel : UIPanel
	{
		[SerializeField] private Sprite Sprite_On;
		[SerializeField] private Sprite Sprite_Off;

		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIMarketEnvironmentPanelData ?? new UIMarketEnvironmentPanelData();
			// please add init code here

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
		}

		private void OnClickButton()
		{
			Btn_Next_1.onClick.AddListener(OnClickNext_1);
			Btn_Submit_1.onClick.AddListener(OnClickSubmit_1);

			Tog_option_1.onValueChanged.AddListener((isOn)=>ChangeSpriteOn(Tog_option_1.image, isOn));
			Tog_option_2.onValueChanged.AddListener((isOn)=>ChangeSpriteOn(Tog_option_2.image, isOn));
			Tog_option_3.onValueChanged.AddListener((isOn)=>ChangeSpriteOn(Tog_option_3.image, isOn));
		}

		private void OnClickNext_1()
		{
			Debug.Log("OnClickNext_1");
			UIKit.ClosePanel<UIMarketEnvironmentPanel>();
			UIKit.OpenPanel<UIConsumerPanel>(UILevel.Common, null, null, "UIPrefabs/UIConsumerPanel");
		}

		private void OnClickSubmit_1()
		{
			Debug.Log("OnClickSubmit_1");
			Btn_Next_1.gameObject.SetActive(true);
			Btn_Submit_1.gameObject.SetActive(false);

			if(Tog_option_1.isOn)
			{
				Img_Correct.gameObject.SetActive(true);
				Img_Error.gameObject.SetActive(false);

				Global.ScoreList[14] = 4;
			}
			else
			{
				Img_Correct.gameObject.SetActive(false);
				Img_Error.gameObject.SetActive(true);

				Global.ScoreList[14] = 0;
			}

			Tog_option_1.isOn = true;
			Tog_option_2.isOn = false;
			Tog_option_3.isOn = false;
		}

		private void ChangeSpriteOn(Image image, bool isOn)
		{	
			if(isOn)
			{
				image.sprite = Sprite_On;
			}
			else
			{
				image.sprite = Sprite_Off;
			}
		}
	}
}
