using FSM;
using QFramework;
using QFramework.Example;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

namespace QFramework.Example
{
	public class UIInvitedPanelData : UIPanelData
	{
	}
	public partial class UIInvitedPanel : UIPanel
	{
		[SerializeField] private Sprite location4;
		public Machine FsmManager;
        private bool isSubmit;
        private bool[] inputsCorrect = new bool[4];
        private Dictionary<int, Dictionary<GlobalEnums.Language, string>> correctAnswers;

        void Start()
        {
            isSubmit = false;
            // 初始化 correctAnswers 只需一次
            correctAnswers = new Dictionary<int, Dictionary<GlobalEnums.Language, string>>
    {
        { 0, new Dictionary<GlobalEnums.Language, string>
            {
                { GlobalEnums.Language.Chinese, "王国信" },
                { GlobalEnums.Language.English, "WangGuoXin" }
            }
        },
        { 1, new Dictionary<GlobalEnums.Language, string>
            {
                { GlobalEnums.Language.Chinese, "2025年6月28日" },
                { GlobalEnums.Language.English, "June 28, 2025" }
            }
        },
        { 2, new Dictionary<GlobalEnums.Language, string>
            {
                { GlobalEnums.Language.Chinese, "广深大厦7层708房" },
                { GlobalEnums.Language.English, "Room 708, 7F, GuangShen Building" }
            }
        },
        { 3, new Dictionary<GlobalEnums.Language, string>
            {
                { GlobalEnums.Language.Chinese, "约翰逊" },
                { GlobalEnums.Language.English, "John" }
            }
        }
    };
            Global.CurrentLanguage.RegisterWithInitValue(OnLanguageChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
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
            

            //SceneManager.Instance.LoadExtraScene("JD");
			TimeLineManager.Instance.LoadScene("JD");
            FsmManager.ChangeToStateByName("State-接待礼仪上车");
            Debug.Log("切换到接待礼仪上车");
            //UIKit.OpenPanel<UIMeetingPanel>(UILevel.Common, null, null, "UIPrefabs/UIMeetingPanel");
            UIKit.ClosePanel<UIInvitedPanel>();
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
            //SceneManager.Instance.UnloadExtraScene();
            //SceneManager.Instance.LoadExtraScene("XD");
            TimeLineManager.Instance.UnloadScene("JD");
            //TimeLineManager.Instance.LoadScene("XD");

            UIKit.OpenPanel<UIMeetingPanel>(UILevel.Common, null, null, "UIPrefabs/UIMeetingPanel");
            UIKit.ClosePanel<UIInvitedPanel>();
        }

        private void OnClickSubmit_1()
        {
            isSubmit = true;
            Debug.Log("OnClickSubmit_1");
            Btn_Submit_1.gameObject.SetActive(false);
            Btn_Next_1.gameObject.SetActive(true);

            TMP_InputField[] inputs = new TMP_InputField[4];
            for (int i = 0; i < 4; i++)
            {
                inputs[i] = InputFileds.transform.GetChild(i).GetChild(1).GetComponent<TMP_InputField>();
            }

            Debug.Log("inputs.Length:" + inputs.Length);

            // 初始化分数
            Global.ScoreList[2] = 0f;

            // 遍历每个输入框，检查答案是否正确
            for (int i = 0; i < inputs.Length; i++)
            {
                string correctAnswer = correctAnswers[i][Global.CurrentLanguage.Value];
                TMP_InputField input = inputs[i];

                Debug.Log("inputs[i].text:" + input.text);

                // 判断输入是否正确
                bool isCorrect = input.text == correctAnswer;
                inputsCorrect[i] = isCorrect; // 记录每个输入框的正确性
                input.textComponent.color = isCorrect ? Color.green : Color.red;

                // 如果不正确，自动填入正确答案
                if (!isCorrect)
                {
                    input.text = correctAnswer;
                }

                // 更新分数
                Global.ScoreList[2] += isCorrect ? 2f : 0f;
            }
        }

        private void OnLanguageChanged(GlobalEnums.Language language)
        {
            if (!isSubmit) return;

            TMP_InputField[] inputs = new TMP_InputField[4];
            for (int i = 0; i < 4; i++)
            {
                inputs[i] = InputFileds.transform.GetChild(i).GetChild(1).GetComponent<TMP_InputField>();
                inputs[i].text = correctAnswers[i][language];

                // 根据语言切换后的正确性状态更新颜色
                bool isCorrect = inputsCorrect[i]; // 使用记录的正确性状态
                inputs[i].textComponent.color = isCorrect ? Color.green : Color.red;
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