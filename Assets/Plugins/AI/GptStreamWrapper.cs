using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using UnityEngine;
using System;
using SimpleJSON;
using TMPro;
using Baidu.Aip.Speech;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Android;
using Duck.Http.Service;
using Duck.Http;
using DG.Tweening;
using Newtonsoft.Json;
using LitJson;
using System.Text.RegularExpressions;

public class GptStreamWrapper:MonoBehaviour
{
    //百度语音识别相关key
    string appId = "34310537";
    string apiKey = "P4w4LGPkvuV3pDWZgekAOuwi";              
    string secretKey = "zpxtFbwSbTG8a8G1IrP7HanR2R1VkqM2";     
    const string GPT_URL = "https://api.coze.cn/v3/chat";
    const string Token_URL = "https://aip.baidubce.com/oauth/2.0/token";

    string getAudio_URL = "https://tsn.baidu.com/text2audio";
    const int TIMEOUT_TIME = 10000; // 超时时间
    Tts clientGet;//语音合成
    public AudioSource audioSource;
    public List<AudioClip> audioclips = new List<AudioClip>(); 
    public GameObject tip;
    public static bool iscanspeak=true;
    public AiUiView aiUiView;
    public string GptUrl { get; } = GPT_URL;
    public string TokenUrl { get; } = Token_URL;
    public static GptStreamWrapper instance;
    public bool ispause;
    public bool iscomplete;


    [Header("令牌")]
    public TextMeshProUGUI authorization_text;
    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
        clientGet = new Tts(apiKey, secretKey);
        StartCoroutine( GetBaiDuToken());
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
#endif
    }
    public async Task<string> PostRequestToStringAsync(string postData, TextMeshProUGUI text)
    {
     
        return await PostRequestToStringAsync(GptUrl, postData,text);
    }

    public void WebglPostAi(string postData, TextMeshProUGUI text)
    {
        iscomplete = false;
        StartCoroutine(StreamData(GptUrl, postData, text));
    }

    public async Task<string> PostRequestToStringAsync(string url, string postData,TextMeshProUGUI text)
    {
        iscanspeak =false;
        try
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            request.ReadWriteTimeout = TIMEOUT_TIME;
            request.Timeout = TIMEOUT_TIME;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer pat_zyCzRFckOLRYsaLYe8cSNixQZgHcC8OJkThq0bHAjCGmv21Sr1fPQNQwcw2rGwsE");

            //流式发起请求
            using (Stream requestStream = await request.GetRequestStreamAsync())
            {
                Debug.Log("[GptStreamWrapper] PostRequestStreamToStringAsync postData : " + postData);
                byte[] dataBytes = Encoding.UTF8.GetBytes(postData);
                await requestStream.WriteAsync(dataBytes, 0, dataBytes.Length);
            }

            // 流式获取数据响应
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(responseStream))
            {  
                StringBuilder sb = new StringBuilder();
                char[] buffer = new char[1024];
                int bytesRead;
                StringBuilder sb1 = new StringBuilder();
                // 循环获取流式数据
                while ((bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    sb.Append(buffer, 0, bytesRead);
                    string a = sb.ToString();
                    var datas = a.Split("data:{");
                   
                    int i = 0;
                    sb1 = new StringBuilder();
                    foreach (var item in datas)
                    {
                        if (item != "")
                        {
                            
                            var item1 = "data:{" + item;
                            int openBraces = 0;
                            int closeBraces = 0;

                            foreach (char c in item1)
                            {
                                if (c == '{')
                                {
                                    openBraces++;
                                }
                                else if (c == '}')
                                {
                                    closeBraces++;
                                }
                            }
                            if (openBraces == closeBraces)
                            {
                                var data = JSON.Parse(item1);
                                if (!data["event"].Value.Equals("done") && i > 0)
                                {
                                    if (!data["is_finish"].AsBool)
                                    {
                                        sb1.Append(data["message"]["content"].Value);
                                    }
                                }
                                i++;
                            }
                           
                        }

                    }
                //    Debug.Log("Ai回答：" + sb1.ToString());
                    text.text = sb1.ToString();
                    buffer = new char[1024];
                    if (ispause)
                    {
                        iscanspeak = true;
                        audioSource.Stop();
                        StopAllCoroutines();
                        break;
                    }
                }
                if (!ispause)
                    StartCoroutine(TtsSequence(text.text));
                tip.SetActive(true);
                Debug.Log("完整数据: " + sb.ToString());
                return sb.ToString();
            }
        }
        catch (Exception e)
        {
            Debug.Log("错误信息: " + e.Message);
            iscanspeak = true;
            return "错误信息: " + e.Message;

        }

    }

    //改这个API
    IEnumerator StreamData(string uri, string postData, TextMeshProUGUI textComponent)
    {
        using (UnityWebRequest request = new UnityWebRequest(uri, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);
            //Debug.Log("令牌:" + "Bearer " + "pat_0g1bBeK3JnhCvgaA1xAQfoleLQYsxyOKBofxmmUT9Yyg2UDCoaXtTnSm6KXdnbmh");
            request.SetRequestHeader("Authorization", "Bearer "+ "pat_2SvjIPpuvTTOmr7QTrifSL5Td5dOM3mlafIJPWldf1s6PwKbXrH4xbKXjRa4DHbI");
            
           request.SetRequestHeader("Content-Type", "application/json");

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();//new CustomDownloadHandler(textComponent, new byte[1024 * 1024]);//

            request.timeout = TIMEOUT_TIME;

            yield return request.SendWebRequest();
            Debug.Log(request.downloadHandler.text);
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                textComponent.text = "Error: " + request.error;
            }
            else
            {
                Debug.Log("成功");
                iscomplete = true;
                //textComponent.text = "元音：声门 → 咽腔 → 软腭 → 硬腭 → 舌头 → 唇。\r\n辅音：声门 → 咽腔 → 软腭 → 硬腭或齿龈 → 牙齿 → 舌头 → 唇。";
                ProcessStreamResponse(request.downloadHandler.text, textComponent);
            }
        }
    }

    public void Close()
    {
        iscomplete = true;
        ispause = true;
        audioSource.Stop();
        StopAllCoroutines();
        aiUiView.StartCoroutine(aiUiView.idleie);
    }

    /// <summary>
    /// 合成语音协程
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public IEnumerator TtsSequence(string str)
    {
        var texts = SplitStringByByteLength(str, 120);
        audioclips.Clear();
        // 初始化足够的空位来存储音频片段
        for (int i = 0; i < texts.Count; i++)
        {
            audioclips.Add(null);
        }

        for (int i = 0; i < texts.Count; i++)
        {
            int index = i; // 当前文本的索引

            StartCoroutine(SendTtsRequest_API(texts[index], index));
        }

        // 等待所有音频片段加载完成
        while (audioclips.Contains(null))
        {
            yield return null;
        }
        Debug.Log("所有音频片段都已加载，现在可以按顺序播放" + audioclips.Count);
        // 所有音频片段都已加载，现在可以按顺序播放
        aiUiView.StopCoroutine(aiUiView.idleie);
       aiUiView.ai_animator.Play("说话");
        foreach (var clip in audioclips)
        {
            audioSource.clip = clip;
            audioSource.Play();
            Debug.Log("开始播放"+ audioSource.clip.name);
           yield return new WaitForSeconds(clip.length);
        }
        Debug.Log("说话完成");
        aiUiView.StartCoroutine(aiUiView.idleie);
        iscanspeak = true;
    }

    private IEnumerator SendTtsRequest_API(string text, int index)
    {
        Guid guid = Guid.NewGuid();
        string base64Guid = Convert.ToBase64String(guid.ToByteArray());
        base64Guid = base64Guid.Replace("=", "").Replace("+", "").Replace("/", "");
        var postdata = new BaiDuData
        {
            tex = text,
            tok = baidutoken,
            cuid = base64Guid,
        };
        WWWForm form = new WWWForm();
        form.AddField("tex", postdata.tex);
        form.AddField("tok", postdata.tok);
        form.AddField("cuid", postdata.cuid);
        form.AddField("ctp", postdata.ctp);
        form.AddField("lan", postdata.lan);
        form.AddField("spd", postdata.spd);
        form.AddField("pit", postdata.pit);
        form.AddField("vol", postdata.vol);
        form.AddField("per", postdata.per);
        form.AddField("aue", postdata.aue);
        Debug.Log("语音合成数据" + JsonUtility.ToJson(postdata));
        UnityWebRequest request = UnityWebRequest.Post(getAudio_URL, form);
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        request.SetRequestHeader("Accept", "*/*");

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            // 处理响应数据
           StartCoroutine(BytesToAudioClip(request.downloadHandler.data, index));
        }
        yield return null;
    }
    /// <summary>
    /// 切割长文本为短文本
    /// </summary>
    /// <param name="input">文本</param>
    /// <param name="maxByteLength">切割字符长度</param>
    /// <returns></returns>
    public List<string> SplitStringByByteLength(string input, int maxByteLength)
    {
        List<string> result = new List<string>();

        // 将输入字符串按照指定的字节长度进行拆分
        for (int i = 0; i < input.Length; i += maxByteLength)
        {
            int length = Mathf.Min(maxByteLength, input.Length - i);
            string substring = input.Substring(i, length);
            result.Add(substring);
        }
        Debug.Log("分割文本：");
        foreach (var item in result)
        {
            Debug.Log(item);
        }
        return result;
    }


    /// <summary>
    /// 将音频数据合成audioclip
    /// </summary>
    /// <param name="audioData"></param>
    /// <returns></returns>
    IEnumerator BytesToAudioClip(byte[] audioData, int index)
    {
        int sampleCount = audioData.Length / 2; // 每个样本是2个字节
        float[] floatData = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            short sample = BitConverter.ToInt16(audioData, i * 2);
            floatData[i] = sample / 32768.0f; // 将样本值转换为浮点数
        }

        // 创建 AudioClip，使用传入的采样率
        AudioClip audioClip = AudioClip.Create("aiaudio" + index, sampleCount, 1, 16000, false);
        audioClip.SetData(floatData, 0);

        audioclips[index] = audioClip;
        yield return null;
    }
    public string baidutoken;
   
    IEnumerator GetBaiDuToken()
    {
        yield return null;
        var postdata = new GetBaiDuTokenData
        {
            client_id = apiKey,
            client_secret = secretKey
        };

        string url = "https://docai.wehuatech.com/stage-api/coze/getBaiduToken";
        Debug.Log("测试Post:" + url);
        Http.PostJson(url, "")
     .OnSuccess(
         (HttpResponse response) =>
         {
             Debug.Log(JSON.Parse(response.Text));
             var data = JSON.Parse(response.Text);
             baidutoken = data["msg"].Value;
             Debug.Log("获取百度token:"+baidutoken);
         }

     )
     .OnError(
         (HttpResponse response) =>
         {
             Debug.Log("错误");
             Debug.Log(JSON.Parse(response.Text));
         }
     )
     .OnNetworkError(
         (HttpResponse response) =>
         {
             Debug.Log(JSON.Parse(response.Text));
         }
     )
     .Send();
    }

    void ProcessStreamResponse(string responseText, TextMeshProUGUI textComponent)
    {
        bool isnext = false;
        string[] lines = responseText.Split('\n');
        foreach (string line in lines)
        {
            //Debug.Log(line);
            if (line.Contains("event:conversation.message.completed"))
            {
                isnext = true;
                Debug.Log("有答案");
                continue;
                
            }
            if (isnext)
            {
                Debug.Log("下一个");
                if (line.StartsWith("data:"))
                {
                    Debug.Log("k可以 ");
                    string resultString = line.Replace("data:", "");
                    Debug.Log("可以 "+ line);
                    ResponseData messageEvent = JsonMapper.ToObject<ResponseData>(resultString); ;
                    if (messageEvent.type == "answer")
                    {
                        Debug.Log("正确答案Answer received: " + messageEvent.content);
                        textComponent.text = messageEvent.content;
                        // This is the final answer from the bot
                        break;
                    }
                }
               
                isnext = false;
            }
        }
    }

}
[Serializable]
public class MessageEvent
{
   // public string event;
        public MessageEventData data;
}

// 定义 JSON 数据模型
public class ResponseData
{
    public string id;
    public string conversation_id;
    public string bot_id;
    public string role;
    public string type;
    public string content;
    public string content_type;
    public string chat_id;
}
[Serializable]
public class MessageEventData
{
    public string id;
    public string conversation_id;
    public string bot_id;
    public string role;
    public string type;
    public string content;
    public string content_type;
    public string chat_id;
}

public class CustomDownloadHandler : DownloadHandlerScript
{
    public CustomDownloadHandler(TextMeshProUGUI textComponent,byte[] buffer) : base(buffer) { this.textComponent = textComponent; }
    private TextMeshProUGUI textComponent;
    private StringBuilder sb = new StringBuilder();
    private StringBuilder sb1 = new StringBuilder();
    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        if (GptStreamWrapper.instance.ispause)
        {
             GptStreamWrapper.iscanspeak = true;
            GptStreamWrapper.instance.audioSource.Stop();
            GptStreamWrapper.instance.StopAllCoroutines();
            return false;
        }
        if (data == null || data.Length < 1)
        {
            return false;
        }
        string receivedData = System.Text.Encoding.UTF8.GetString(data, 0, dataLength);
        sb.Append(receivedData);
        string a = sb.ToString();
        var datas = a.Split(new string[] { "data:{" }, System.StringSplitOptions.None);

        int i = 0;
        sb1.Clear();
        foreach (var item in datas)
        {
            if (!string.IsNullOrEmpty(item))
            {
                var item1 = "data:{" + item;
                int openBraces = 0;
                int closeBraces = 0;

                foreach (char c in item1)
                {
                    if (c == '{')
                    {
                        openBraces++;
                    }
                    else if (c == '}')
                    {
                        closeBraces++;
                    }
                }

                if (openBraces == closeBraces)
                {
                    var dataJson = JSON.Parse(item1);
                    if (!dataJson["event"].Value.Equals("done") && i > 0)
                    {
                        if (!dataJson["is_finish"].AsBool)
                        {
                            sb1.Append(dataJson["message"]["content"].Value);
                        }
                    }
                    i++;
                }
            }
        }
     //   textComponent.text = sb1.ToString();
        Debug.Log("流式数据响应"+ sb1.ToString());
     
        return true;
    }

   
    protected override void CompleteContent()
    {

        //#if !UNITY_EDITOR && UNITY_WEBGL
        //textComponent.text = "";
        //textComponent.DOText(sb1.ToString(), 1f);
        textComponent.text = sb1.ToString();
        //#endif
        if (!GptStreamWrapper.instance.ispause)
        {
           
            GptStreamWrapper.instance.StartCoroutine(GptStreamWrapper.instance.TtsSequence(sb1.ToString()));
        }
          
        GptStreamWrapper.instance.tip.SetActive(true);
        Debug.Log("完整数据: " + sb.ToString());
        Debug.Log("Download complete.");
      
        //#if !UNITY_EDITOR && UNITY_WEBGL
        //        string str = textComponent.text;
        //        textComponent.text = "";

      
        GptStreamWrapper.instance.iscomplete = true;
    }

    protected override float GetProgress()
    {
        return base.GetProgress();
    }

  
}

public class BaiDuData
{
    public string tex;
    public string tok = "";
    public string cuid = "";
    public string ctp = "1";
    public string lan = "zh";
    public int spd = 5;
    public int pit = 7;
    public int vol = 7;
    public int per = 110;
    public int aue = 4;
}

public class GetBaiDuTokenData
{
  
    public string client_id ;
    public string client_secret ;
    public string grant_type = "client_credentials";
}

