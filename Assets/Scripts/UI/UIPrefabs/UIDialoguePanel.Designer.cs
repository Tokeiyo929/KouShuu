using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:af8e69b8-0703-413a-89ed-e46f98a07ee0
	public partial class UIDialoguePanel
	{
		public const string Name = "UIDialoguePanel";
		
		
		private UIDialoguePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIDialoguePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIDialoguePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIDialoguePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
