using UnityEngine;
using System.Collections.Generic;
using QFramework.Example;

[CreateAssetMenu(fileName = "QuestionDatabase", menuName = "Quiz/Question Database")]
public class QuestionDatabase : ScriptableObject
{
    [System.Serializable]
    public class QuestionData
    {
        [Header("题目基本信息")]
        public string type; // 题目类型
        [TextArea(3, 5)]
        public string questionText; // 题目内容
        [TextArea(3, 5)]
        public string questionText_EN; // 题目内容
        
        [Header("选项")]
        public string optionA;
        public string optionB;
        public string optionC;
        public string optionD;
        public string optionE;
        public string optionA_EN;
        public string optionB_EN;
        public string optionC_EN;
        public string optionD_EN;
        public string optionE_EN;
        
        [Header("答案和解析")]
        public string correctAnswer; // 正确答案
        [TextArea(2, 4)]
        public string parse; // 答案解析
        [TextArea(2, 4)]
        public string parse_EN; // 答案解析
        
        public string[] GetOptions(GlobalEnums.Language language)
        {
            if(language == GlobalEnums.Language.Chinese)
            {
                return new string[] { optionA, optionB, optionC, optionD, optionE };
            }
            else
            {
                return new string[] { optionA_EN, optionB_EN, optionC_EN, optionD_EN, optionE_EN };
            }
        }
    }
    
    [Header("题目列表")]
    public List<QuestionData> questions = new List<QuestionData>();
    
    public QuestionData GetQuestion(int index, GlobalEnums.Language language)
    {
        if (index >= 0 && index < questions.Count)
        {
            return questions[index];
        }
        return null;
    }
    
    public int GetQuestionCount()
    {
        return questions.Count;
    }
} 