using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using Baidu.Aip.Speech;
using Duck.Http;
using Duck.Http.Service;
using SimpleJSON;
using Newtonsoft.Json;
using DG.Tweening;
//using uMicrophoneWebGL.Samples;
using System.Security.Cryptography;

[RequireComponent(typeof(AudioSource))]
public class MicPhone : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("bot_id")]
    public TextMeshProUGUI bot_id_text;

    //语音识别的结果
    public string asrResult = string.Empty;

    //存储录音的片段
    [HideInInspector]
    public AudioClip saveAudioClip;

    //当前按钮下的文本
    public TextMeshProUGUI textBtn;

    //显示结果的文本
    public TMP_InputField textResult;

    public TextMeshProUGUI textAnswer;

    public AiUiView aiUiView;
    public GameObject waitui;

    public GptStreamWrapper streamWrapper;
    public GameObject stopui;

   // public MicrophoneRecorder microphoneRecorder;
    private void Awake()
    {
      
    }
    void Start()
    {
      
    }

  
    public bool StartRecording()
    {
       
        aiUiView.StopIdle();
        aiUiView.ai_animator.Play("思考1");
        aiUiView.ai_animator1.Play("思考1");
        return true;

    }

    /// <summary>
    /// 按下录音按钮
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        GptStreamWrapper.instance.ispause = false;
        if (GptStreamWrapper.iscanspeak)
        {
            GetComponent<Animator>().Play("语音动效");
            transform.GetChild(0).gameObject.SetActive(false);
            textBtn.text = "松开识别";
            StopAllCoroutines();
            // GetComponent<Image>().color = new Color32(233, 181, 83, 255);
            //StartRecording();
            aiUiView.StopIdle();
            aiUiView.ai_animator.Play("思考1");
            aiUiView.ai_animator1.Play("思考1");
        
           // microphoneRecorder.ToggleRecord();
        }
       
        
    }

    /// <summary>
    /// 放开录音按钮
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (GptStreamWrapper.iscanspeak)
        {
            GetComponent<Animator>().Play("empty");
            transform.GetChild(0).gameObject.SetActive(true);
            textBtn.text = "按住说话";
            //   microphoneRecorder.ToggleRecord();
            ////GetComponent<Image>().color = new Color32(255, 255, 255, 255);


            //asrResult = string.Empty;
            //var clipdata = microphoneRecorder._clip;
            //Debug.Log("旧音频声道" + clipdata.channels + "采样率" + clipdata.frequency);
            //clipdata = ResampleClip(microphoneRecorder._clip, 16000);
            //Debug.Log("新音频声道" + clipdata.channels + "采样率" + clipdata.frequency);
            //float[] samples = new float[clipdata.samples * clipdata.channels];
            //clipdata.GetData(samples, 0);
            ////GetComponent<AudioSource>().clip = clipdata;
            ////GetComponent<AudioSource>().Play();
            //short[] samplesShort = new short[samples.Length];
            //for (int i = 0; i < samples.Length; i++)
            //{
            //    samplesShort[i] = (short)(samples[i] * short.MaxValue);
            //}
            ////var samplesShort = new short[samples.Length];
            ////for (var index = 0; index < samples.Length; index++)
            ////{
            ////    samplesShort[index] = (short)(samples[index] * short.MaxValue);
            ////}
            //byte[] byteArray = new byte[samplesShort.Length * sizeof(short)];
            //Buffer.BlockCopy(samplesShort, 0, byteArray, 0, byteArray.Length);

            //AsrData(byteArray);

        }

    }
    /// <summary>
    /// 音频重采样
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="newSampleRate"></param>
    /// <returns></returns>
    public AudioClip ResampleClip(AudioClip clip, int newSampleRate)
    {
        int samples = clip.samples;
        int channels = clip.channels;
        float[] data = new float[samples * channels];
        clip.GetData(data, 0);

        float resampleRatio = (float)newSampleRate / clip.frequency;
        int newSamples = Mathf.FloorToInt(samples * resampleRatio);
        float[] newData = new float[newSamples * channels];

        for (int i = 0; i < newSamples; i++)
        {
            int oldIndex = Mathf.FloorToInt(i / resampleRatio);
            for (int j = 0; j < channels; j++)
            {
                newData[i * channels + j] = data[oldIndex * channels + j];
            }
        }

        AudioClip newClip = AudioClip.Create(clip.name + "_resampled", newSamples, channels, newSampleRate, false);
        newClip.SetData(newData, 0);
        return newClip;
    }
    public void AsrData(byte[] bytes)
    {
        Debug.Log(bytes.Length);
        if (bytes.Length == 0)
        {
            Debug.Log("输入的数据为空");
        }
        else
        {
            StartCoroutine(BaiduApi_GetText(bytes));
            
        }


    }

    public void InputPostAi()
    {
        GptStreamWrapper.instance.ispause = false;
        if (textResult.text.Length<=1)
        {
            textResult.text = "";
            textResult.placeholder.gameObject.SetActive(true);
            return;
           
        }
        if (GptStreamWrapper.iscanspeak)
        {
            if (postAiDataIE != null)
                StopCoroutine(postAiDataIE);
            GetComponent<Animator>().Play("empty");
            transform.GetChild(0).gameObject.SetActive(true);
           // waitui.SetActive(true);
            postAiDataIE = PostAiDataIE(textResult.text);
            StartCoroutine(postAiDataIE);
        }
    }
  
    IEnumerator postAiDataIE;
    IEnumerator PostAiDataIE(string str)
    {
        yield return null;
        PostAiData data = new PostAiData
        {            
            stream = true,
            additional_messages = new List<AdditionalMessage>
            {
                new AdditionalMessage
                {
                    role = "user",
                    content = str
                }
            }
        };
        Debug.Log($"赋值bot_id:{data.bot_id}");
        Debug.Log("提问：" + JsonUtility.ToJson(data));
        //GetComponent<CanvasGroup>().alpha = 0;
        //GetComponent<CanvasGroup>().interactable = false;
        stopui.gameObject.SetActive(true);
//#if UNITY_WEBGL
        streamWrapper.WebglPostAi(JsonUtility.ToJson(data), textAnswer);

        waitui.SetActive(true);
        while (!streamWrapper.iscomplete)
        {
            yield return null;
        }
        Complete();

//#endif
//#if !UNITY_WEBGL
// PostAiDataAs(JsonUtility.ToJson(data));
//#endif
    }

    public void Complete()
    {
        waitui.SetActive(false);
        Debug.Log("ai回答完成");
        stopui.gameObject.SetActive(false);
        //GetComponent<CanvasGroup>().alpha = 1;
        //GetComponent<CanvasGroup>().interactable = true;
        StartCoroutine(WaitAndContinue());
    }
    private async void PostAiDataAs(string postData)
    {
        // 调用 PostRequestToStringAsync 方法
    
        await streamWrapper.PostRequestToStringAsync(postData,textAnswer);
       
        Debug.Log("ai回答完成");
        stopui.gameObject.SetActive(false);
        //GetComponent<CanvasGroup>().alpha = 1;
        //GetComponent<CanvasGroup>().interactable = true;
        StartCoroutine(WaitAndContinue());
    }
    IEnumerator WaitAndContinue()
    {
        yield return new WaitForSeconds(2.633f);
        Debug.Log("回答完成");
       // aiUiView.StartCoroutine(aiUiView.idleie);
        waitui.SetActive(false);
    }


    string audiototextURL= "https://vop.baidu.com/server_api";

    public void Test()
    {
        textResult.text = "颅咽管瘤患者实验室检查需要检查什么？";
        textAnswer.text = textAnswer.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().text;
    }

    IEnumerator BaiduApi_GetText(byte[] bytes)
    {
        yield return null;
        Guid guid = Guid.NewGuid();
        string base64Guid = Convert.ToBase64String(guid.ToByteArray());
        base64Guid = base64Guid.Replace("=", "").Replace("+", "").Replace("/", "");
        var baidudata2 = new BaiDuData2
        {
            format = "pcm",
            rate = 16000,
            cuid = base64Guid,
            token = streamWrapper.baidutoken,
            speech = Convert.ToBase64String(bytes),
            len = bytes.Length,
        };
        var postdata= JsonUtility.ToJson(baidudata2);
        Debug.Log("测试Post Data:" + postdata);
        Http.PostJson("https://docai.wehuatech.com/stage-api/coze/uploadBaidu", postdata)
        .SetHeader("Content-Type", "application/json")
        .SetHeader("Accept", "application/json")

        .OnSuccess(
            (HttpResponse response) =>
            {
                Debug.Log(response.Text);
                string str = response.Text.Replace("\\\"", "\"").Replace("\\\\", "\\");
                Debug.Log(str);
                var data = JSON.Parse(str);
                var msg = JSON.Parse(data["msg"]);
                var result = msg["result"][0].Value;
                textResult.text = result;
                Debug.Log("转文本" + result);
                if (postAiDataIE != null)
                    StopCoroutine(postAiDataIE);

             
                //postAiDataIE = PostAiDataIE(result);
                //StartCoroutine(postAiDataIE);
            }
        )
        .OnError(
            (HttpResponse response) =>
            {
                Debug.Log(response.Text);
            }
        )
        .OnNetworkError(
            (HttpResponse response) =>
            {
                Debug.Log(response.Text);
            }
        )
        .Send();
    }

   
}
[Serializable]
public class PostAiData
{
    public string conversation_id = "123";
    public string bot_id = "7454162614919135272";
    public string user_id = "3411125730547116";
    public List<AdditionalMessage> additional_messages;
    public bool stream = true;
    public bool auto_save_history=true;
}
[Serializable]
public class AdditionalMessage
{
    public string role = "user";
    public string content;
    public string content_type = "text";
}
public class BaiDuData2
{
    public string format;
    public int rate;
    public int channel = 1;
    public string cuid = "";
    public string token = "";
    public string ctp = "1";
    public string speech;
    public int len ;

}

