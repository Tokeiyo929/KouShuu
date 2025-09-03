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