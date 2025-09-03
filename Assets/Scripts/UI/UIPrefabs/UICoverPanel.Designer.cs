using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:22d0cdf6-9f4a-4694-9383-feac104851f7
	public partial class UICoverPanel
	{
		public const string Name = "UICoverPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button Btn_Exit;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Start;
		
		private UICoverPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Btn_Exit = null;
			Btn_Start = null;
			
			mData = null;
		}
		
		public UICoverPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UICoverPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UICoverPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
