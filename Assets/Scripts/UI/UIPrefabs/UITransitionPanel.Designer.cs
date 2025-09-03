using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:8af1d2a1-0d3a-4990-8a54-b21f44631bb1
	public partial class UITransitionPanel
	{
		public const string Name = "UITransitionPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next;
		
		private UITransitionPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Btn_Next = null;
			
			mData = null;
		}
		
		public UITransitionPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UITransitionPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UITransitionPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
