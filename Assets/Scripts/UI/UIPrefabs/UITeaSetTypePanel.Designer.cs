using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:9053654b-6486-4a73-a57b-79863fcc80e9
	public partial class UITeaSetTypePanel
	{
		public const string Name = "UITeaSetTypePanel";
		
		[SerializeField]
		public UnityEngine.UI.Image Page_TeaSetType;
		[SerializeField]
		public UnityEngine.UI.Toggle Tog_TeaSet_1;
		[SerializeField]
		public UnityEngine.UI.Toggle Tog_TeaSet_2;
		[SerializeField]
		public UnityEngine.UI.Toggle Tog_TeaSet_3;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Check;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next;
		[SerializeField]
		public UnityEngine.UI.Image Img_Correct;
		[SerializeField]
		public UnityEngine.UI.Image Img_Error;
		
		private UITeaSetTypePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Page_TeaSetType = null;
			Tog_TeaSet_1 = null;
			Tog_TeaSet_2 = null;
			Tog_TeaSet_3 = null;
			Btn_Check = null;
			Btn_Next = null;
			Img_Correct = null;
			Img_Error = null;
			
			mData = null;
		}
		
		public UITeaSetTypePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UITeaSetTypePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UITeaSetTypePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
