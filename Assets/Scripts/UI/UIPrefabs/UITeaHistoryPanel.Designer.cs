using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:3918df30-21a5-47cb-a00c-7ea9c1e66758
	public partial class UITeaHistoryPanel
	{
		public const string Name = "UITeaHistoryPanel";
		
		[SerializeField]
		public RectTransform Content_Obj;
		[SerializeField]
		public UnityEngine.UI.Image Page_Little;
		[SerializeField]
		public UnityEngine.UI.Image Img_BG;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Exit;
		
		private UITeaHistoryPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Content_Obj = null;
			Page_Little = null;
			Img_BG = null;
			Btn_Exit = null;
			
			mData = null;
		}
		
		public UITeaHistoryPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UITeaHistoryPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UITeaHistoryPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
