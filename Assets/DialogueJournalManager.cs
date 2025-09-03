using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

namespace QFramework.Example
{
    [MonoSingletonPath("DialogueJournalManager")]
    public class DialogueJournalManager : MonoBehaviour, ISingleton
    {
        [SerializeField]
        private List<DialogueJournal> dialogueJournalList_CN = new List<DialogueJournal>();
        [SerializeField]
        private List<DialogueJournal> dialogueJournalList_EN = new List<DialogueJournal>();

        // 为每个对话分别维护节点ID列表，避免不同对话之间的干扰
        private readonly Dictionary<string, List<int>> conversationNodeIdsMap = new Dictionary<string, List<int>>();

        private List<NPCConversation> conversations;

        // 缓存所有对话的ScriptableObject数据
        private DialogueJournalData[] dialogueJournalDataArray;
        private readonly Dictionary<string, DialogueJournalData> dialogueDataCache = new Dictionary<string, DialogueJournalData>();

        private int currentConversationId = 0;
        private int currentNodeId = 0;

        // 开机自动初始化（非懒加载）
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoInit()
        {
            var _ = Instance;
        }

        void ISingleton.OnSingletonInit()
        {
            Debug.Log("DialogueJournalManager OnSingletonInit");
            // 预加载所有对话的ScriptableObject数据
            LoadAllDialogueData();
            
            // 延迟到 DialogueManager 就绪后再订阅，避免空引用
            StartCoroutine(SetupListenersWhenReady());
        }

        private IEnumerator SetupListenersWhenReady()
        {
            // 等待 DialogueManager 实例与会话列表可用
            while (DialogueManager.Instance == null || DialogueManager.Instance.Conversations == null)
                yield return null;

            conversations = DialogueManager.Instance.Conversations;

            foreach (var conversation in conversations)
            {
                var convName = conversation.name;
                // 为每个对话初始化节点ID列表
                conversationNodeIdsMap[convName] = new List<int>();
                
                foreach (var holder in conversation.GetComponentsInChildren<NodeEventHolder>(true))
                {
                    int id = holder.NodeID;
                    holder.Event.AddListener(() => AddDialogueJournal(convName, id));
                }
            }
        }

        /// <summary>
        /// 加载所有对话的ScriptableObject数据到缓存中
        /// </summary>
        private void LoadAllDialogueData()
        {
            // 强制刷新资源
            Resources.UnloadUnusedAssets();
            // 清空缓存
            dialogueDataCache.Clear();
            
            // 强制重新加载所有资源，避免缓存问题
            var allData = Resources.LoadAll<DialogueJournalData>("ScriptableObjects/DialogueJournal");
            
            // 创建新的数组，避免引用旧数据
            dialogueJournalDataArray = new DialogueJournalData[allData.Length];
            for (int i = 0; i < allData.Length; i++)
            {
                dialogueJournalDataArray[i] = allData[i];
            }

            if (dialogueJournalDataArray == null || dialogueJournalDataArray.Length == 0)
            {
                Debug.LogError("未找到任何DialogueJournalData文件！请检查ScriptableObjects/DialogueJournal文件夹");
                return;
            }

            int successCount = 0;
            foreach (var data in dialogueJournalDataArray)
            {
                if (data != null && !string.IsNullOrEmpty(data.conversationName))
                {
                    // 检查是否有重复的conversationName
                    if (dialogueDataCache.ContainsKey(data.conversationName))
                    {
                        Debug.LogWarning($"发现重复的对话名称: {data.conversationName}，将覆盖之前的记录");
                    }
                    
                    dialogueDataCache[data.conversationName] = data;
                    successCount++;
                    Debug.Log($"已加载对话数据: {data.conversationName}, 包含 {data.dialogueEntries.Count} 条记录");
                }
                else
                {
                    Debug.LogWarning("发现无效的DialogueJournalData文件，已跳过");
                }
            }
            
            Debug.Log($"预加载完成！成功加载 {successCount}/{dialogueJournalDataArray.Length} 个对话的ScriptableObject数据");
            
            // 输出所有已加载的对话名称，方便调试
            if (dialogueDataCache.Count > 0)
            {
                string[] conversationNames = new string[dialogueDataCache.Keys.Count];
                dialogueDataCache.Keys.CopyTo(conversationNames, 0);
                Debug.Log($"已加载的对话列表: {string.Join(", ", conversationNames)}");
            }
        }

        private void AddDialogueJournal(string conversationName, int nodeId)
        {
            Debug.Log("AddDialogueJournal: " + nodeId);

            // 获取当前对话的节点ID列表
            if (!conversationNodeIdsMap.TryGetValue(conversationName, out var currentConversationNodeIds))
            {
                currentConversationNodeIds = new List<int>();
                conversationNodeIdsMap[conversationName] = currentConversationNodeIds;
            }

            // 如果这是一次回跳（跳到更小的 nodeId），将上一次出现该 nodeId 之后的记录全部标红
            if (currentConversationNodeIds.Count > 0 && nodeId < currentConversationNodeIds[currentConversationNodeIds.Count - 1])
            {
                int targetIndex = -1;
                for (int i = currentConversationNodeIds.Count - 1; i >= 0; i--)
                {
                    if (currentConversationNodeIds[i] <= nodeId)
                    {
                        targetIndex = i;
                        break;
                    }
                }
                if (targetIndex >= 0)
                {
                    // 计算需要标红的记录数量
                    int recordsToMarkRed = currentConversationNodeIds.Count - (targetIndex + 1);
                    // 从对话记录列表的末尾开始，标记对应数量的记录为红色
                    // 同时标记中文和英文列表中的记录
                    for (int i = dialogueJournalList_CN.Count - recordsToMarkRed; i < dialogueJournalList_CN.Count; i++)
                    {
                        if (i >= 0)
                        {
                            dialogueJournalList_CN[i].color = Color.red;
                        }
                    }
                    for (int i = dialogueJournalList_EN.Count - recordsToMarkRed; i < dialogueJournalList_EN.Count; i++)
                    {
                        if (i >= 0)
                        {
                            dialogueJournalList_EN[i].color = Color.red;
                        }
                    }
                }
            }

            if (!dialogueDataCache.TryGetValue(conversationName, out var dialogueData))
            {
                Debug.LogWarning($"未找到对话 {conversationName} 的ScriptableObject数据");
                return;
            }

            // 检查nodeId是否在有效范围内
            if (nodeId < 0 || nodeId >= dialogueData.dialogueEntries.Count)
            {
                Debug.LogWarning($"对话 {conversationName} 的节点ID {nodeId} 超出范围 (0-{dialogueData.dialogueEntries.Count - 1})");
                return;
            }

            var entry = dialogueData.dialogueEntries[nodeId];
            
            // 创建中文版对话记录条目
            var journalEntry_CN = new DialogueJournal 
            { 
                characterName_CN = entry.characterName_CN, 
                dialogue_CN = entry.dialogue_CN, 
                color = entry.color 
            };
            
            // 创建英文版对话记录条目
            var journalEntry_EN = new DialogueJournal 
            { 
                characterName_EN = entry.characterName_EN, 
                dialogue_EN = entry.dialogue_EN, 
                color = entry.color 
            };
            
            // 对选项节点和结局节点，将角色名称设为"   "，但仍然记录在对话记录中
            if (entry.characterName_CN == "选项" || entry.characterName_CN == "结局")
            {
                journalEntry_CN.characterName_CN = "   ";
                journalEntry_EN.characterName_EN = "   ";
            }
            
            // 同时向两个列表添加记录
            dialogueJournalList_CN.Add(journalEntry_CN);
            dialogueJournalList_EN.Add(journalEntry_EN);
            currentConversationNodeIds.Add(nodeId);

            //PlayTalkingAnimation(entry.characterName);
        }



        //单例
        private static DialogueJournalManager instance;
        public static DialogueJournalManager Instance
        {
            get
            {
                if (!instance)
                {
                    var uiRoot = UIRoot.Instance;
                    Debug.Log("currentUIRoot:" + uiRoot);
                    instance = MonoSingletonProperty<DialogueJournalManager>.Instance;
                }
                return instance;
            }
        }

        public void AddDialogueJournal(DialogueJournal dialogueJournal)
        {
            // 创建中文版记录
            var journalEntry_CN = new DialogueJournal 
            { 
                characterName_CN = dialogueJournal.characterName_CN, 
                dialogue_CN = dialogueJournal.dialogue_CN, 
                color = dialogueJournal.color 
            };
            
            // 创建英文版记录（交换中英文内容）
            var journalEntry_EN = new DialogueJournal 
            { 
                characterName_EN = dialogueJournal.characterName_EN, 
                dialogue_EN = dialogueJournal.dialogue_EN, 
                color = dialogueJournal.color 
            };
            
            dialogueJournalList_CN.Add(journalEntry_CN);
            dialogueJournalList_EN.Add(journalEntry_EN);
        }

        public void ClearDialogueJournal()
        {
            dialogueJournalList_CN.Clear();
            dialogueJournalList_EN.Clear();
        }

        public List<DialogueJournal> GetDialogueJournal_CN()
        {
            return dialogueJournalList_CN;
        }

        public List<DialogueJournal> GetDialogueJournal_EN()
        {
            return dialogueJournalList_EN;
        }

        /// <summary>
        /// 检查指定对话是否已预加载
        /// </summary>
        public bool IsConversationLoaded(string conversationName)
        {
            return dialogueDataCache.ContainsKey(conversationName);
        }

        /// <summary>
        /// 获取指定对话的预加载数据
        /// </summary>
        public DialogueJournalData GetConversationData(string conversationName)
        {
            dialogueDataCache.TryGetValue(conversationName, out var data);
            return data;
        }

        /// <summary>
        /// 获取所有已预加载的对话名称
        /// </summary>
        public string[] GetAllLoadedConversationNames()
        {
            string[] names = new string[dialogueDataCache.Keys.Count];
            dialogueDataCache.Keys.CopyTo(names, 0);
            return names;
        }

        /// <summary>
        /// 获取预加载状态信息
        /// </summary>
        public string GetPreloadStatus()
        {
            if (dialogueJournalDataArray == null)
                return "未开始预加载";
            
            int total = dialogueJournalDataArray.Length;
            int loaded = dialogueDataCache.Count;
            return $"预加载状态: {loaded}/{total} 个对话已加载";
        }

        /// <summary>
        /// 强制刷新对话数据（当SO文件被修改后调用）
        /// </summary>
        public void ForceRefreshDialogueData()
        {
            Debug.Log("强制刷新对话数据...");
            
            // 清空所有缓存
            dialogueDataCache.Clear();
            dialogueJournalDataArray = null;
            
            // 强制垃圾回收
            System.GC.Collect();
            
            // 重新加载
            LoadAllDialogueData();
            
            Debug.Log("对话数据刷新完成！");
        }
    }

    public class DialogueJournal
    {
        public string characterName_CN;
        public string characterName_EN;
        public string dialogue_CN;
        public string dialogue_EN;
        public Color color;
    }
}