using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:ef271c16-ef8c-48c6-910c-3ccd40227128
	public partial class UITipPanel_SignAgreement
	{
		public const string Name = "UITipPanel_SignAgreement";
		
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next_2;
		
		private UITipPanel_SignAgreementData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Btn_Next = null;
			Btn_Next_2 = null;
			
			mData = null;
		}
		
		public UITipPanel_SignAgreementData Data
		{
			get
			{
				return mData;
			}
		}
		
		UITipPanel_SignAgreementData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UITipPanel_SignAgreementData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
