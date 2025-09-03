using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
    [CreateAssetMenu(fileName = "DialogueJournalData", menuName = "Dialogue/Dialogue Journal Data")]
    public class DialogueJournalData : ScriptableObject
    {
        [Serializable]
        public class DialogueEntry
        {
            public string characterName_CN;
            public string characterName_EN;
            [TextArea(3, 10)]
            public string dialogue_CN;
            [TextArea(3, 10)]
            public string dialogue_EN;
            public Color color = Color.black;
        }

        public string conversationName;
        public List<DialogueEntry> dialogueEntries = new List<DialogueEntry>();
    }
} 