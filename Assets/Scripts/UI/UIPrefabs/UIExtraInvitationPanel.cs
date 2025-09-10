using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;
using FSM;
using DialogueEditor;

namespace QFramework.Example
{
	public class UIExtraInvitationPanelData : UIPanelData
	{
	}
	public partial class UIExtraInvitationPanel : UIPanel
	{
        
		public Machine FsmManager;
		protected override void OnInit(IUIData uiData = null)
		{
            UIKit.ClosePanel<UIDialogueJournalPanel>();
			mData = uiData as UIExtraInvitationPanelData ?? new UIExtraInvitationPanelData();

            // 设置当前步骤
			Global.CurrentStep.Value =3;
			ConversationManager.Instance.EndConversation();

			// please add init code here
			FsmManager = FindObjectOfType<Machine>();
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
			Btn_Skip_1.onClick.AddListener(onClickSkip_1);
		}

		private void OnClickNext_1()
		{
			UIKit.ClosePanel<UIExtraInvitationPanel>();
			UIKit.OpenPanel<UITipPanel_FirstMeeting>(UILevel.Common, null, null, "UIPrefabs/UITipPanel_FirstMeeting");
            PanelManager.Instance.OpenPanel_DialogueJournal();
            FsmManager.ChangeToStateByName("State-他方邀约会面");

            if(TimeLineManager.Instance.GetCurrentSceneName()!="GC")
            {    
                //UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(TimeLineManager.Instance.GetCurrentSceneName());
                TimeLineManager.Instance.LoadScene("GC");
            }
            TimeLineManager.Instance.ChangeToState("State-他方邀约会面");
		}

		private void OnClickSubmit_1()
		{
			Debug.Log("OnClickSubmit_1");
			Btn_Submit_1.gameObject.SetActive(false);
			Btn_Next_1.gameObject.SetActive(true);

            TMP_InputField[] inputs = new TMP_InputField[4];
            for(int i = 0; i < 4; i++)
            {
                inputs[i] = InputFileds.transform.GetChild(i).GetChild(1).GetComponent<TMP_InputField>();
            }

            Debug.Log("inputs.Length:"+inputs.Length);
            for(int i = 0; i < inputs.Length; i++)
            {
                Debug.Log("inputs[i].text:"+inputs[i].text);
                switch(i)
                {
                    case 0:
                        if(inputs[i].text != "约翰逊")
                        {    
                            inputs[i].textComponent.color = Color.red;
                            inputs[i].text="约翰逊";

                            Global.ScoreList[13] += 0;
                        }
                        else
                        {    inputs[i].textComponent.color = Color.green;
                            Global.ScoreList[13] += 2;}
                        break;
                    case 1:
                        if(inputs[i].text != "2025年8月24日18:30")
                        {
                            inputs[i].textComponent.color = Color.red;
                            inputs[i].text="2025年8月24日18:30";

                            Global.ScoreList[13] += 0;
                        }
                        else    
                        {    inputs[i].textComponent.color = Color.green;
                            Global.ScoreList[13] += 2;}
                        break;
                    case 2:
                        if(inputs[i].text != "沿江西一路5号")
                        {
                            inputs[i].textComponent.color = Color.red;
                            inputs[i].text="沿江西一路5号";

                            Global.ScoreList[13] += 0;
                        }
                        else
                        {    inputs[i].textComponent.color = Color.green;
                            Global.ScoreList[13] += 2;}
                        break;
                    case 3:
                        if(inputs[i].text != "李天然")
                        {
                            inputs[i].textComponent.color = Color.red;
                            inputs[i].text="李天然";

                            Global.ScoreList[13] += 0;
                        }
                        else
                        {    inputs[i].textComponent.color = Color.green;
                            Global.ScoreList[13] += 2;}
                        break;
                }
            }
		}

		private void onClickSkip_1()
		{
			UIKit.ClosePanel<UIExtraInvitationPanel>();
			UIKit.OpenPanel<UITipPanel_SignAgreement>(UILevel.Common, null, null, "UIPrefabs/UITipPanel_SignAgreement");
		}
	}
}
