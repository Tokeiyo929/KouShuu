using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:e2e0d464-5678-44d3-9195-aece2fdde8fd
	public partial class UITeaMeetingPanel
	{
		public const string Name = "UITeaMeetingPanel";
		
		[SerializeField]
		public RectTransform Tip_SelectError;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next_1;
		[SerializeField]
		public UnityEngine.UI.Image Page_MakeTea;
		[SerializeField]
		public RectTransform InputFields;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Submit;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next_2;
		[SerializeField]
		public UnityEngine.UI.Button Btn_NextModule;
		
		private UITeaMeetingPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Tip_SelectError = null;
			Btn_Next_1 = null;
			Page_MakeTea = null;
			InputFields = null;
			Btn_Submit = null;
			Btn_Next_2 = null;
			Btn_NextModule = null;
			
			mData = null;
		}
		
		public UITeaMeetingPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UITeaMeetingPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UITeaMeetingPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
