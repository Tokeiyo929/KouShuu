using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:f7e5a635-0d9a-469e-a95f-b9065f73fb16
	public partial class UIDialogueJournalPanel
	{
		public const string Name = "UIDialogueJournalPanel";
		
		[SerializeField]
		public UnityEngine.UI.Toggle Tog_Open;
		[SerializeField]
		public UnityEngine.UI.Image Page_DialogueJournal;
		[SerializeField]
		public TMPro.TextMeshProUGUI Text_DialogueJournal;
		
		private UIDialogueJournalPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Tog_Open = null;
			Page_DialogueJournal = null;
			Text_DialogueJournal = null;
			
			mData = null;
		}
		
		public UIDialogueJournalPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIDialogueJournalPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIDialogueJournalPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
