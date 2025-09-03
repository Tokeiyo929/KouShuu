using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
//using UnityEditor.Experimental.GraphView;

namespace QFramework.Example
{
    [RequireComponent(typeof(Button))]
    public class LocalizedAudioButton : MonoBehaviour
    {
        [Header("汉语/粤语/英语")]public bool ChineseOrCantoneseOrEnglish = false;
        [Header("汉语/粤语")]public bool ChineseOrCantonese = false;
        [Header("汉语/英语")]public bool ChineseOrEnglish = false;
        [Header("纯英语")]public bool IsEnglish = false;

        public string chineseAudioId;
        public string cantoneseAudioId;
        public string englishAudioId;

        private Button Btn_Audio;

        private void Start()
        {
            Btn_Audio = GetComponent<Button>();
            Btn_Audio.onClick.AddListener(OnClick);

        }

        

        private void OnClick()
        {
            string audioId = GetAudioIdToPlay();
            if (!string.IsNullOrEmpty(audioId))
            {
                AudioManager.Instance.PlayAudio(audioId);
                Debug.Log($"播放音频ID: {audioId}");
            }
            else
            {
                Debug.LogError("无法确定要播放的音频ID！");
            }
        }
        
        private string GetAudioIdToPlay()
        {
            // 支持中文、粤语、英文三种语言
            if (ChineseOrCantoneseOrEnglish){ return GetAudioIdForThreeLanguages(); }
            
            // 支持中文、粤语两种语言
            if (ChineseOrCantonese){ return GetAudioIdForChineseAndCantonese(); }
            
            // 支持中文、英文两种语言
            if (ChineseOrEnglish){ return GetAudioIdForChineseAndEnglish(); }
            
            // 仅支持英文
            if (IsEnglish){ return englishAudioId; }
            
            // 没有匹配的语言配置
            return null;
        }
        
        /// <summary>
        /// 处理三语言模式：中文、粤语、英文
        /// </summary>
        private string GetAudioIdForThreeLanguages()
        {
            if (Global.CurrentLanguage.Value == GlobalEnums.Language.English)
                return englishAudioId;

            return GetAudioIdForChineseAndCantonese();// 否则在中文和粤语之间选择
        }
        
        /// <summary>
        /// 处理双语言模式：中文、粤语
        /// </summary>
        private string GetAudioIdForChineseAndCantonese()
        {
            if (Global.CurrentAudioType.Value == GlobalEnums.AudioType.Chinese)
                return chineseAudioId;
            else
                return cantoneseAudioId;
            
        }
        
        /// <summary>
        /// 处理双语言模式：中文、英文
        /// </summary>
        private string GetAudioIdForChineseAndEnglish()
        {
            if (Global.CurrentLanguage.Value == GlobalEnums.Language.Chinese)
                return chineseAudioId;
            else
                return englishAudioId;
            
        }


    }
}
