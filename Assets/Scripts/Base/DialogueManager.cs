using DialogueEditor;
using QFramework.Example;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public List<NPCConversation> Conversations;

    public List<int>ErrorTimes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
        }
        
        ErrorTimes = new List<int>();
        for(int i=0;i<7;i++)
        {
            //-1代码没有被选择过
            ErrorTimes.Add(-1);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playRandomConversation()
    {
        int randomIndex = Random.Range(1,3);//index:1/2

        if(randomIndex==1)
        {
            ConversationManager.Instance.StartConversation(Conversations[11]);
        }
        else if(randomIndex==2)
        {
            ConversationManager.Instance.StartConversation(Conversations[12]);
        }
    }

    public void onErrorChoose(int index)
    {
        //如果错误次数为-1，说明是第一次选择错误，先初始化为0，再加1
        if (ErrorTimes[index]==-1)
        {
            ErrorTimes[index]=0;
        }
        ErrorTimes[index]++;
        Debug.Log("ErrorTimes["+index+"]="+ErrorTimes[index]);

        for(int i=0;i<7;i++)
        {
            if(ErrorTimes[i]>=2)
            {
                ConversationManager.Instance.SetBool("isError_"+i,true);
                Debug.Log("ConversationManager.Instance.GetBool(\"isError_"+i+"\")="+ConversationManager.Instance.GetBool("isError_"+i));
            }
        }
    }
    public void SetConversationScore(int i)
    {
        List<int> errorTimes = DialogueManager.Instance.ErrorTimes;
        if (errorTimes[i] == -1){
            //结束对话后错误数据为-1，说明没有选择过错误，赋值为0
            DialogueManager.Instance.ErrorTimes[i] = 0;
        }
        //Debug.Log("Final ErrorTimes[" + i + "]=" + errorTimes[i]);
        float totalScore = 0;
        if (i == 0 || i == 1)
        {
            totalScore = 2f;
        }
        else
        {
            totalScore = 3f;
        }

        if (i != 2)
        {
            // 判断是否为 -1，若是则赋值 0 分
            if (errorTimes[i] == -1)
            {
                Global.ScoreList[i + 6] = 0f;
            }
            else if (errorTimes[i] == 0)
            {
                Global.ScoreList[i + 6] = totalScore;
            }
            else if (errorTimes[i] < 2)
            {
                Global.ScoreList[i + 6] = totalScore / 2;
            }
            else
            {
                Global.ScoreList[i + 6] = 0f;
            }
        }
        else
        {
            if (errorTimes[i] == -1)
            {
                Global.ScoreList[i + 6] = 0f;
            }
            else if (errorTimes[i] == 0)
            {
                Global.ScoreList[i + 6] = totalScore;
            }
            else
            {
                Global.ScoreList[i + 6] = 0;
            }
        }
    }
}