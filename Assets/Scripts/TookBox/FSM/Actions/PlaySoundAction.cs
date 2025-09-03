using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public class PlaySoundAction : Action
    {
        public AudioClip clip;

        [HideInInspector] public AudioSource source;


        private void Awake()
        {
            source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
        }

        public override void Execute()
        {
            base.Execute();
            
            var allPlaySoundActions = FindObjectsOfType<PlaySoundAction>();
            foreach (var action in allPlaySoundActions)
            {
                action.StopSound();
            }

            //source.volume = DataManager.instance.settingData.soundVolume;

            source.Play();
            float clipTime = clip.length;
            if (nextAction != null)
            {
                this.StartCoroutine(ExecuteNextAction(clipTime));
            }
        }

        public void StopSound(){
            source.Stop();
        }

        private IEnumerator ExecuteNextAction(float waitTime)
        {
            Debug.Log("声音播放开始，开始执行下一Action");
            yield return new WaitForSeconds(waitTime);
            Debug.Log("声音播放完毕，开始执行下一Action" + nextAction);

            nextAction.Execute();
        }
    }

}