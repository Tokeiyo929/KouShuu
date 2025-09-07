using UnityEngine;

namespace QFramework.Example
{
    [MonoSingletonPath("AnimationManager")]
    public class AnimationManager : MonoBehaviour,ISingleton
    {
        
        [SerializeField] private GameObject TimeLine;

        [SerializeField] private GameObject John;
        [SerializeField] private GameObject MoLi;
        [SerializeField] private GameObject WangGuoXin;
        [SerializeField] private GameObject LiWenJun;
        [SerializeField] private GameObject LiTianRan;

        void ISingleton.OnSingletonInit()
        {
            if(TimeLine==null)
            {
                TimeLine=GameObject.Find("TimeLine");
            }
        }
        //单例
        private static AnimationManager instance;
        public static AnimationManager Instance
        {
            get
            {
                if (!instance)
                {
                    var uiRoot = UIRoot.Instance;
                    Debug.Log("currentUIRoot:" + uiRoot);
                    instance = MonoSingletonProperty<AnimationManager>.Instance;
                }
                return instance;
            }
        }

        public void PauseTimeline(float time)
        {
            Debug.Log("Pause Timeline");
			TimeLine.GetComponent<TimelineController>().PlayTimelineAtTime(time);
			TimeLine.GetComponent<TimelineController>().PauseTimeline();
        }

        public void ActivatePerson(string name)
        {
            switch(name)
            {
                case "John":
                    John.SetActive(true);
                    John.GetComponent<Animator>().enabled=true;
                    John.GetComponent<Animator>().Play("Default", 0, 0);
                    break;
                case "MoLi":
                    MoLi.SetActive(true);
                    MoLi.GetComponent<Animator>().enabled=true;
                    MoLi.GetComponent<Animator>().Play("Default", 0, 0);
                    break;
                case "WangGuoXin":
                    WangGuoXin.SetActive(true);
                    WangGuoXin.GetComponent<Animator>().enabled=true;
                    WangGuoXin.GetComponent<Animator>().Play("Default", 0, 0);
                    break;
                case "LiWenJun":
                    LiWenJun.SetActive(true);
                    LiWenJun.GetComponent<Animator>().enabled=true;
                    LiWenJun.GetComponent<Animator>().Play("Default", 0, 0);
                    break;
                case "LiTianRan":
                    LiTianRan.SetActive(true);
                    LiTianRan.GetComponent<Animator>().enabled=true;
                    break;
            }
        }

        public void DeactivatePerson(string name)
        {
            switch(name)
            {
                case "John":
                    John.SetActive(false);
                    break;
                case "MoLi":
                    MoLi.SetActive(false);
                    break;
                case "WangGuoXin":
                    WangGuoXin.SetActive(false);
                    break;
                case "LiWenJun":
                    LiWenJun.SetActive(false);
                    break;
                case "LiTianRan":
                    LiTianRan.SetActive(false);
                    break;
            }
        }

        public void StartInspection()
        {
            //对王国信的所有TeaSet标签子物体，启用TeaSetInteraction组件
            foreach(var teaSet in WangGuoXin.GetComponentsInChildren<TeaSetInteraction>(true))
            {
                Debug.Log("启用TeaSetInteraction组件:"+teaSet.name);
                teaSet.enabled=true;
                //同时启用BoxCollider组件
                teaSet.gameObject.GetComponent<BoxCollider>().enabled=true;
            }
        }

        public void EndInspection()
        {
            //对王国信的所有TeaSet标签子物体，禁用TeaSetInteraction组件
            foreach(var teaSet in WangGuoXin.GetComponentsInChildren<TeaSetInteraction>(true))
            {
                Debug.Log("禁用TeaSetInteraction组件:"+teaSet.name);
                teaSet.enabled=false;
                //同时禁用BoxCollider组件
                teaSet.gameObject.GetComponent<BoxCollider>().enabled=false;
            }
        }

    }
}