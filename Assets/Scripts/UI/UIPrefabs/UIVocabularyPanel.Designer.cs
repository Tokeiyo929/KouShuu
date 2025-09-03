using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:65046112-e863-4966-b5a1-1fb26b50a7fd
	public partial class UIVocabularyPanel
	{
		public const string Name = "UIVocabularyPanel";
		
		
		private UIVocabularyPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIVocabularyPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIVocabularyPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIVocabularyPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
