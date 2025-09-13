using UnityEngine;
using UnityEngine.UI;
using QFramework;
using DialogueEditor;
using FSM;

namespace QFramework.Example
{
	public class UITipPanel_SignAgreementData : UIPanelData
	{
	}
	public partial class UITipPanel_SignAgreement : UIPanel
	{
		[SerializeField] private NPCConversation Conversation_SalesContract_1;
		[SerializeField] private NPCConversation Conversation_SalesContract_2;
		[SerializeField] private NPCConversation Conversation_SalesContract_Random_1;
		[SerializeField] private NPCConversation Conversation_SalesContract_Random_2;
		[SerializeField] private NPCConversation Conversation_SalesContract_3;
		[SerializeField] private NPCConversation Conversation_SalesContract_4;
		[SerializeField] private NPCConversation Conversation_SalesContract_5;

		public Machine FsmManager;

		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UITipPanel_SignAgreementData ?? new UITipPanel_SignAgreementData();
			// please add init code here

			Conversation_SalesContract_1=DialogueManager.Instance.Conversations[9];
			Conversation_SalesContract_2=DialogueManager.Instance.Conversations[10];
			Conversation_SalesContract_3=DialogueManager.Instance.Conversations[13];
			Conversation_SalesContract_4=DialogueManager.Instance.Conversations[14];
			Conversation_SalesContract_5=DialogueManager.Instance.Conversations[15];
			Conversation_SalesContract_Random_1=DialogueManager.Instance.Conversations[11];
			Conversation_SalesContract_Random_2=DialogueManager.Instance.Conversations[12];

			// 在OnInit中初始化随机数，而不是在字段初始化时

			// 设置当前步骤
			Global.CurrentStep.Value = 4;
			ConversationManager.Instance.EndConversation();

			FsmManager = FindObjectOfType<Machine>();

            // if(SceneManager.Instance.GetCurrentExtraSceneName()!="XD")
            // {
            // 	SceneManager.Instance.LoadExtraScene("XD");

            // 	if(TimeLineManager.Instance.GetCurrentSceneName()=="GC")
            // 	{
            // 		TimeLineManager.Instance.UnloadScene("GC");
            // 	}
            // }
            if (TimeLineManager.Instance.GetCurrentSceneName() == "XD")
            {
                TimeLineManager.Instance.UnloadScene("XD");
            }
            TimeLineManager.Instance.LoadScene("XD");

			if(ConversationManager.Instance!=null)
			{
				SceneMoveManager.Instance.TransferImmediately(1);
				FsmManager.ChangeToStateByName("State-销售协议传递");
				ConversationManager.Instance.StartConversation(Conversation_SalesContract_1);
				PanelManager.Instance.OpenPanel_DialogueJournal();
				Debug.Log("play conversation:"+Conversation_SalesContract_1.name);
			}
			else
			{
				Debug.LogError("ConversationManager is null");
			}

			OnListener();
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
			AnimationManager.Instance.ActivatePerson("John");
			AnimationManager.Instance.ActivatePerson("MoLi");
			AnimationManager.Instance.ActivatePerson("WangGuoXin");
			AnimationManager.Instance.ActivatePerson("LiWenJun");
			AnimationManager.Instance.DeactivatePerson("LiTianRan");

			//FsmManager.ChangeToStateByName("State-早茶静态坐");
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}

		private void OnListener()
		{
            foreach (var node in Conversation_SalesContract_1.GetComponentsInChildren<NodeEventHolder>())
            {
				if(node.NodeID == 0){
					GameObject.FindObjectOfType<TimelineController>().PlayTimelineAtTimeAndPauseNextFrame(545f);
                }
                if (node.NodeID == 1)
                {
                    node.Event.AddListener(() => TriggerSelfAction(9));
                }
            }

            foreach (var node in Conversation_SalesContract_2.GetComponentsInChildren<NodeEventHolder>())
			{
				if(node.NodeID==0)
				{
					node.Event.AddListener(StartSalesContract);
				}
				if(node.NodeID==2)
				{
					//node.Event.AddListener(StartRandomConversation1);
				}
			}

			foreach(var node in Conversation_SalesContract_Random_1.GetComponentsInChildren<NodeEventHolder>())
			{
				if(node.NodeID==3)
				{
					node.Event.AddListener(FixContract);
				}
			}

			foreach(var node in Conversation_SalesContract_Random_2.GetComponentsInChildren<NodeEventHolder>())
			{
				if(node.NodeID==2)
				{
					node.Event.AddListener(FixContract);
				}
			}

			foreach(var node in Conversation_SalesContract_5.GetComponentsInChildren<NodeEventHolder>())
			{
				if(node.NodeID==0)
				{
					node.Event.AddListener(EndConversation);
				}
			}

			//Btn_Next.onClick.AddListener(StartRandomConversation2);
		}

		private void StartRandomConversation1()
		{
			Btn_Next.gameObject.SetActive(true);
		}

		private void FixContract()
		{
			FsmManager.ChangeToStateByName("State-销售协议错误");
		}

		private void StartSalesContract()
		{
			FsmManager.ChangeToStateByName("State-销售协议传递2");
		}

		private void EndConversation()
		{
			FsmManager.ChangeToStateByName("State-早茶");
		}

        #region LaJiDaima
        public void TriggerSelfAction(int index)
        {
            DialogueManager.Instance.Conversations[index].GetComponent<ClickableObject>().TriggerAction();
        }
        #endregion
    }
}
