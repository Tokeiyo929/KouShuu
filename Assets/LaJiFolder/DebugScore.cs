using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework.Example;

public class DebugScore : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PrintScore()
    {
        string scoreStr = "Score List: ";
        foreach (var score in Global.ScoreList)
        {
            scoreStr += score + ", ";
        }
        Debug.Log(scoreStr);
        string errorTime = "Error Time: ";
        foreach(int item in DialogueManager.Instance.ErrorTimes)
        {
            errorTime += item + ", ";
        }
        Debug.Log(errorTime);
    }
}
