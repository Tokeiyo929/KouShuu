using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;
using System.Collections.Generic;

namespace QFramework.Example
{
	public class UIDialogueJournalPanelData : UIPanelData
	{
	}
	public partial class UIDialogueJournalPanel : UIPanel
	{

		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIDialogueJournalPanelData ?? new UIDialogueJournalPanelData();
			// please add init code here

			Tog_Open.onValueChanged.AddListener((isOn)=>OpenPage(isOn));

			Global.CurrentLanguage.Register(OnLanguageChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
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

		private void OpenPage(bool isOn)
		{
			if(isOn)
			{
				UpdateDialogueJournalForCurrentLanguage();
				Page_DialogueJournal.Show();
			}
			else
			{
				Page_DialogueJournal.Hide();
			}
		}

		private void OnLanguageChanged(GlobalEnums.Language language)
		{
			UpdateDialogueJournalForCurrentLanguage();
		}

        private void UpdateDialogueJournalForCurrentLanguage()
        {
            if (Global.CurrentLanguage.Value == GlobalEnums.Language.English)
                UpdateDialogueJournal(1);
            else
                UpdateDialogueJournal(0);
        }
        

		private void UpdateDialogueJournal(int language)
		{
			List<DialogueJournal> logs = new List<DialogueJournal>();
			if(language == 0)
				logs = DialogueJournalManager.Instance.GetDialogueJournal_CN();
			else if(language == 1)
				logs = DialogueJournalManager.Instance.GetDialogueJournal_EN();

			// 确保启用富文本
			var tmp = Text_DialogueJournal as TMPro.TMP_Text;
			if (tmp != null)
			{
				tmp.richText = true;
			}
			else
			{
				var comp = Text_DialogueJournal as UnityEngine.Component;
				var uiText = comp != null ? comp.GetComponent<UnityEngine.UI.Text>() : null;
				if (uiText != null) uiText.supportRichText = true;
			}

			if (logs == null || logs.Count == 0)
			{
				Text_DialogueJournal.text = "";
			}
			else
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				for (int i = 0; i < logs.Count; i++)
				{
					var item = logs[i];
					string hex = ColorUtility.ToHtmlStringRGB(item.color);
					sb.Append("<color=#").Append(hex).Append(">");
					
					// 如果角色名称为"选项"或"结局"，则不附加冒号
					if (item.characterName_CN == "选项" || item.characterName_CN == "结局")
					{
						if(language == 0)
							sb.Append(item.characterName_CN ?? string.Empty)
							  .Append(item.dialogue_CN ?? string.Empty);
						else
							sb.Append(item.characterName_EN ?? string.Empty)
							  .Append(item.dialogue_EN ?? string.Empty);
					}
					else
					{
						if(language == 0)
							sb.Append(item.characterName_CN ?? string.Empty)
							  .Append(": ")
							  .Append(item.dialogue_CN ?? string.Empty);
						else
							sb.Append(item.characterName_EN ?? string.Empty)
							  .Append(": ")
							  .Append(item.dialogue_EN ?? string.Empty);
					}
					
					sb.Append("</color>");
					if (i < logs.Count - 1) sb.Append('\n');
				}
				Text_DialogueJournal.text = sb.ToString();
				Debug.Log("Text_DialogueJournal.text: " + Text_DialogueJournal.text);
			}	
		}
	}
}
