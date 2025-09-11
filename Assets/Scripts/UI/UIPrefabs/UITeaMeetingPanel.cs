using UnityEngine;
using UnityEngine.UI;
using QFramework;
using DialogueEditor;
using TMPro;
using FSM;
using System.Collections.Generic; // Added for List
using ExcelDataReader;
using System.IO; // Added for Path, File, FileMode

namespace QFramework.Example
{
    public class UITeaMeetingPanelData : UIPanelData
    {
    }
    public partial class UITeaMeetingPanel : UIPanel
    {
        [SerializeField] private NPCConversation TeaMeetingConversation_1;
        [SerializeField] private NPCConversation TeaMeetingConversation_2;
        [SerializeField] private NPCConversation TeaMeetingConversation_3;
        [SerializeField] private NPCConversation BlackTeaConversation_1;
        [SerializeField] private NPCConversation BlackTeaConversation_2;
        [SerializeField] private NPCConversation BlackTeaConversation_3;
        [SerializeField] private NPCConversation AddConversation_1;
        [SerializeField] private NPCConversation AddConversation_2;
        [SerializeField] private NPCConversation AddConversation_3;
        [SerializeField] private NPCConversation AddConversation_4;
        [SerializeField] private NPCConversation AddConversation_5;
        [SerializeField] private NPCConversation AddConversation_6;
        [SerializeField] private NPCConversation AddConversation_7;
        [SerializeField] private NPCConversation AddConversation_8;
        [SerializeField] private NPCConversation AddConversation_9;
        [SerializeField] private NPCConversation AddConversation_10;
        [SerializeField] private NPCConversation AddConversation_11;
        [SerializeField] private NPCConversation AddConversation_12;
        [SerializeField] private NPCConversation AddConversation_13;
        [SerializeField] private NPCConversation AddConversation_14;
        [SerializeField] private NPCConversation AddConversation_15;
        [SerializeField] private NPCConversation AddConversation_16;

        [SerializeField] private Animator[] animatorObjects;

        public Machine FsmManager;
        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UITeaMeetingPanelData ?? new UITeaMeetingPanelData();
            // please add init code here
            TeaMeetingConversation_1 = DialogueManager.Instance.Conversations[1];
            TeaMeetingConversation_2 = DialogueManager.Instance.Conversations[2];
            TeaMeetingConversation_3 = DialogueManager.Instance.Conversations[3];
            BlackTeaConversation_1 = DialogueManager.Instance.Conversations[4];
            BlackTeaConversation_2 = DialogueManager.Instance.Conversations[5];
            BlackTeaConversation_3 = DialogueManager.Instance.Conversations[6];
            AddConversation_1 = DialogueManager.Instance.Conversations[16];
            AddConversation_2 = DialogueManager.Instance.Conversations[17];
            AddConversation_3 = DialogueManager.Instance.Conversations[18];
            AddConversation_4 = DialogueManager.Instance.Conversations[19];
            AddConversation_5 = DialogueManager.Instance.Conversations[20];
            AddConversation_6 = DialogueManager.Instance.Conversations[21];
            AddConversation_7 = DialogueManager.Instance.Conversations[22];
            AddConversation_8 = DialogueManager.Instance.Conversations[23];
            AddConversation_9 = DialogueManager.Instance.Conversations[24];
            AddConversation_10 = DialogueManager.Instance.Conversations[25];
            AddConversation_11 = DialogueManager.Instance.Conversations[26];
            AddConversation_12 = DialogueManager.Instance.Conversations[27];
            AddConversation_13 = DialogueManager.Instance.Conversations[28];
            AddConversation_14 = DialogueManager.Instance.Conversations[29];
            AddConversation_15 = DialogueManager.Instance.Conversations[30];
            AddConversation_16 = DialogueManager.Instance.Conversations[31];

            // 设置当前步骤
            Global.CurrentStep.Value = 2;
            ConversationManager.Instance.EndConversation();

            FsmManager = FindObjectOfType<Machine>();
            // if(SceneManager.Instance.GetCurrentExtraSceneName()!="XD")
            // {
            // 	SceneManager.Instance.LoadExtraScene("XD");
            // }
            Debug.Log("TimeLineManager.Instance.GetCurrentSceneName(): " + TimeLineManager.Instance.GetCurrentSceneName());
            if (TimeLineManager.Instance.GetCurrentSceneName() != "XD")
            {
                TimeLineManager.Instance.LoadScene("XD");
            }

            TimeLineManager.Instance.ChangeToState("State-视角转换_1");

            Debug.Log("TimeLineManager.Instance.GetCurrentSceneName(): " + TimeLineManager.Instance.GetCurrentSceneName());

            SceneMoveManager.Instance.TransferImmediately(1);

            if (animatorObjects == null || animatorObjects.Length == 0)
            {
                // 直接查找所有Animator组件，包括被禁用的对象
                Animator[] allAnimators = GameObject.FindObjectsOfType<Animator>(true); // true表示包括被禁用的对象
                Debug.Log("allAnimators.Length:" + allAnimators.Length);

                animatorObjects = new Animator[4];
                foreach (Animator animator in allAnimators)
                {
                    if (animator.gameObject.name == "王国信")
                    {
                        animatorObjects[0] = animator;
                        animator.gameObject.SetActive(true);
                    }
                    if (animator.gameObject.name == "茉 莉")
                    {
                        animatorObjects[1] = animator;
                        animator.gameObject.SetActive(true);
                    }
                    else if (animator.gameObject.name == "约翰逊")
                    {
                        animatorObjects[2] = animator;
                        animator.gameObject.SetActive(true);
                    }
                    else if (animator.gameObject.name == "李文俊")
                    {
                        animatorObjects[3] = animator;
                        animator.gameObject.SetActive(true);
                    }
                    Debug.Log("find animator:" + animator.gameObject.name);

                    // 将动画设置到最后一帧以确保模型位置正确
                    animator.Play("谈判交谈", 0, 0.99f); // normalizedTime = 1.0 表示最后一帧
                    animator.Update(0f); // 立即更新动画状态
                    animator.enabled = false;
                    Debug.Log("animator.Play:谈判交谈" + animator.gameObject.name);
                }

            }

            if (ConversationManager.Instance != null)
            {
                ConversationManager.Instance.StartConversation(TeaMeetingConversation_1);
                Debug.Log("play conversation:" + TeaMeetingConversation_1.name);
            }
            else
            {
                Debug.LogError("ConversationManager is null");
            }

            OnClickButton();
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

            PanelManager.Instance.OpenPanel_DialogueJournal();

            foreach (var animator in animatorObjects)
            {
                animator.Play("谈判交谈", 0, 0.99f); // normalizedTime = 1.0 表示最后一帧
                animator.Update(0f); // 立即更新动画状态
                animator.enabled = false;
            }
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
        }

        public void playTalkingAnimation(string conversationName, int nodeId)
        {
            string characterName = "";

            // 读取对应 excel 的对应行，并添加到对话记录中
            string excelDir = Path.Combine(Application.streamingAssetsPath, "Excel");
            string xlsx = Path.Combine(excelDir, "DialogueJournal", conversationName + ".xlsx");
            string targetPath = File.Exists(xlsx) ? xlsx : null;
            if (string.IsNullOrEmpty(targetPath)) return;

            using (var stream = File.Open(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                // 跳过表头
                if (!reader.Read()) return;
                // 前进到第 nodeId 条数据行（0 基），读 nodeId+1 次
                for (int i = 0; i <= nodeId; i++)
                {
                    if (!reader.Read()) return;
                }
                characterName = SafeGetString(reader, 0);

                if (characterName == "选项" | characterName == "结局")
                    return;

                Debug.Log("playTalkingAnimation" + characterName);
                playTalkingAnimation(characterName);
            }
        }

        public void playTalkingAnimation(string characterName)
        {
            if (animatorObjects != null && animatorObjects.Length > 0)
            {
                Animator animator = null;
                if (characterName == "王国信")
                {
                    animator = animatorObjects[0];
                }
                else if (characterName == "茉 莉")
                {
                    animator = animatorObjects[1];
                }
                else if (characterName == "约翰逊")
                {
                    animator = animatorObjects[2];
                }
                else if (characterName == "李文俊")
                {
                    animator = animatorObjects[3];
                }

                if (animator != null)
                {
                    // 播放交谈动画
                    animator.enabled = true;
                    animator.Play("谈判交谈", 0, 0f); // 从头开始播放
                    Debug.Log("播放交谈动画:" + animator.gameObject.name);
                }

            }
            else
            {
                Debug.LogError("playTalkingAnimation: animatorObjects为null或空，无法播放动画");
            }
        }

        private void OnClickButton()
        {
            //Btn_NextPage.onClick.AddListener(OnClickNextPage);
            Btn_Next_1.onClick.AddListener(OnClickNext_1);
            Btn_NextModule.onClick.AddListener(onClickNextModule);

            foreach (var node in TeaMeetingConversation_1.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(TeaMeetingConversation_1.name, node.NodeID));
                if (node.NodeID == 0)
                {
                    node.Event.AddListener(() => TriggerSelfAction(1));
                }
                if (node.NodeID == 2)
                {
                    node.Event.AddListener(ChangeToState_MakeTea);
                }
            }
            foreach (var node in TeaMeetingConversation_2.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(TeaMeetingConversation_2.name, node.NodeID));
                if (node.NodeID == 30)
                {
                    node.Event.AddListener(ChangeToState_ExtraTea);
                }
                if (node.NodeID == 36)
                {
                    node.Event.AddListener(EnterNextConversation);
                }
            }
            foreach (var node in TeaMeetingConversation_3.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(TeaMeetingConversation_3.name, node.NodeID));
                if (node.NodeID == 4)
                {
                    node.Event.AddListener(EnterNextConversation);
                }
            }
            foreach (var node in BlackTeaConversation_1.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(BlackTeaConversation_1.name, node.NodeID));
                if (node.NodeID == 87)
                {
                    node.Event.AddListener(ExtraSceneConversation);
                }
                if (node.NodeID == 92)
                {
                    node.Event.AddListener(NextPanel);
                    node.Event.AddListener(() => TriggerSelfAction(4));
                }
            }
            foreach (var node in BlackTeaConversation_2.GetComponentsInChildren<NodeEventHolder>())
            {
                //node.Event.AddListener(()=>playTalkingAnimation(BlackTeaConversation_2.name,node.NodeID));
                if (node.NodeID == 3)
                {
                    node.Event.AddListener(ReturnConversation);
                }
            }
            foreach (var node in BlackTeaConversation_3.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(BlackTeaConversation_3.name, node.NodeID));
                if (node.NodeID == 13)
                {
                    node.Event.AddListener(ExtraSceneConversation);
                }
                if (node.NodeID == 18)
                {
                    node.Event.AddListener(NextPanel);
                    node.Event.AddListener(() => TriggerSelfAction(6));
                }
            }
            foreach (var node in AddConversation_1.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(AddConversation_1.name, node.NodeID));
                if (node.NodeID == 4)
                {
                    node.Event.AddListener(() => TriggerSelfAction(16));
                }
            }
            foreach (var node in AddConversation_2.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(AddConversation_2.name, node.NodeID));
                if (node.NodeID == 1)
                {
                    node.Event.AddListener(() => TriggerSelfAction(17));
                }
            }
            foreach (var node in AddConversation_3.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(AddConversation_3.name, node.NodeID));
                if (node.NodeID == 1)
                {
                    node.Event.AddListener(() => TriggerSelfAction(18));
                }
            }
            foreach (var node in AddConversation_4.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(AddConversation_4.name, node.NodeID));
                if (node.NodeID == 1)
                {
                    node.Event.AddListener(() => TriggerSelfAction(19));
                }
            }
            foreach (var node in AddConversation_5.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(AddConversation_5.name, node.NodeID));
                if (node.NodeID == 1)
                {
                    node.Event.AddListener(() => TriggerSelfAction(20));
                }
            }
            foreach (var node in AddConversation_6.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(AddConversation_6.name, node.NodeID));
                if (node.NodeID == 1)
                {
                    node.Event.AddListener(() => TriggerSelfAction(21));
                }
            }
            foreach (var node in AddConversation_7.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(AddConversation_7.name, node.NodeID));
                if (node.NodeID == 1)
                {
                    node.Event.AddListener(() => TriggerSelfAction(22));
                }
            }
            foreach (var node in AddConversation_8.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(AddConversation_8.name, node.NodeID));
                if (node.NodeID == 1)
                {
                    node.Event.AddListener(() => TriggerSelfAction(23));
                }
            }
            foreach (var node in AddConversation_9.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(AddConversation_9.name, node.NodeID));
                if (node.NodeID == 1)
                {
                    node.Event.AddListener(() => TriggerSelfAction(24));
                }
            }
            foreach (var node in AddConversation_10.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(AddConversation_10.name, node.NodeID));
                if (node.NodeID == 1)
                {
                    node.Event.AddListener(() => TriggerSelfAction(25));
                }
            }
            foreach (var node in AddConversation_11.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(AddConversation_11.name, node.NodeID));
                if (node.NodeID == 1)
                {
                    node.Event.AddListener(() => TriggerSelfAction(26));
                }
            }
            foreach (var node in AddConversation_12.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(AddConversation_12.name, node.NodeID));
                if (node.NodeID == 1)
                {
                    node.Event.AddListener(() => TriggerSelfAction(27));
                }
            }
            foreach (var node in AddConversation_13.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(AddConversation_13.name, node.NodeID));
                if (node.NodeID == 1)
                {
                    node.Event.AddListener(() => TriggerSelfAction(28));
                }
            }
            foreach (var node in AddConversation_14.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(AddConversation_14.name, node.NodeID));
                if (node.NodeID == 3)
                {
                    node.Event.AddListener(() => TriggerSelfAction(29));
                }
            }
            foreach (var node in AddConversation_15.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(AddConversation_15.name, node.NodeID));
                if (node.NodeID == 1)
                {
                    node.Event.AddListener(() => TriggerSelfAction(30));
                }
            }
            foreach (var node in AddConversation_16.GetComponentsInChildren<NodeEventHolder>())
            {
                node.Event.AddListener(() => playTalkingAnimation(AddConversation_16.name, node.NodeID));
                if (node.NodeID == 5)
                {
                    node.Event.AddListener(() => TriggerSelfAction(31));
                }
            }
        }
        private void OnClickNext_1()
        {
            Tip_SelectError.Hide();
        }


        public void ChangeToState_MakeTea()
        {
            foreach (var animator in animatorObjects)
            {
                animator.enabled = true;
            }
            FsmManager.ChangeToStateByName("State-泡茶");
            TimeLineManager.Instance.ChangeToState("State-泡茶");
        }

        public void ChangeToState_ExtraTea()
        {
            FsmManager.ChangeToStateByName("State-新茶");
            TimeLineManager.Instance.ChangeToState("State-新茶");
        }

        public void EnterNextConversation()
        {
            Btn_NextModule.gameObject.SetActive(true);
        }

        public void ExtraSceneConversation()
        {
            //AnimationManager.Instance.PauseTimeline(1);
            //SceneManager.Instance.LoadExtraScene("JD");
            TimeLineManager.Instance.LoadScene("JD");
            FsmManager.ChangeToStateByName("State-额外交谈");
            // animatorObjects[1].Play("Default", 0, 0);
            // animatorObjects[1].Update(0f);
            // animatorObjects[1].enabled = false;
            // animatorObjects[2].Play("Default", 0, 0);
            // animatorObjects[2].Update(0f);
            // animatorObjects[2].enabled = false;

            Debug.Log("ExtraSceneConversation");
            //AnimationManager.Instance.PauseTimeline(1);
            //System.Threading.Thread.Sleep(1);//暂停一秒
            //ConversationManager.Instance.StartConversation(BlackTeaConversation_3);
        }

        public void ReturnConversation()
        {
            //SceneManager.Instance.LoadExtraScene("XD");
            // animatorObjects[1].enabled = true;
            // animatorObjects[2].enabled = true;

            TimeLineManager.Instance.LoadScene("XD");
            //TimeLineManager.Instance.ChangeToState("State-视角转换_1");
            FsmManager.ChangeToStateByName("State-静态坐");
            TimeLineManager.Instance.ChangeToState("State-静态坐");
            SceneMoveManager.Instance.TransferImmediately(1);
            //ConversationManager.Instance.StartConversation(BlackTeaConversation_3);
            //TimeLineManager.Instance.ChangeToState("State-静态坐");
        }

        public void onClickNextModule()
        {
            Debug.Log("onClickNextModule");
            Btn_NextModule.gameObject.SetActive(false);
            if (ConversationManager.Instance != null)
            {
                ConversationManager.Instance.StartConversation(BlackTeaConversation_1);
                Debug.Log("play conversation:" + BlackTeaConversation_1.name);
                Debug.Log("ConversationManager.Instance.IsConversationActive:" + ConversationManager.Instance.IsConversationActive);
            }
            else
            {
                Debug.LogError("ConversationManager is null");
            }
        }
        #region LaJiDaima
        public void TriggerSelfAction(int index)
        {
            DialogueManager.Instance.Conversations[index].GetComponent<ClickableObject>().TriggerAction();
        }
        #endregion

        public void NextPanel()
        {
            UIKit.ClosePanel<UITeaMeetingPanel>();
            SceneManager.Instance.UnloadCurrentScene();
            UIKit.OpenPanel<UIExtraInvitationPanel>(UILevel.Common, null, null, "UIPrefabs/UIExtraInvitationPanel");
        }

        /// <summary>
        /// 安全获取Excel单元格的字符串值
        /// </summary>
        private string SafeGetString(IExcelDataReader reader, int index)
        {
            if (index < 0 || index >= reader.FieldCount) return string.Empty;
            try
            {
                return reader.GetValue(index)?.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }


}