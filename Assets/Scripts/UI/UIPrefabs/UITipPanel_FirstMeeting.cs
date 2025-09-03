using UnityEngine;
using UnityEngine.UI;
using QFramework;
using DialogueEditor;
using FSM;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

namespace QFramework.Example
{
	public class UITipPanel_FirstMeetingData : UIPanelData
	{
	}
	public partial class UITipPanel_FirstMeeting : UIPanel
	{
		[SerializeField] private NPCConversation ExtraInvitationConversation_1;
		[SerializeField] private NPCConversation ExtraInvitationConversation_2;

		public Machine FsmManager;

		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UITipPanel_FirstMeetingData ?? new UITipPanel_FirstMeetingData();
			// please add init code here

			ExtraInvitationConversation_1=DialogueManager.Instance.Conversations[7];
			ExtraInvitationConversation_2=DialogueManager.Instance.Conversations[8];

			FsmManager = FindObjectOfType<Machine>();

			if(SceneManager.Instance.GetCurrentExtraSceneName()!="GC")
			{
				//SceneManager.Instance.LoadExtraScene("GC");
			}

			// if(ConversationManager.Instance!=null)
			// {
			// 	ConversationManager.Instance.StartConversation(ExtraInvitationConversation_1);
			// 	Debug.Log("play conversation:"+ExtraInvitationConversation_1.name);
			// }
			// else
			// {
			// 	Debug.LogError("ConversationManager is null");
			// }

			foreach(var node in ExtraInvitationConversation_1.GetComponentsInChildren<NodeEventHolder>())
			{
				if(node.NodeID==2)
				{
					node.Event.AddListener(ProductIntroduction);
				}

				if(node.NodeID==10)
				{
					node.Event.AddListener(OnQuestionBegin);
				}
			}

			foreach(var node in ExtraInvitationConversation_2.GetComponentsInChildren<NodeEventHolder>())
			{
				if(node.NodeID==6)
				{
					node.Event.AddListener(()=>Btn_Next.gameObject.SetActive(true));
				}
			}

			Btn_Next.onClick.AddListener(NextModule);
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


				
			//Debug.Log("PlayTimeline");
			//PlayTimeline();
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}

		private void ProductIntroduction()
		{
			FsmManager.ChangeToStateByName("State-工厂参观");

			TimeLineManager.Instance.ChangeToState("State-工厂参观");

		}

		private void OnQuestionBegin()
		{
			UIKit.OpenPanel<UIMarketEnvironmentPanel>(UILevel.Common, null, null, "UIPrefabs/UIMarketEnvironmentPanel");
		}

		private void NextModule()
		{
			UIKit.ClosePanel<UITipPanel_FirstMeeting>();
			UIKit.OpenPanel<UITipPanel_SignAgreement>(UILevel.Common, null, null, "UIPrefabs/UITipPanel_SignAgreement");
			TimeLineManager.Instance.UnloadScene("GC");
		}

		private void OnClickSubmit()
		{
			
		}

		private void OnClickNext()
		{
			EndChoose();
		}

		private void EndChoose()
		{
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
	}
}
