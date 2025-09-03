using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;
using DialogueEditor;

namespace QFramework.Example
{
	public class UIMakeTeaPanelData : UIPanelData
	{
	}
	public partial class UIMakeTeaPanel : UIPanel
	{
		[SerializeField] private NPCConversation Conversation_TeaMeeting_2;

		private Button[] Btn_Steps;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIMakeTeaPanelData ?? new UIMakeTeaPanelData();
			// please add init code here

			Btn_Steps = new Button[9];
			for(int i = 0; i < 9; i++)
			{
				Btn_Steps[i] = Text_Left.transform.GetChild(i).GetComponent<Button>();
			}
			Conversation_TeaMeeting_2=DialogueManager.Instance.Conversations[2];
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

			for(int i = 0; i < Btn_Steps.Length; i++)
			{
				int index = i;
				Btn_Steps[i].onClick.AddListener(()=>OnClickStep(index));
			}
		}

		private void OnClickStep(int i)
		{
			Page_Tips.transform.GetChild(i).gameObject.SetActive(true);
		}

		private void OnClickNext()
		{
			UIKit.ClosePanel<UIMakeTeaPanel>();
			ConversationManager.Instance.StartConversation(Conversation_TeaMeeting_2);
		}

		private void OnClickSubmit()
		{
            Debug.Log("OnClickSubmit");
			Btn_Submit.gameObject.SetActive(false);
			Btn_Next.gameObject.SetActive(true);

            TMP_InputField[] inputs = new TMP_InputField[9];
            for(int i = 0; i < 9; i++)
            {
                inputs[i] = InputFields.transform.GetChild(i).GetChild(1).GetComponent<TMP_InputField>();
            }
			TMP_Text[] answers = new TMP_Text[9];
			for(int i = 0; i < 9; i++)
			{
				answers[i] = InputFields.transform.GetChild(i).GetChild(2).GetComponent<TMP_Text>();
			}

            Debug.Log("inputs.Length:"+inputs.Length);
            for(int i = 0; i < inputs.Length; i++)
            {
                Debug.Log("inputs[i].text:"+inputs[i].text);
				Debug.Log("answers[i].text:"+answers[i].text);
				if(inputs[i].text != answers[i].text)
				{
					inputs[i].textComponent.color = Color.red;
					inputs[i].text=answers[i].text;

					Global.ScoreList[5] += 0;
				}
				else
				{
					inputs[i].textComponent.color=Color.green;
					Global.ScoreList[5] += 1;
				}
            }
		}
	}
}
