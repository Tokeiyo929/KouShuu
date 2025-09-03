using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;
using FSM;
using QFramework.Example;

namespace QFramework.Example
{
	public class UIInvitedPanelData : UIPanelData
	{
	}
	public partial class UIInvitedPanel : UIPanel
	{
		[SerializeField] private Sprite location4;
		public Machine FsmManager;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIInvitedPanelData ?? new UIInvitedPanelData();
			// please add init code here
			OnClickButton();

			FsmManager = FindObjectOfType<Machine>();
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{ 
			Page_Invitation.Show();
			Page_Car.Hide();
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

		private void OnClickButton()
		{
			//TitlePanel.OnBackButtonClick =OnClickLastButton;

			Btn_Next_1.onClick.AddListener(OnClickNext_1);
			Btn_Next_2.onClick.AddListener(OnClickNext_2);

            Btn_Submit_1.onClick.AddListener(OnClickSubmit_1);
			Btn_Submit_2.onClick.AddListener(OnClickSubmit_2);

		}

		private void OnClickLastButton()
		{
			UIKit.ClosePanel<UIInvitedPanel>();
			UIKit.OpenPanel<UISelectClosePanel>(UILevel.Common, null, null, "UIPrefabs/UISelectClosePanel");
		}

		private void OnClickNext_1()
		{
            UIKit.ClosePanel<UIInvitedPanel>();

            SceneManager.Instance.LoadExtraScene("JD");
            FsmManager.ChangeToStateByName("State-接待礼仪上车");
            Debug.Log("切换到接待礼仪上车");
            //UIKit.OpenPanel<UIMeetingPanel>(UILevel.Common, null, null, "UIPrefabs/UIMeetingPanel");
		}
		public void OpenCarPage()
		{
            Page_Invitation.Hide();
            Page_Car.Show();

            Tog_location1.isOn = false;
            Tog_location2.isOn = false;
            Tog_location3.isOn = false;
            Tog_location4.isOn = false;
        }
		public void OnClickNext_2()
		{
            SceneManager.Instance.UnloadExtraScene();
            SceneManager.Instance.LoadExtraScene("XD");

            UIKit.OpenPanel<UIMeetingPanel>(UILevel.Common, null, null, "UIPrefabs/UIMeetingPanel");
            UIKit.ClosePanel<UIInvitedPanel>();
        }

		private void OnClickSubmit_1()
		{
            Debug.Log("OnClickSubmit_1");
			Btn_Submit_1.gameObject.SetActive(false);
			Btn_Next_1.gameObject.SetActive(true);

            TMP_InputField[] inputs = new TMP_InputField[4];
            for(int i = 0; i < 4; i++)
            {
                inputs[i] = InputFileds.transform.GetChild(i).GetChild(1).GetComponent<TMP_InputField>();
            }

            Debug.Log("inputs.Length:"+inputs.Length);
			Global.ScoreList[2]=0f;
            for(int i = 0; i < inputs.Length; i++)
            {
                Debug.Log("inputs[i].text:"+inputs[i].text);
                switch(i)
                {
                    case 0:
                        if(inputs[i].text != "王国信")
                        {    
                            inputs[i].textComponent.color = Color.red;
                            inputs[i].text="王国信";
							Global.ScoreList[2] += 0f;
                        }
                        else
                        {    inputs[i].textComponent.color = Color.green;
							Global.ScoreList[2] += 2f;}
                        break;
                    case 1:
                        if(inputs[i].text != "2025年6月28日")
                        {
                            inputs[i].textComponent.color = Color.red;
                            inputs[i].text="2025年6月28日";
							Global.ScoreList[2] += 0f;
                        }
                        else    
                        {    inputs[i].textComponent.color = Color.green;
							Global.ScoreList[2] += 2f;}
                        break;
                    case 2:
                        if(inputs[i].text != "广深大厦7层708房")
                        {
                            inputs[i].textComponent.color = Color.red;
                            inputs[i].text="广深大厦7层708房";
							Global.ScoreList[2] += 0f;
                        }
                        else
                        {    inputs[i].textComponent.color = Color.green;
							Global.ScoreList[2] += 2f;}
                        break;
                    case 3:
                        if(inputs[i].text != "约翰逊")
                        {
                            inputs[i].textComponent.color = Color.red;
                            inputs[i].text="约翰逊";
							Global.ScoreList[2] += 0f;
                        }
                        else
                        {    inputs[i].textComponent.color = Color.green;
							Global.ScoreList[2] += 2f;}
                        break;
                }
            }
            
		}

		private void OnClickSubmit_2()
		{
			Btn_Submit_2.gameObject.SetActive(false);
			Btn_Next_2.gameObject.SetActive(true);

			if(Tog_location4.isOn)
			{
				Img_Correct.gameObject.SetActive(true);
				Img_Error.gameObject.SetActive(false);

				Global.ScoreList[3] = 4f;
			}
			else
			{
				Img_Correct.gameObject.SetActive(false);
				Img_Error.gameObject.SetActive(true);

				Global.ScoreList[3] = 0f;
			}
			Img_Left.sprite = location4;

		}
	}

	
}