using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:1c0fd781-9dc8-4d4f-958f-0c5cbf17d290
	public partial class UIMakeTeaPanel
	{
		public const string Name = "UIMakeTeaPanel";
		
		[SerializeField]
		public UnityEngine.UI.Image Page_MakeTea;
		[SerializeField]
		public UnityEngine.GameObject Text_Left;
		[SerializeField]
		public RectTransform InputFields;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Submit;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next;
		[SerializeField]
		public UnityEngine.GameObject Page_Tips;
		
		private UIMakeTeaPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Page_MakeTea = null;
			Text_Left = null;
			InputFields = null;
			Btn_Submit = null;
			Btn_Next = null;
			Page_Tips = null;
			
			mData = null;
		}
		
		public UIMakeTeaPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIMakeTeaPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIMakeTeaPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
