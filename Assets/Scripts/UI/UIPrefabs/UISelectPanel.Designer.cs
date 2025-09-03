using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:9d3c7ab4-3b0e-48a0-8177-5bfceddda5a6
	public partial class UISelectPanel
	{
		public const string Name = "UISelectPanel";
		
		[SerializeField]
		public UnityEngine.UI.ToggleGroup Left;
		[SerializeField]
		public UnityEngine.UI.Toggle Btn_History;
		[SerializeField]
		public UnityEngine.UI.Toggle Btn_Environment;
		[SerializeField]
		public UnityEngine.UI.Toggle Btn_Culture;
		[SerializeField]
		public UnityEngine.UI.Toggle Btn_Communication;
		[SerializeField]
		public UnityEngine.UI.Toggle Btn_Vocabulary;
		[SerializeField]
		public UnityEngine.UI.Toggle Btn_Logic;
		[SerializeField]
		public UnityEngine.UI.Toggle Btn_Contract;
		[SerializeField]
		public UnityEngine.UI.Toggle Btn_Test;
		
		private UISelectPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Left = null;
			Btn_History = null;
			Btn_Environment = null;
			Btn_Culture = null;
			Btn_Communication = null;
			Btn_Vocabulary = null;
			Btn_Logic = null;
			Btn_Contract = null;
			Btn_Test = null;
			
			mData = null;
		}
		
		public UISelectPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UISelectPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UISelectPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
