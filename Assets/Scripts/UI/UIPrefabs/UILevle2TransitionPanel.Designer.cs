using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:041892fe-fbf7-4ecd-9615-3a4c3e1c7f90
	public partial class UILevle2TransitionPanel
	{
		public const string Name = "UILevle2TransitionPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next;
		
		private UILevle2TransitionPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Btn_Next = null;
			
			mData = null;
		}
		
		public UILevle2TransitionPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UILevle2TransitionPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UILevle2TransitionPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
