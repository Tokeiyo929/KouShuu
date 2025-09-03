using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.IO;
using System;

namespace QFramework.Example
{
    public class Global : Architecture<Global>
    {
        // 当前语言
        public static BindableProperty<GlobalEnums.Language> CurrentLanguage = new BindableProperty<GlobalEnums.Language>(GlobalEnums.Language.Chinese);
        //当前选择的音频类型
        public static BindableProperty<GlobalEnums.AudioType> CurrentAudioType = new BindableProperty<GlobalEnums.AudioType>(GlobalEnums.AudioType.Chinese);
        //当前选择的关键词文本
        public static BindableProperty<string> CurrentKeyword = new BindableProperty<string>("");
        //当前场景
        public static BindableProperty<string> CurrentScene = new BindableProperty<string>("");
        //当前选择的步骤
        public static BindableProperty<int> CurrentStep = new BindableProperty<int>(0);

        public static List<float> ScoreList=new List<float>();

        public static string StudentClass="1班";
        public static string StudentName="张三";
        public static string StudentId="1234567890";
        public static int StudentScore;

        public static string StudentEndTime;
        public static DateTime StudentStartTime;
        public static float StudentEndtime;

        protected override void Init()
        {
            for(int i=0;i<18;i++)
            {
                ScoreList.Add(0f);
            }
        }
        

    }
}
