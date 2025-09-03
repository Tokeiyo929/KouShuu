using UnityEngine;
using ExcelDataReader; // 导入 ExcelDataReader，用于读取 Excel 文件
using System.IO;
using System.Data;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using DG.Tweening;
using System;
using QFramework;
using QFramework.Example;

public class DaTiView : MonoBehaviour
{
    [Header("选择题图片")]
    public Sprite defaultImage;
    public Sprite selectImage;
    public Sprite trueImage;
    public Sprite errorImage;

    // 保存各个题型对应的界面
    [Header("保存各个题型对应的界面")]
    public GameObject singleChoicePanel;
    public GameObject multipleChoicePanel;
    public GameObject fillBlankPanel;
    public GameObject trueFalsePanel;
    public GameObject shortAnswerPanel;

    // 保存各个题型对应的题目
    [Header("保存各个题型对应的题目")]
    public TextMeshProUGUI singleChoiceQuestion;
    public TextMeshProUGUI multipleChoiceQuestion;
    public TextMeshProUGUI fillBlankQuestion;
    public TextMeshProUGUI trueFalseQuestion;
    public TextMeshProUGUI shortAnswerQuestion;

    // 保存各个题型对应的解析
    [Header("保存各个题型对应的解析")]
    public TextMeshProUGUI singleChoiceTips;
    public TextMeshProUGUI multipleChoiceTips;
    public TextMeshProUGUI fillBlankTips;
    public TextMeshProUGUI trueFalseTips;
    public TextMeshProUGUI shortAnswerTips;

    //单选题的Toggle
    [Header("单选题的Toggle")]
    public Toggle optionAToggle;
    public Toggle optionBToggle;
    public Toggle optionCToggle;
    public Toggle optionDToggle;
    public Toggle optionEToggle;

    // 用于多选题的 Toggle 选项
    [Header("多选题的Toggle")]
    public Toggle optionA_MultipleChoice;
    public Toggle optionB_MultipleChoice;
    public Toggle optionC_MultipleChoice;
    public Toggle optionD_MultipleChoice;
    public Toggle optionE_MultipleChoice;

    [Header("填空题答案的预制体")]
    public GameObject fillBlankPrefabs;
    private List<string> fillBlankCorrectAnswers = new List<string>();//填空题的正确答案
    private List<TMP_InputField> fillBlankText = new List<TMP_InputField>();//填空题输入的答案

    // 用于判断题的 Toggle 选项（是/否）
    [Header("判断题的Toggle")]
    public Toggle trueToggle; // 是
    public Toggle falseToggle; // 否

    // 简答题答案
    [Header("简答题答案")]
    public TMP_InputField shortAnswerField;

    [Header("Check按钮")]
    public Button singleChoiceCheckBtn;
    public Button multipleChoiceCheckBtn;
    public Button fillBlankCheckBtn;
    public Button trueFalseCheckBtn;
    public Button shortAnswerCheckBtn;

    private List<Question> questions; // 存储读取的所有问题
    private bool hasBeCheck = false;
    private int nowQuestionIndex = 0;
    private QuestionDatabase questionData_SO;

    private List<int> scoreList=new List<int>();

    public  void Start()
    {
        //十道题
        scoreList=new List<int>{0,0,0,0,0,0,0,0,0,0};
        LoadQuestionDatabase();
        ToggleMgr();
        HideAllPanels();
        DisplayQuestion(nowQuestionIndex);
        /*
        string filePath = Path.Combine(Application.streamingAssetsPath, "Excel/ZYQuestion.xlsx");
        questions = LoadQuestionsFromExcel(filePath); // 读取 Excel 文件中的题目数据
        ToggleMgr();
        HideAllPanels();
        DisplayQuestion(nowQuestionIndex);
        */        
        /*
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
        //------------------------------------------------------------------------------------------------------
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(Screen.width, 0); // 假设从屏幕下方弹入

        // 动画：淡入 + 弹入
        
        DOTween.Sequence()
            .Append(rectTransform.DOAnchorPos(Vector2.zero, 1f).SetEase(Ease.OutExpo)) // 弹入效果
            .Join(DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1f, 1f)); // 同时淡入
        */
        Global.CurrentLanguage.RegisterWithInitValue(newValue=>
        {
            UpdateQuestionText(nowQuestionIndex);
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }


    public  void ClosePanel()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, Screen.height); // 假设从屏幕下方弹入

        // 动画：淡出 + 滑回右侧屏幕外
        DOTween.Sequence()
            .Append(DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0f, 0.2f)) // 淡出效果
            .Join(rectTransform.DOAnchorPos(new Vector2(Screen.width, 0), 0.2f).SetEase(Ease.InExpo)) // 滑出到右边
            .OnComplete(() =>
            {
                //base.ClosePanel(); // 动画结束后关闭面板

        });
    }


    // 定义一个 Question 类，用来存储每个问题的相关信息
    [System.Serializable]
    private class Question
    {
        public string type; // 问题类型（例如：单选题、多选题、判断题等）
        public string questionText; // 问题文本
        public string questionText_EN; // 问题文本
        public string[] options; // 问题选项（例如，单选或多选）
        public string[] options_EN; // 问题选项（例如，单选或多选）
        public string correctAnswer; // 正确答案
        public string parse; // 答案解析
        public string parse_EN; // 答案解析
    }

    // 读取 Excel 数据并返回一个问题列表
    List<Question> LoadQuestionsFromExcel(string path)
    {
        List<Question> questionsList = new List<Question>(); // 存储读取的所有问题

        // 使用 using 语句块自动关闭文件流
        using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
        {
            // 使用 ExcelReaderFactory 创建一个读取器来读取文件内容
            IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream);

            // 读取数据并将其转换为 DataSet
            DataSet result = excelReader.AsDataSet();

            // 获取 Excel 表格中的第一张表
            DataTable table = result.Tables[0];

            // 遍历 Excel 中的每一行数据，读取问题并存储
            foreach (DataRow row in table.Rows)
            {
                Question q = new Question(); // 创建一个新的 Question 对象

                // 将 Excel 中的各列数据赋值到 Question 对象的字段中
                q.type = row[1].ToString(); // 题目类型（单选题、多选题等）
                q.questionText = row[2].ToString(); // 题目内容
                q.options = new string[5] {
                row[3].ToString(), // 选项A
                row[4].ToString(), // 选项B
                row[5].ToString(), // 选项C
                row[6].ToString(), // 选项D
                row[7].ToString()  // 选项E
            };
                q.correctAnswer = row[8].ToString(); // 正确答案
                q.parse = row[9].ToString(); // 答案解析

                // 将 Question 对象添加到 questionsList 列表中
                questionsList.Add(q);
            }
        }

        return questionsList; // 返回问题列表
    }

    private void LoadQuestionDatabase()
    {
        questionData_SO = Resources.Load<QuestionDatabase>("ScriptableObjects/Question/QuestionDatabase");

        questions=new List<Question>();

        foreach (var questionData in questionData_SO.questions)
        {
            Question question = new Question();
            question.type = questionData.type;
            question.questionText = questionData.questionText;
            question.questionText_EN = questionData.questionText_EN;
            question.options = new string[5] {
                questionData.optionA, // 选项A
                questionData.optionB, // 选项B
                questionData.optionC, // 选项C
                questionData.optionD, // 选项D
                questionData.optionE  // 选项E
            };
            question.options_EN = new string[5] {
                questionData.optionA_EN, // 选项A
                questionData.optionB_EN, // 选项B
                questionData.optionC_EN, // 选项C
                questionData.optionD_EN, // 选项D
                questionData.optionE_EN  // 选项E
            };

            question.correctAnswer = questionData.correctAnswer;
            question.parse = questionData.parse;
            question.parse_EN = questionData.parse_EN;
            questions.Add(question);
        }

    }

    // 根据问题索引，显示问题并根据问题类型显示相应的 UI
    void DisplayQuestion(int index)
    {
        if (index < 0 || index >= questions.Count)
        {
            NextPanel();
            return; // 如果索引无效，则不做任何操作
        }

        InitPage();



        Question question = questions[index]; // 获取当前问题

        Debug.Log("Question: " + question.questionText); // 在控制台输出当前问题内容

        // 根据题目类型显示不同的 UI 元素
        HideAllPanels(); // 首先隐藏所有面板


        if (question.type == "单选题") // 如果是单选题
        {
            singleChoicePanel.SetActive(true); // 显示单选题的面板

            singleChoiceCheckBtn.onClick.AddListener(() =>
            {
                CheckSingleChoiceAnswer(question);
            });

            if(Global.CurrentLanguage.Value == GlobalEnums.Language.Chinese)
            {
            singleChoiceQuestion.text = question.questionText;
            singleChoiceTips.text = question.parse;
            }
            else
            {
                singleChoiceQuestion.text = question.questionText_EN;
                singleChoiceTips.text = question.parse_EN;
            }

            optionAToggle.gameObject.SetActive(question.options[0]!="");
            optionBToggle.gameObject.SetActive(question.options[1]!="");
            optionCToggle.gameObject.SetActive(question.options[2]!="");
            optionDToggle.gameObject.SetActive(question.options[3]!="");
            optionEToggle.gameObject.SetActive(question.options[4]!="");

            // 设置单选题选项
            if(Global.CurrentLanguage.Value == GlobalEnums.Language.Chinese)
            {
                optionAToggle.GetComponentInChildren<TextMeshProUGUI>().text = "A."+question.options[0];
                optionBToggle.GetComponentInChildren<TextMeshProUGUI>().text = "B." + question.options[1];
                optionCToggle.GetComponentInChildren<TextMeshProUGUI>().text = "C." + question.options[2];
                optionDToggle.GetComponentInChildren<TextMeshProUGUI>().text = "D." + question.options[3];
                optionEToggle.GetComponentInChildren<TextMeshProUGUI>().text = "E." + question.options[4];
            }
            else
            {
                optionAToggle.GetComponentInChildren<TextMeshProUGUI>().text = "A."+question.options_EN[0];
                optionBToggle.GetComponentInChildren<TextMeshProUGUI>().text = "B." + question.options_EN[1];
                optionCToggle.GetComponentInChildren<TextMeshProUGUI>().text = "C." + question.options_EN[2];
                optionDToggle.GetComponentInChildren<TextMeshProUGUI>().text = "D." + question.options_EN[3];
                optionEToggle.GetComponentInChildren<TextMeshProUGUI>().text = "E." + question.options_EN[4];
            }
        }
        else if (question.type == "多选题") // 如果是多选题
        {
            multipleChoicePanel.SetActive(true); // 显示多选题的面板

            multipleChoiceCheckBtn.onClick.AddListener(() =>
            {
                CheckMultipleChoiceAnswer(question);
            });

            if(Global.CurrentLanguage.Value == GlobalEnums.Language.Chinese)
            {
                multipleChoiceQuestion.text = question.questionText;
                multipleChoiceTips.text = question.parse;
            }
            else
            {
                multipleChoiceQuestion.text = question.questionText_EN;
                multipleChoiceTips.text = question.parse_EN;
            }

            optionA_MultipleChoice.gameObject.SetActive(question.options[0]!="");
            optionB_MultipleChoice.gameObject.SetActive(question.options[1]!="");
            optionC_MultipleChoice.gameObject.SetActive(question.options[2]!="");
            optionD_MultipleChoice.gameObject.SetActive(question.options[3]!="");
            optionE_MultipleChoice.gameObject.SetActive(question.options[4]!="");


            // 设置多选题选项
            if(Global.CurrentLanguage.Value == GlobalEnums.Language.Chinese)
            {
                optionA_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "A." + question.options[0];
                optionB_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "B." + question.options[1];
                optionC_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "C." + question.options[2];
                optionD_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "D." + question.options[3];
                optionE_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "E." + question.options[4];
            }
            else
            {
                optionA_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "A." + question.options_EN[0];
                optionB_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "B." + question.options_EN[1];
                optionC_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "C." + question.options_EN[2];
                optionD_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "D." + question.options_EN[3];
                optionE_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "E." + question.options_EN[4];
            }
        }
        else if (question.type == "填空题") // 如果是填空题
        {
            fillBlankCheckBtn.onClick.AddListener(() =>
            {
                CheckFillBlankPanelAnswer(question);
            });

            if(Global.CurrentLanguage.Value == GlobalEnums.Language.Chinese)
            {
                fillBlankQuestion.text = question.questionText;
                fillBlankTips.text = question.parse;
            }
            else
            {
                fillBlankQuestion.text = question.questionText_EN;
                fillBlankTips.text = question.parse_EN;
            }


            fillBlankPanel.SetActive(true); // 显示填空题的面板

            fillBlankCorrectAnswers.Clear();
            fillBlankText.Clear();
            fillBlankCorrectAnswers = question.correctAnswer.Split(',').ToList(); // 解析正确答案
            GameObject parentAnswer = transform.Find("填空题").Find("答案").gameObject;

            foreach (Transform child in parentAnswer.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < fillBlankCorrectAnswers.Count; i++)
            {
                GameObject preFillBlank = Instantiate(fillBlankPrefabs, parentAnswer.transform);
                //找到preFillBlank下的第一个TMP组件
                preFillBlank.GetComponentInChildren<TextMeshProUGUI>().text = $"{i + 1}、";

                TMP_Text placeholderText = preFillBlank.GetComponent<TMP_InputField>().placeholder.GetComponent<TMP_Text>();
                placeholderText.text = $"第{i + 1}空答案";
                fillBlankText.Add(preFillBlank.GetComponent<TMP_InputField>());
            }


        }
        else if (question.type == "判断题") // 如果是判断题
        {
            trueFalsePanel.SetActive(true); // 显示判断题的面板

            trueFalseCheckBtn.onClick.AddListener(() =>
            {
                CheckTrueFalseAnswer(question);
            });

            if(Global.CurrentLanguage.Value == GlobalEnums.Language.Chinese)
            {
                trueFalseQuestion.text = question.questionText;
                trueFalseTips.text = question.parse;
            }
            else
            {
                trueFalseQuestion.text = question.questionText_EN;
                trueFalseTips.text = question.parse_EN;
            }

            if(Global.CurrentLanguage.Value == GlobalEnums.Language.Chinese)
            {
                trueToggle.GetComponentInChildren<TextMeshProUGUI>().text = "A." + question.options[0];
                falseToggle.GetComponentInChildren<TextMeshProUGUI>().text = "B." + question.options[1];
            }
            else
            {
                trueToggle.GetComponentInChildren<TextMeshProUGUI>().text = "A." + question.options_EN[0];
                falseToggle.GetComponentInChildren<TextMeshProUGUI>().text = "B." + question.options_EN[1];
            }
        }
        else if (question.type == "简答题") // 如果是简答题
        {
            shortAnswerPanel.SetActive(true); // 显示简答题的面板
            shortAnswerCheckBtn.onClick.AddListener(() =>
            {
                CheckShortAnswerAnswer(question);
            });

            if(Global.CurrentLanguage.Value == GlobalEnums.Language.Chinese)
            {
                shortAnswerQuestion.text = question.questionText;
                shortAnswerTips.text = question.parse;
            }
            else
            {
                shortAnswerQuestion.text = question.questionText_EN;
                shortAnswerTips.text = question.parse_EN;
            }

        }
    }


    // 判断用户选择的单选题答案是否正确
    private void CheckSingleChoiceAnswer(Question question)
    {
        if (hasBeCheck)
        {
            nowQuestionIndex++;
            DisplayQuestion(nowQuestionIndex);
            return;
        }

        // 创建一个字典映射答案选项和 Toggle
        var optionToggles = new Dictionary<string, Toggle>
    {
        { "A", optionAToggle },
        { "B", optionBToggle },
        { "C", optionCToggle },
        { "D", optionDToggle },
        { "E", optionEToggle }
    };

        // 查找选中的答案
        string selectedAnswer = optionToggles.FirstOrDefault(x => x.Value.isOn).Key;

        if (string.IsNullOrEmpty(selectedAnswer))
        {
            Debug.Log("没有选中任何选项");
            return;
        }

        // 判断选择的答案是否正确
        if (selectedAnswer == question.correctAnswer)
        {
            scoreList[nowQuestionIndex]=1;
            Debug.Log("答案正确！");
            optionToggles[selectedAnswer].GetComponent<Image>().sprite = trueImage; // 设置正确答案的图片
        }
        else
        {
            scoreList[nowQuestionIndex]=0;
            Debug.Log("答案错误！正确答案是：" + question.correctAnswer);
            optionToggles[selectedAnswer].GetComponent<Image>().sprite = errorImage;
            optionToggles[question.correctAnswer].GetComponent<Image>().sprite = trueImage; // 设置正确答案的图片
            singleChoiceTips.gameObject.SetActive(true);
        }

        hasBeCheck = true;
        SetAnswerInteractable(false);
    }


    // 判断用户选择的多选题答案是否正确
    private void CheckMultipleChoiceAnswer(Question question)
    {
        if (hasBeCheck)
        {
            nowQuestionIndex++;
            DisplayQuestion(nowQuestionIndex);
            return;
        }
        hasBeCheck = true;
        SetAnswerInteractable(false);

        // 创建一个字典映射答案选项和 Toggle
        var optionToggles = new Dictionary<string, Toggle>
    {
        { "A", optionA_MultipleChoice },
        { "B", optionB_MultipleChoice },
        { "C", optionC_MultipleChoice },
        { "D", optionD_MultipleChoice },
        { "E", optionE_MultipleChoice }
    };

        // 获取用户选择的答案
        List<string> selectedAnswers = new List<string>();

        if (optionA_MultipleChoice.isOn)
        {
            selectedAnswers.Add("A");
        }
        if (optionB_MultipleChoice.isOn)
        {
            selectedAnswers.Add("B");
        }
        if (optionC_MultipleChoice.isOn)
        {
            selectedAnswers.Add("C");
        }
        if (optionD_MultipleChoice.isOn)
        {
            selectedAnswers.Add("D");
        }
        if (optionE_MultipleChoice.isOn)
        {
            selectedAnswers.Add("E");
        }

        // 将答案按逗号分隔并进行比较
        List<string> correctAnswers = question.correctAnswer.Split(',').ToList(); // 解析正确答案

        // 检查选中的答案是否包含所有正确答案
        if (selectedAnswers.Count == correctAnswers.Count && !selectedAnswers.Except(correctAnswers).Any())
        {
            Debug.Log("答案正确！");

        }
        else
        {
            Debug.Log("答案错误！正确答案是：" + question.correctAnswer);
            multipleChoiceTips.gameObject.SetActive(true);
        }

        foreach (var tog in optionToggles.Values)
        {
            tog.isOn = false;
        }

        foreach (var str in correctAnswers)
        {
            if (optionToggles.ContainsKey(str))
            {
                optionToggles[str].GetComponent<Image>().sprite = trueImage;
            }
        }

    }

    //判断用户选择的填空题答案是否正确
    private void CheckFillBlankPanelAnswer(Question question)
    {
        if (hasBeCheck)
        {
            nowQuestionIndex++;
            DisplayQuestion(nowQuestionIndex);
            foreach (var txt in fillBlankText)
            {
                txt.textComponent.color = Color.black;
                txt.text = string.Empty;
            }

            return;
        }
        hasBeCheck = true;
        SetAnswerInteractable(false);

        for (int i = 0; i < fillBlankCorrectAnswers.Count; i++)
        {
            if (!fillBlankText[i].text.Equals(fillBlankCorrectAnswers[i]))
            {
                //回答错误
                Debug.Log("回答错误");
                scoreList[nowQuestionIndex+i]=0;
                fillBlankText[i].textComponent.color = Color.red;
                fillBlankText[i].text = fillBlankCorrectAnswers[i];
                fillBlankTips.gameObject.SetActive(true);
            }
            else
            {
                scoreList[nowQuestionIndex+i]=1;
                fillBlankText[i].textComponent.color = Color.green;
            }
        }



        //回答正确
        Debug.Log("回答正确");
    }


    // 判断用户选择的判断题答案是否正确
    private void CheckTrueFalseAnswer(Question question)
    {
        if (hasBeCheck)
        {
            nowQuestionIndex++;
            DisplayQuestion(nowQuestionIndex);
            return;
        }


        // 创建一个字典映射答案选项和 Toggle
        var optionToggles = new Dictionary<string, Toggle>
    {
        { "是", trueToggle },
        { "否", falseToggle },
    };

        // 查找选中的答案
        string selectedAnswer = optionToggles.FirstOrDefault(x => x.Value.isOn).Key;

        if (string.IsNullOrEmpty(selectedAnswer))
        {
            Debug.Log("没有选中任何选项");
            return;
        }

        // 判断选择的答案是否正确
        if (selectedAnswer == question.correctAnswer)
        {
            scoreList[nowQuestionIndex]=1;
            Debug.Log("答案正确！");
            optionToggles[selectedAnswer].GetComponent<Image>().sprite = trueImage; // 设置正确答案的图片
        }
        else
        {
            scoreList[nowQuestionIndex]=0;
            Debug.Log("答案错误！正确答案是：" + question.correctAnswer);
            optionToggles[selectedAnswer].GetComponent<Image>().sprite = errorImage;
            optionToggles[question.correctAnswer].GetComponent<Image>().sprite = trueImage; // 设置正确答案的图片
            trueFalseTips.gameObject.SetActive(true);
        }

        hasBeCheck = true;
        SetAnswerInteractable(false);

    }

    //判断用户简答题答案是否正确
    private void CheckShortAnswerAnswer(Question question)
    {
        if (hasBeCheck)
        {
            nowQuestionIndex++;
            DisplayQuestion(nowQuestionIndex);
            return;
        }
        hasBeCheck = true;

        // 获取用户输入的文本
        string userInput = shortAnswerField.text.Trim().ToLower();  // 去除空格并转换为小写，避免大小写问题

        // 将正确答案按逗号分隔并转为小写
        List<string> correctAnswers = question.correctAnswer.Split(',').Select(a => a.Trim().ToLower()).ToList();

        bool isCorrect = true;  // 假设答案正确

        // 检查每个关键词是否都包含在用户输入的文本中
        foreach (var correctAnswer in correctAnswers)
        {
            if (!userInput.Contains(correctAnswer))
            {
                isCorrect = false;  // 如果任何一个关键词未包含，设置为错误
                break;
            }
        }
        shortAnswerTips.gameObject.SetActive(true);
        // 输出结果
        if (isCorrect)
        {
            scoreList[nowQuestionIndex]=1;
            Debug.Log("nowQuestionIndex: "+nowQuestionIndex);
            shortAnswerTips.color = Color.green;
            shortAnswerTips.text = "答案正确！";
        }
        else
        {
            scoreList[nowQuestionIndex]=0;
            Debug.Log("nowQuestionIndex: "+nowQuestionIndex);
            shortAnswerTips.color = Color.red;
            shortAnswerTips.text = question.parse;
        }
    }

    //重置按钮事件
    private void ResetCheckBtn()
    {
        singleChoiceCheckBtn.onClick.RemoveAllListeners();
        multipleChoiceCheckBtn.onClick.RemoveAllListeners();
        fillBlankCheckBtn.onClick.RemoveAllListeners();
        trueFalseCheckBtn.onClick.RemoveAllListeners();
        shortAnswerCheckBtn.onClick.RemoveAllListeners();
    }

    //重置页面状态
    private void InitPage()
    {
        hasBeCheck = false;
        ResetCheckBtn();
        ResetAnswer();
        SetAnswerInteractable(true);
        singleChoiceTips.gameObject.SetActive(false);
        multipleChoiceTips.gameObject.SetActive(false);
        fillBlankTips.gameObject.SetActive(false);
        trueFalseTips.gameObject.SetActive(false);
        shortAnswerTips.gameObject.SetActive(false);
    }

    //启用禁用所有答案的interactable
    private void SetAnswerInteractable(bool active)
    {

        optionAToggle.interactable = active;
        optionBToggle.interactable = active;
        optionCToggle.interactable = active;
        optionDToggle.interactable = active;
        optionEToggle.interactable = active;

        optionA_MultipleChoice.interactable = active;
        optionB_MultipleChoice.interactable = active;
        optionC_MultipleChoice.interactable = active;
        optionD_MultipleChoice.interactable = active;
        optionE_MultipleChoice.interactable = active;

        foreach (var txt in fillBlankText)
        {
            txt.interactable = active;
        }

        trueToggle.interactable = active; // 是
        falseToggle.interactable = active; // 否
    }

    //重置所有答案状态
    private void ResetAnswer()
    {
        // 获取当前物体下所有子物体中的 Toggle 组件
        Toggle[] toggles = GetComponentsInChildren<Toggle>();

        // 遍历所有的 Toggle 组件，为它们添加事件监听器
        foreach (var toggle in toggles)
        {
            toggle.GetComponent<Image>().sprite = defaultImage;
            toggle.isOn = false;
        }

      
    }

    private void ToggleMgr()
    {
        // 获取当前物体下所有子物体中的 Toggle 组件
        Toggle[] toggles = GetComponentsInChildren<Toggle>();

        // 遍历所有的 Toggle 组件，为它们添加事件监听器
        foreach (var toggle in toggles)
        {
            // 获取当前 Toggle 组件的 Image 组件
            Image toggleImage = toggle.GetComponent<Image>();

            if (toggleImage != null)
            {
                Debug.Log("添加事件");

                // 初始化图片，根据 Toggle 初始状态设置图片
                toggleImage.sprite = toggle.isOn ? selectImage : defaultImage;

                // 为 Toggle 添加监听器，根据状态切换图片
                toggle.onValueChanged.AddListener((isOn) => OnToggleValueChanged(isOn, toggleImage));
            }
        }
    }

    // 当 Toggle 状态变化时，更新图片
    private void OnToggleValueChanged(bool isOn, Image toggleImage)
    {
        if (isOn)
        {
            toggleImage.sprite = selectImage;  // 设置选中后的图片
        }
        else
        {
            toggleImage.sprite = defaultImage;  // 设置未选中时的默认图片
        }
    }

    // 隐藏所有面板
    void HideAllPanels()
    {
        singleChoicePanel.SetActive(false);
        multipleChoicePanel.SetActive(false);
        fillBlankPanel.SetActive(false);
        trueFalsePanel.SetActive(false);
        shortAnswerPanel.SetActive(false);
    }


    public void NextPanel()
    {
        //关闭所有界面，打开Title
        UIKit.ClosePanel<UISelectPanel>();
        UIKit.OpenPanel<UILevle2TransitionPanel>(UILevel.Common, null, null, "UIPrefabs/UILevle2TransitionPanel");

        int score=0;
        foreach(var item in scoreList)
        {
            score+=item;
        }

        if(Global.ScoreList.Count==0)
        {
            for(int i=0;i<18;i++)
            {
                Global.ScoreList.Add(0f);
            }
        }
        Global.ScoreList[0]=score;
    }
    private Transform GetRootParent(Transform current)
    {
        while (current.parent != null)
        {
            current = current.parent;
        }
        return current;
    }

    private void UpdateQuestionText(int index)
    {
        if (index < 0 || index >= questions.Count)
        {
            return;
        }

        Question question = questions[index];

        // 根据当前语言更新题目和解析文本
        if (Global.CurrentLanguage.Value == GlobalEnums.Language.Chinese)
        {
            // 更新题目文本
            singleChoiceQuestion.text = question.questionText;
            multipleChoiceQuestion.text = question.questionText;
            fillBlankQuestion.text = question.questionText;
            trueFalseQuestion.text = question.questionText;
            shortAnswerQuestion.text = question.questionText;
            
            // 更新解析文本
            singleChoiceTips.text = question.parse;
            multipleChoiceTips.text = question.parse;
            fillBlankTips.text = question.parse;
            trueFalseTips.text = question.parse;
            shortAnswerTips.text = question.parse;
        }
        else
        {
            // 更新题目文本
            singleChoiceQuestion.text = question.questionText_EN;
            multipleChoiceQuestion.text = question.questionText_EN;
            fillBlankQuestion.text = question.questionText_EN;
            trueFalseQuestion.text = question.questionText_EN;
            shortAnswerQuestion.text = question.questionText_EN;
            
            // 更新解析文本
            singleChoiceTips.text = question.parse_EN;
            multipleChoiceTips.text = question.parse_EN;
            fillBlankTips.text = question.parse_EN;
            trueFalseTips.text = question.parse_EN;
            shortAnswerTips.text = question.parse_EN;
        }

        // 根据问题类型更新选项文本
        if (question.type == "单选题")
        {
            if (Global.CurrentLanguage.Value == GlobalEnums.Language.Chinese)
            {
                optionAToggle.GetComponentInChildren<TextMeshProUGUI>().text = "A." + question.options[0];
                optionBToggle.GetComponentInChildren<TextMeshProUGUI>().text = "B." + question.options[1];
                optionCToggle.GetComponentInChildren<TextMeshProUGUI>().text = "C." + question.options[2];
                optionDToggle.GetComponentInChildren<TextMeshProUGUI>().text = "D." + question.options[3];
                optionEToggle.GetComponentInChildren<TextMeshProUGUI>().text = "E." + question.options[4];
            }
            else
            {
                optionAToggle.GetComponentInChildren<TextMeshProUGUI>().text = "A." + question.options_EN[0];
                optionBToggle.GetComponentInChildren<TextMeshProUGUI>().text = "B." + question.options_EN[1];
                optionCToggle.GetComponentInChildren<TextMeshProUGUI>().text = "C." + question.options_EN[2];
                optionDToggle.GetComponentInChildren<TextMeshProUGUI>().text = "D." + question.options_EN[3];
                optionEToggle.GetComponentInChildren<TextMeshProUGUI>().text = "E." + question.options_EN[4];
            }
        }
        else if (question.type == "多选题")
        {
            if (Global.CurrentLanguage.Value == GlobalEnums.Language.Chinese)
            {
                optionA_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "A." + question.options[0];
                optionB_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "B." + question.options[1];
                optionC_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "C." + question.options[2];
                optionD_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "D." + question.options[3];
                optionE_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "E." + question.options[4];
            }
            else
            {
                optionA_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "A." + question.options_EN[0];
                optionB_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "B." + question.options_EN[1];
                optionC_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "C." + question.options_EN[2];
                optionD_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "D." + question.options_EN[3];
                optionE_MultipleChoice.GetComponentInChildren<TextMeshProUGUI>().text = "E." + question.options_EN[4];
            }
        }
        else if (question.type == "判断题")
        {
            if (Global.CurrentLanguage.Value == GlobalEnums.Language.Chinese)
            {
                trueToggle.GetComponentInChildren<TextMeshProUGUI>().text = "A." + question.options[0];
                falseToggle.GetComponentInChildren<TextMeshProUGUI>().text = "B." + question.options[1];
            }
            else
            {
                trueToggle.GetComponentInChildren<TextMeshProUGUI>().text = "A." + question.options_EN[0];
                falseToggle.GetComponentInChildren<TextMeshProUGUI>().text = "B." + question.options_EN[1];
            }
        }
    }
}
