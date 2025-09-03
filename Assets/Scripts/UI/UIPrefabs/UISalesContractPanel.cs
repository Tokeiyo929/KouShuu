using UnityEngine;
using UnityEngine.UI;
using QFramework;
using DialogueEditor;
using System;
using FSM;
using TMPro;

namespace QFramework.Example
{
	public class UISalesContractPanelData : UIPanelData
	{
	}
	public partial class UISalesContractPanel : UIPanel
	{
		public Machine FsmManager;

		private TMP_Dropdown[] Questions_DropDown=new TMP_Dropdown[16];
		private TMP_Text[] Questions_Text=new TMP_Text[4];
		private TMP_InputField[] Questions_InputField=new TMP_InputField[3];
		private int[] Answers_DropDown={1,1,0,0,1,2,3,4,4,2,5,1,5,3,3,0};
		private float[] Answers_Text={698400.00f,465600.00f,360000.00f,1524000.00f};
		private String[] Answers_InputField={"1524000","八月上旬","early August"};

		private GameObject[] DropDown_Product = new GameObject[3];


		private TMP_Dropdown[] DropDown_Commoditys = new TMP_Dropdown[3];
		private TMP_Dropdown[] DropDown_Quantitys = new TMP_Dropdown[3];
		private TMP_Dropdown[] DropDown_Units = new TMP_Dropdown[3];
		private TMP_Dropdown[] DropDown_TradeTerms = new TMP_Dropdown[3];
		private TMP_Text[] Text_Amounts = new TMP_Text[3];

		private int[] currentQuantitys = new int[3];
		private int[] currentUnitPrices = new int[3];
		private float[] currentDiscounts = new float[3];

		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UISalesContractPanelData ?? new UISalesContractPanelData();
			// please add init code here
			
			FsmManager = FindObjectOfType<Machine>();

			Questions_DropDown[0]=DropDown_Seller_Chinese;
			Questions_DropDown[1]=DropDown_Seller_English;
			Questions_DropDown[2]=DropDown_Buyer_Chinese;
			Questions_DropDown[3]=DropDown_Buyer_English;

			Questions_InputField[0]=InputField_TotalValue;
			Questions_InputField[1]=InputField_Time_Chinese;
			Questions_InputField[2]=InputField_Time_English;

			Questions_Text[3]=Text_TotalAmount;


			for(int i=0;i<3;i++)
			{
				DropDown_Product[i]=DropDown_Products.transform.GetChild(i).gameObject;
				Debug.Log("DropDown_Product["+i+"]:"+DropDown_Product[i].name);
			}

			for(int i=0;i<3;i++)
			{
				DropDown_Commoditys[i]=DropDown_Product[i].transform.GetChild(0).GetComponent<TMP_Dropdown>();
				DropDown_Quantitys[i]=DropDown_Product[i].transform.GetChild(1).GetComponent<TMP_Dropdown>();
				DropDown_Units[i]=DropDown_Product[i].transform.GetChild(2).GetComponent<TMP_Dropdown>();
				DropDown_TradeTerms[i]=DropDown_Product[i].transform.GetChild(3).GetComponent<TMP_Dropdown>();
				Text_Amounts[i]=DropDown_Product[i].transform.GetChild(4).GetComponent<TMP_Text>();

				Questions_DropDown[i+4]=DropDown_Commoditys[i];
				Questions_DropDown[i+7]=DropDown_Quantitys[i];
				Questions_DropDown[i+10]=DropDown_Units[i];
				Questions_DropDown[i+13]=DropDown_TradeTerms[i];
				Questions_Text[i]=Text_Amounts[i];

				// 绑定事件监听器 - 使用局部变量捕获正确的索引值
				int rowIndex = i; // 捕获循环变量
				
				if (DropDown_Quantitys[i] != null)
				{
					DropDown_Quantitys[i].onValueChanged.AddListener((value) => OnValueChangedQuantity(rowIndex, value));
					Debug.Log($"产品行 {i} 数量下拉框事件绑定成功，索引: {rowIndex}");
				}

				if (DropDown_Units[i] != null)
				{
					DropDown_Units[i].onValueChanged.AddListener((value) => OnValueChangedUnit(rowIndex, value));
					Debug.Log($"产品行 {i} 单价下拉框事件绑定成功，索引: {rowIndex}");
				}

				if (DropDown_TradeTerms[i] != null)
				{
					DropDown_TradeTerms[i].onValueChanged.AddListener((value) => OnValueChangedTradeTerms(rowIndex, value));
					Debug.Log($"产品行 {i} 贸易条款下拉框事件绑定成功，索引: {rowIndex}");
				}
			}

			Btn_NextPage.onClick.AddListener(OnClickNextPage);
			Btn_PreviousPage.onClick.AddListener(OnClickPreviousPage);
			Btn_Submit.onClick.AddListener(OnClickSubmit);
			Btn_Next.onClick.AddListener(OnClickNext);

			Debug.Log("UISalesContractPanel 初始化完成！");
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

		private void OnClickNextPage()
		{
			Page_Contract_1.SetActive(false);
			Page_Contract_2.SetActive(true);
			
		}

		private void OnClickPreviousPage()
		{
			Page_Contract_1.SetActive(true);
			Page_Contract_2.SetActive(false);
		}

		private void OnClickSubmit()
		{
			Btn_Submit.gameObject.SetActive(false);
			Btn_Next.gameObject.SetActive(true);


			for(int i=0;i<4;i++)
			{
				// 使用格式化字符串确保小数位一致
				string currentText = Questions_Text[i].text;
				string correctAnswer = Answers_Text[i].ToString("F2");
				
				Debug.Log($"第{i+1}题文本：当前='{currentText}', 正确答案='{correctAnswer}'");
				
				if(currentText != correctAnswer)
				{
					Questions_Text[i].text = correctAnswer;
					Questions_Text[i].color = Color.red;
					Debug.Log($"第{i+1}题文本错误，已更正为：{correctAnswer}");

					Global.ScoreList[17] += 0;
				}
				else
				{
					Questions_Text[i].color = Color.green;
					Debug.Log($"第{i+1}题文本正确！");

					Global.ScoreList[17] += 1;
				}
			}
			
			for(int i=0;i<16;i++)
			{
				// 获取当前选择的值和正确答案
				int currentSelection = Questions_DropDown[i].value;
				int correctAnswer = Answers_DropDown[i];
				
				if(currentSelection != correctAnswer)
				{
					// 选择错误：将文本替换为正确答案并标红
					// 使用captionText来显示正确答案
					Questions_DropDown[i].value=correctAnswer;
					Questions_DropDown[i].captionText.color = Color.red;

					Global.ScoreList[17] += 0;
				}
				else
				{
					// 选择正确：标绿
					Questions_DropDown[i].captionText.color = Color.green;

					Global.ScoreList[17] += 1;
				}
			}
			
			for(int i=0;i<3;i++)
			{
				if(Questions_InputField[i].text!=Answers_InputField[i].ToString())
				{
					Questions_InputField[i].text=Answers_InputField[i].ToString();
					Questions_InputField[i].textComponent.color=Color.red;

					Global.ScoreList[17] += 0;
				}
				else
				{
					Questions_InputField[i].textComponent.color=Color.green;

					Global.ScoreList[17] += 1;
				}
			}


		}

		private void OnClickNext()
		{
			UIKit.ClosePanel<UISalesContractPanel>();
			FsmManager.ChangeToStateByName("State-销售协议递回");
		}

		private void OnValueChangedQuantity(int index,int value)
		{
			// 检查索引范围
			if (index < 0 || index >= 3)
			{
				Debug.LogError($"数量变化事件索引超出范围: {index}");
				return;
			}

			// 检查组件是否存在
			if (DropDown_Quantitys[index] == null)
			{
				Debug.LogError($"产品行 {index} 的数量下拉框为null！");
				return;
			}

			// 检查选项索引范围
			if (value < 0 || value >= DropDown_Quantitys[index].options.Count)
			{
				Debug.LogError($"数量下拉框选项索引超出范围: {value}");
				return;
			}

			// 获取下拉框当前选择的文本内容
			string quantityText = DropDown_Quantitys[index].options[value].text;
			Debug.Log($"Quantity dropdown text: '{quantityText}' at index {index}");
			
			// 提取数字部分，例如"500斤" -> 500
			// 查找第一个非数字字符的位置
			int endIndex = 0;
			for (int i = 0; i < quantityText.Length; i++)
			{
				if (!char.IsDigit(quantityText[i]))
				{
					endIndex = i;
					break;
				}
				endIndex = i + 1;
			}
			
			if (endIndex > 0 && int.TryParse(quantityText.Substring(0, endIndex), out int quantityInt))
			{
				currentQuantitys[index] = quantityInt;
				Debug.Log("quantity: " + currentQuantitys[index]);
				CalculateTotalAmount(index);
			}
			else
			{
				Debug.LogWarning($"Failed to parse quantity from text: '{quantityText}'");
			}
		}

		private void OnValueChangedUnit(int index,int value)
		{
			// 检查索引范围
			if (index < 0 || index >= 3)
			{
				Debug.LogError($"单价变化事件索引超出范围: {index}");
				return;
			}

			// 检查组件是否存在
			if (DropDown_Units[index] == null)
			{
				Debug.LogError($"产品行 {index} 的单价下拉框为null！");
				return;
			}

			// 检查选项索引范围
			if (value < 0 || value >= DropDown_Units[index].options.Count)
			{
				Debug.LogError($"单价下拉框选项索引超出范围: {value}");
				return;
			}

			// 获取下拉框当前选择的文本内容
			string unitText = DropDown_Units[index].options[value].text;
			Debug.Log($"Unit dropdown text: '{unitText}' at index {index}");
			
			// 提取数字部分，例如"300美元/斤" -> 300
			// 查找第一个非数字字符的位置
			int endIndex = 0;
			for (int i = 0; i < unitText.Length; i++)
			{
				if (!char.IsDigit(unitText[i]))
				{
					endIndex = i;
					break;
				}
				endIndex = i + 1;
			}
			
			if (endIndex > 0 && int.TryParse(unitText.Substring(0, endIndex), out int unitPrice))
			{
				currentUnitPrices[index] = unitPrice;
				Debug.Log("unit price: " + currentUnitPrices[index]);
				CalculateTotalAmount(index);
			}
			else
			{
				Debug.LogWarning($"Failed to parse unit price from text: '{unitText}'");
			}
		}

		private void OnValueChangedTradeTerms(int index,int value)
		{
			// 检查索引范围
			if (index < 0 || index >= 3)
			{
				Debug.LogError($"贸易条款变化事件索引超出范围: {index}");
				return;
			}

			// 检查组件是否存在
			if (DropDown_TradeTerms[index] == null)
			{
				Debug.LogError($"产品行 {index} 的贸易条款下拉框为null！");
				return;
			}

			// 检查选项索引范围
			if (value < 0 || value >= DropDown_TradeTerms[index].options.Count)
			{
				Debug.LogError($"贸易条款下拉框选项索引超出范围: {value}");
				return;
			}

			// 获取下拉框当前选择的文本内容
			string tradeTermsText = DropDown_TradeTerms[index].options[value].text;
			Debug.Log($"Trade terms dropdown text: '{tradeTermsText}' at index {index}");
			
			// 提取数字部分，例如"1%" -> 1
			if (float.TryParse(tradeTermsText.Split('%')[0], out float discount))
			{
				currentDiscounts[index] = discount;
				Debug.Log("discount: " + currentDiscounts[index] + "%");
				CalculateTotalAmount(index);
			}
			else
			{
				Debug.LogWarning($"Failed to parse discount from text: '{tradeTermsText}'");
			}
		}
		
		private void CalculateTotalAmount(int index)
		{
			// 检查索引范围
			if (index < 0 || index >= 3)
			{
				Debug.LogError($"计算总金额索引超出范围: {index}");
				return;
			}

			// 检查金额文本组件是否存在
			if (Text_Amounts[index] == null)
			{
				Debug.LogError($"产品行 {index} 的金额文本组件为null！");
				return;
			}

			// 计算总金额：数量 × 单价 × (1 - 折扣率)
			float totalAmount = currentQuantitys[index] * currentUnitPrices[index] * (1f - currentDiscounts[index] / 100f);
			
			Text_Amounts[index].text = totalAmount.ToString("F2");
			Debug.Log($"Total Amount: {currentQuantitys[index]} × {currentUnitPrices[index]} × (1 - {currentDiscounts[index]}%) = {totalAmount:F2}");

			float total=0;
			for(int i=0;i<3;i++)
			{
				total+=currentQuantitys[i]*currentUnitPrices[i]*(1f-currentDiscounts[i]/100f);
			}
			Text_TotalAmount.text = total.ToString("F2");
			Debug.Log("Total Amount: " + total);
		}
	}
}
