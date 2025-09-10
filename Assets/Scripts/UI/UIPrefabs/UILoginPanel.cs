using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System;

namespace QFramework.Example
{
	public class UILoginPanelData : UIPanelData
	{
	}
	public partial class UILoginPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UILoginPanelData ?? new UILoginPanelData();
			// please add init code here
			Btn_Check.onClick.AddListener(OnClickCheck);
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}

		private void OnClickCheck()
		{
			if(InputField_Name.text==""||InputField_Class.text==""||InputField_ID.text=="")
			{
				return;
			}

			Global.StudentName = InputField_Name.text;
			Global.StudentClass = InputField_Class.text;
			Global.StudentId = InputField_ID.text;

			Global.StudentStartTime = DateTime.Now;

            // 打开标题界面

            //临时解决ScoreList为空问题
            if (Global.ScoreList.Count == 0)
            {
                for (int i = 0; i < 18; i++)
                {
                    Global.ScoreList.Add(0f);
                }
            }

            UIKit.CloseAllPanel();
			UIKit.OpenPanel<UITitlePanel>(UILevel.PopUI, null, null, "UIPrefabs/UITitlePanel");
			UIKit.OpenPanel<UIDialoguePanel>(UILevel.PopUI, null, null, "UIPrefabs/UIDialoguePanel");
			UIKit.OpenPanel<UISelectPanel>(UILevel.Common, null, null, "UIPrefabs/UISelectPanel");
		}
	}
}
