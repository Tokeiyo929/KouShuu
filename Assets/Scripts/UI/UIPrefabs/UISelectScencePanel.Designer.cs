using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:2e0b8e72-965d-4190-a065-4c00f320eeb0
	public partial class UISelectScencePanel
	{
		public const string Name = "UISelectScencePanel";
		
		[SerializeField]
		public TMPro.TextMeshProUGUI Text_CurrentScence;
		[SerializeField]
		public UnityEngine.UI.Button Btn_LastScence;
		[SerializeField]
		public UnityEngine.UI.Button Btn_NextScence;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Exit;
		
		private UISelectScencePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Text_CurrentScence = null;
			Btn_LastScence = null;
			Btn_NextScence = null;
			Btn_Exit = null;
			
			mData = null;
		}
		
		public UISelectScencePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UISelectScencePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UISelectScencePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
