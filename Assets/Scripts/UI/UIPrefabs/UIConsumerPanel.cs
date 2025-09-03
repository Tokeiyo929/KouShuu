using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIConsumerPanelData : UIPanelData
	{
	}
	public partial class UIConsumerPanel : UIPanel
	{
		[SerializeField] private Sprite Sprite_On;
		[SerializeField] private Sprite Sprite_Off;
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIConsumerPanelData ?? new UIConsumerPanelData();
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
			Btn_Next.onClick.AddListener(OnClickNext);
			Btn_Submit.onClick.AddListener(OnClickSubmit);

			Tog_Consumer_1.onValueChanged.AddListener((isOn)=>ChangeSpriteOn(Tog_Consumer_1.image, isOn));
			Tog_Consumer_2.onValueChanged.AddListener((isOn)=>ChangeSpriteOn(Tog_Consumer_2.image, isOn));
			Tog_Consumer_3.onValueChanged.AddListener((isOn)=>ChangeSpriteOn(Tog_Consumer_3.image, isOn));
		}

		private void OnClickNext()
		{
			Debug.Log("OnClickNext");
			UIKit.ClosePanel<UIConsumerPanel>();
			UIKit.OpenPanel<UITeaSetTypePanel>(UILevel.Common, null, null, "UIPrefabs/UITeaSetTypePanel");
		}

		private void OnClickSubmit()
		{
			Debug.Log("OnClickSubmit");
			Btn_Next.gameObject.SetActive(true);
			Btn_Submit.gameObject.SetActive(false);

			if(Tog_Consumer_1.isOn)
			{
				Img_Correct.gameObject.SetActive(true);
				Img_Error.gameObject.SetActive(false);

				Global.ScoreList[15] = 4;
			}
			else
			{
				Img_Correct.gameObject.SetActive(false);
				Img_Error.gameObject.SetActive(true);

				Global.ScoreList[15] = 0;
			}

			Tog_Consumer_1.isOn = true;
			Tog_Consumer_2.isOn = false;
			Tog_Consumer_3.isOn = false;
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
