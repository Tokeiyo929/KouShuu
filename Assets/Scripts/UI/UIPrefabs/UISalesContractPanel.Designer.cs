using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:d8d7a1d2-d73b-48f7-ae2e-da3a8b5d8504
	public partial class UISalesContractPanel
	{
		public const string Name = "UISalesContractPanel";
		
		[SerializeField]
		public UnityEngine.GameObject Page_Contract_1;
		[SerializeField]
		public TMPro.TMP_Dropdown DropDown_Seller_Chinese;
		[SerializeField]
		public TMPro.TMP_Dropdown DropDown_Seller_English;
		[SerializeField]
		public TMPro.TMP_Dropdown DropDown_Buyer_Chinese;
		[SerializeField]
		public TMPro.TMP_Dropdown DropDown_Buyer_English;
		[SerializeField]
		public UnityEngine.GameObject DropDown_Products;
		[SerializeField]
		public UnityEngine.UI.Image DropDown_Commodity_1;
		[SerializeField]
		public TMPro.TextMeshProUGUI Text_Amount_1;
		[SerializeField]
		public UnityEngine.UI.Image DropDown_Commodity_2;
		[SerializeField]
		public TMPro.TextMeshProUGUI Text_Amount_2;
		[SerializeField]
		public UnityEngine.UI.Image DropDown_Commodity_3;
		[SerializeField]
		public TMPro.TextMeshProUGUI Text_Amount_3;
		[SerializeField]
		public UnityEngine.UI.Button Btn_NextPage;
		[SerializeField]
		public TMPro.TextMeshProUGUI Text_TotalAmount;
		[SerializeField]
		public UnityEngine.GameObject Page_Contract_2;
		[SerializeField]
		public UnityEngine.UI.Button Btn_PreviousPage;
		[SerializeField]
		public TMPro.TMP_InputField InputField_TotalValue;
		[SerializeField]
		public TMPro.TMP_InputField InputField_Time_Chinese;
		[SerializeField]
		public TMPro.TMP_InputField InputField_Time_English;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Submit;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next;
		
		private UISalesContractPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Page_Contract_1 = null;
			DropDown_Seller_Chinese = null;
			DropDown_Seller_English = null;
			DropDown_Buyer_Chinese = null;
			DropDown_Buyer_English = null;
			DropDown_Products = null;
			DropDown_Commodity_1 = null;
			Text_Amount_1 = null;
			DropDown_Commodity_2 = null;
			Text_Amount_2 = null;
			DropDown_Commodity_3 = null;
			Text_Amount_3 = null;
			Btn_NextPage = null;
			Text_TotalAmount = null;
			Page_Contract_2 = null;
			Btn_PreviousPage = null;
			InputField_TotalValue = null;
			InputField_Time_Chinese = null;
			InputField_Time_English = null;
			Btn_Submit = null;
			Btn_Next = null;
			
			mData = null;
		}
		
		public UISalesContractPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UISalesContractPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UISalesContractPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
