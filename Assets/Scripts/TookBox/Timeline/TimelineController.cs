using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


public class TimelineController : MonoBehaviour
{
    public PlayableDirector director;
    private int currentQuestId;  // 当前步骤 ID
    public bool isplaying;

    void Awake()
    {

        director = GetComponent<PlayableDirector>();
    }

    public void PauseTimeline()
    {
        director.Pause();
        isplaying = false;
        Debug.Log("TimelineController PauseTimeline");
    }

    public void ResumeTimeline()
    {
        //TODO 检查speed，如果为0，则rebuild
        var speed = director.playableGraph.GetRootPlayable(0).GetSpeed();
        if(speed == 0){
            director.RebuildGraph();  // 丢弃现有 PlayableGraph 并创建一个新实例
            director.playableGraph.GetRootPlayable(0).SetSpeed(1); // 返回给定索引处无任何输出连接的 Playable
            director.Play();
        }
        else{
            director.Resume(); //恢复播放暂停的可播放项
            isplaying = true;
        }
    }

    /// <summary>
    /// 窗口回调后播放特定帧的动画
    /// </summary>
    /// <param name="frameNum">要播放的帧数</param>
    public void PlayTimelineAtFrame(int frameNum)  // 播放某帧所在的时间点的动画
    {
        director.time = frameNum / ((TimelineAsset)director.playableAsset).editorSettings.frameRate;
        director.RebuildGraph();
        director.Play();
    }

    /// <summary>
    /// 播放特定时间的动画
    /// </summary>
    /// <param name="time"></param>
    public void PlayTimelineAtTime(float time)
    {
        director.time = time;
        director.RebuildGraph();
        director.Play();
        isplaying = true;
    }

    /// <summary>
    /// 移动Timeline到指定时间
    /// </summary>
    /// <param name="time"></param>
    public void MoveTimelineAtTime(float time)
    {
        director.time = time;
        director.RebuildGraph();
        director.Play();
        director.playableGraph.GetRootPlayable(0).SetSpeed(0);
        isplaying = false;
    }

     public void PauseTimeline(float time)
    {
        Debug.Log("Pause Timeline");
		PlayTimelineAtTime(time);
		PauseTimeline();
    }
}