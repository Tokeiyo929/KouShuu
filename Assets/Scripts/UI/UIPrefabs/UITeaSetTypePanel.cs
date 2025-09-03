using UnityEngine;
using UnityEngine.UI;
using QFramework;
using DialogueEditor;

namespace QFramework.Example
{
	public class UITeaSetTypePanelData : UIPanelData
	{
	}
	public partial class UITeaSetTypePanel : UIPanel
	{
		[SerializeField] private NPCConversation ExtraInvitationConversation_2;
		[SerializeField] private Sprite[] Sprite_On;
		[SerializeField] private Sprite[] Sprite_Off;

		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UITeaSetTypePanelData ?? new UITeaSetTypePanelData();
			// please add init code here

			ExtraInvitationConversation_2=DialogueManager.Instance.Conversations[8];

			Btn_Check.onClick.AddListener(OnClickCheck);
			Btn_Next.onClick.AddListener(OnClickNext);
			
			Tog_TeaSet_1.onValueChanged.AddListener((isOn)=>ChangeSpriteOn(Tog_TeaSet_1.image, 0, isOn));
			Tog_TeaSet_2.onValueChanged.AddListener((isOn)=>ChangeSpriteOn(Tog_TeaSet_2.image, 1, isOn));
			Tog_TeaSet_3.onValueChanged.AddListener((isOn)=>ChangeSpriteOn(Tog_TeaSet_3.image, 2, isOn));
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

		private void OnClickCheck()
		{
			Btn_Next.gameObject.SetActive(true);
			Btn_Check.gameObject.SetActive(false);
			
			if(!Tog_TeaSet_1.isOn||!Tog_TeaSet_2.isOn||!Tog_TeaSet_3.isOn)
			{
				Img_Correct.gameObject.SetActive(false);
				Img_Error.gameObject.SetActive(true);

				Global.ScoreList[16] = 0;
			}
			else
			{
				Img_Correct.gameObject.SetActive(true);
				Img_Error.gameObject.SetActive(false);

				Global.ScoreList[16] = 4;
			}

			Tog_TeaSet_1.isOn = true;
			Tog_TeaSet_2.isOn = true;
			Tog_TeaSet_3.isOn = true;
		}

		private void OnClickNext()
		{
			UIKit.ClosePanel<UITeaSetTypePanel>();
			if(ConversationManager.Instance!=null)
			{
				ConversationManager.Instance.StartConversation(ExtraInvitationConversation_2);
				Debug.Log("play conversation:"+ExtraInvitationConversation_2.name);
			}
			else
			{
				Debug.LogError("ConversationManager is null");
			}

		}

		private void ChangeSpriteOn(Image image, int index, bool isOn)
		{
			if(isOn)
			{
				image.sprite = Sprite_On[index];
			}
			else
			{
				image.sprite = Sprite_Off[index];
			}
		}
	}
}
