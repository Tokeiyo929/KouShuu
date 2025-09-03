using System.Collections;
using System.Collections.Generic;
using DialogueEditor;
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
            ErrorTimes.Add(0);
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
}