using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:b05042e9-e3f3-4da8-94a6-c486d00c82e6
	public partial class UITestUI
	{
		public const string Name = "UITestUI";
		
		[SerializeField]
		public UnityEngine.UI.Button Btn_Test;
		
		private UITestUIData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Btn_Test = null;
			
			mData = null;
		}
		
		public UITestUIData Data
		{
			get
			{
				return mData;
			}
		}
		
		UITestUIData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UITestUIData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
