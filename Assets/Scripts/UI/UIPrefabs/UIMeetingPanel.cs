using UnityEngine;
using UnityEngine.UI;
using QFramework;
using DialogueEditor;
using FSM;
using UnityEngine.Playables;
using System.Collections;

namespace QFramework.Example
{
	public class UIMeetingPanelData : UIPanelData
	{
	}
	public partial class UIMeetingPanel : UIPanel
	{

		[SerializeField] private NPCConversation NiceToMeetYou;
		public Machine FsmManager;
		[SerializeField]
		public GameObject TimeLine;
		public PlayableDirector director;
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIMeetingPanelData ?? new UIMeetingPanelData();
			// please add init code here
			NiceToMeetYou=DialogueManager.Instance.Conversations[0];

			InitPage();
			OnClickButton();

			FsmManager = FindObjectOfType<Machine>();

			if(TimeLine==null)
			{
				TimeLine = FindObjectOfType<TimelineController>().gameObject;
				Debug.Log("Found TimelineController and assigned TimeLine object");
				Debug.Log("TimeLine: " + TimeLine.name);
			}
			if(director==null)
			{
				director = TimeLine.GetComponent<PlayableDirector>();
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

		private void InitPage()
		{
			Tips_Handshake.Hide();
			Tips_Greetings.Hide();
			Page_Greetings.Show();
			Tips_Introduction.Show();
			Btn_NextPage.Hide();
		}

		private void OnClickButton()
		{
			//TitlePanel.OnBackButtonClick =OnClickLastButton;
			Btn_Check.onClick.AddListener(OnClickCheckButton);
			Btn_Next.onClick.AddListener(OnClickNextButton);
			Btn_NextPage.onClick.AddListener(OnClickNextPageButton);

			foreach(var node in NiceToMeetYou.GetComponentsInChildren<NodeEventHolder>())
			{
				if(node.NodeID==2)
				{
					node.Event.AddListener(DialogueEndEvent);
				}
			}
		}

		private void OnClickLastButton()
		{
			SceneManager.Instance.UnloadCurrentScene();
			UIKit.ClosePanel<UIMeetingPanel>();
			UIKit.OpenPanel<UIInvitedPanel>(UILevel.Common, null, null, "UIPrefabs/UIInvitedPanel");
		}

		private void OnClickCheckButton()
		{
			if(Tog_Handshake.isOn)
			{
				Img_Correct.gameObject.SetActive(true);
				Img_Error.gameObject.SetActive(false);

				Global.ScoreList[4] = 4;
			}
			else
			{
				Img_Correct.gameObject.SetActive(false);
				Img_Error.gameObject.SetActive(true);

				Global.ScoreList[4] = 0;
			}
			Tog_Handshake.isOn=true;
			Tog_Embrace.isOn=false;
			
			Btn_Next.gameObject.SetActive(true);
			Btn_Check.gameObject.SetActive(false);
		}

		private void OnClickNextButton()
		{
			Page_Greetings.Hide();
			Tips_Introduction.Hide();
			Tips_Handshake.Show();

			//SceneManager.Instance.LoadExtraScene("XD");
			TimeLineManager.Instance.LoadScene("XD");
			Debug.Log("TimeLineManager.Instance.GetCurrentSceneName(): " + TimeLineManager.Instance.GetCurrentSceneName());
			TimeLineManager.Instance.ChangeToState("State-握手");
			FsmManager.ChangeToStateByName("State-握手");

			SceneMoveManager.Instance.TransferImmediately(0);
		}

		
		private void OnClickNextPageButton()
		{
            UIKit.OpenPanel<UITeaMeetingPanel>(UILevel.Common, null, null, "UIPrefabs/UITeaMeetingPanel");
			UIKit.ClosePanel<UIMeetingPanel>();

			AnimationManager.Instance.EndInspection();

			SceneMoveManager.Instance.TransferImmediately(1);
			TimeLineManager.Instance.ChangeToState("State-视角转换_1");

            //关闭检视
            UIKit.ClosePanel<UICheckModelPanel>();
			if(GameObject.Find("泡茶-茶杯1_InspectionArea_Stable") != null)
				Destroy(GameObject.Find("泡茶-茶杯1_InspectionArea_Stable"));
            if (GameObject.Find("泡茶-公道杯_InspectionArea_Stable") != null)
                Destroy(GameObject.Find("泡茶-公道杯_InspectionArea_Stable"));
        }
	
		private void DialogueEndEvent()
		{
			Tips_Handshake.Hide();
			Tips_Greetings.Show();
			Btn_NextPage.Show();

			FsmManager.ChangeToStateByName("State-入座");
			TimeLineManager.Instance.ChangeToState("State-入座");
			ObjectHoverManager.Instance.SetTargetTag("TeaSet");

			SceneMoveManager.Instance.TransferImmediately(1);
		}
	}
}
