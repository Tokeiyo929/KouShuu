using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "BusinessCommunication", 
    menuName = "Data/BusinessCommunication", 
    order = 2)]
public class BusinessCommunication : ScriptableObject
{
    [System.Serializable]
    public class BusinessCommunicationEntry
    {
        
        [Tooltip("标题")]public string cn_Title;
        [Tooltip("中文内容")][TextArea]public string cn_Content;
        [Tooltip("英文内容")][TextArea]public string en_Content;
        [Tooltip("国语音频名称")]public string cn_AudioName;
        [Tooltip("粤语音频名称")]public string ct_AudioName;
        [Tooltip("英语音频名称")]public string en_AudioName;
    }

    public List<BusinessCommunicationEntry> entries = new List<BusinessCommunicationEntry>();

    public BusinessCommunicationEntry GetEntry(string key)
    {
        return entries.Find(entry => entry.cn_Title == key);
    }
}
