using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FSM
{
    [System.Serializable]
    public class State : MonoBehaviour
    {
        public List<Action> inActions = new List<Action>();
        public List<Action> outActions = new List<Action>();
        public List<State> nextStates = new List<State>();
        [HideInInspector] public Vector2 position;
        public DecisionMaker decisionMaker;
        public string guid;
       

        protected virtual void Awake()
        {

        }

        public virtual void Enter()
        {
            foreach (var action in inActions)
            {
                action.Execute();
            }
        }

        public virtual void Exit()
        {
            foreach (var action in outActions)
            {
                action.Execute();
            }
            if (this.name == "State-入座")
            {
                Debug.Log("进入入座状态，显示下一页按钮");
                // 这里调用Btn_NextPage.Show()，假设Btn_NextPage是一个全局可以访问的对象
                CanvasGroup nextPageCanvasGroup = GameObject.Find("Btn_NextPage")?.GetComponent<CanvasGroup>();
                nextPageCanvasGroup.alpha = 1;
                nextPageCanvasGroup.interactable = true;
                nextPageCanvasGroup.blocksRaycasts = true;
            }
        }

        public virtual bool Transfer(State newState)
        {
            if(decisionMaker!=null){
                return decisionMaker.Decide(newState);
            }
            else{
                return true;
            }
        }

        public void ExecuteAction(Action action)
        {
            // if(.currentSate==this){
            action.Execute();
            // }
            // else{
            //     Debug.LogWarning("当前状态无法执行该Action");
            // }
        }
    }
}