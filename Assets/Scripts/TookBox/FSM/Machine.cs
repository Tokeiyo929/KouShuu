using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;

namespace FSM
{
    //有限状态机
    //状态机有多个DecisionMaker，用来确定转换的逻辑
    public class Machine : MonoBehaviour
    {
        [HideInInspector] public State currentSate;
        public List<State> states = new List<State>();
        public State firstState;
        public List<DecisionMaker> decisionMakers = new List<DecisionMaker>();
        
        [Header("状态跳转控制")]
        [SerializeField] private bool allowAutoPlay = true; // 是否允许自动播放
        protected virtual void Awake()
        {
            var _states = GetComponentsInChildren<State>();
            foreach (var state in _states)
            {
                if (state is FirstState) 
                {
                    firstState = state;
                }
                states.Add(state);
            }
        }

        private void Start() {
            //TODO 是否需要挪到GameManager点击开始按钮后再开始
            Run();
            StopAllCoroutines();
        }

        public virtual void ChangeState(State newState)
        {
            if (currentSate != null)
            {
                if (currentSate.Transfer(newState))
                {
                    currentSate.Exit();
                    currentSate = newState;
                    currentSate.Enter();
                }
                else
                {
                    Debug.LogWarning("转换状态失败:" + newState.name);
                }
            }
            else
            {
                currentSate = newState;
                currentSate.Enter();
            }
        }

        public virtual void ChangeToStateByName(string name)
        {
            // 处理空字符串或null的情况
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogWarning("尝试切换到空状态名称，已忽略");
                return;
            }

            var state = FindState(name);
            if (state == null)
            {
                Debug.LogWarning($"未找到状态: {name}");
                return;
            }
            
            ChangeState(state);
            Debug.Log("切换至状态" + name);
        }

        public void Run(){
            currentSate = firstState;
            currentSate?.Enter();
            Debug.Log("machineStart");
        }

        public State FindState(string name)
        {
            var result = states.Find(x => x.name == name);
            return result;
        }
        


        #region Editor Compatibility

#if UNITY_EDITOR

        public State CreateState(System.Type type)
        {
            Debug.Log("type" + type);
            var obj = new GameObject("State");
            obj.AddComponent(type);
            obj.transform.SetParent(this.transform,false);
            var cmp = obj.GetComponent<State>();
            cmp.guid = GUID.Generate().ToString();
            return cmp;
        }

        public void DeleteState(State state)
        {
            // Undo.RecordObject(this, "FSM (DeleteState)");
            states.Remove(state);
            // Undo.DestroyObjectImmediate(state);
        }

        public void RemoveChild(State parent, State child)
        {
            // Undo.RecordObject(parent, "FSM (RemoveChild)");
            parent.nextStates.Remove(child);
            // EditorUtility.SetDirty(parent);
        }

        public void RemoveChild(FSM.Action parent,FSM.Action child)
        {
            parent.nextAction = null;
        }

        public void RemoveChild(State parent, FSM.Action child)
        {
            // Undo.RecordObject(parent, "FSM (RemoveChild)");
            parent.inActions.Remove(child);
            parent.outActions.Remove(child);
            // EditorUtility.SetDirty(parent);
        }

        public void AddChild(State parent, State child)
        {
            // Undo.RecordObject(parent, "FSM (AddChild)");
            parent.nextStates.Add(child);
            // EditorUtility.SetDirty(parent);
        }

        public void AddChild(List<Action> parent, FSM.Action child)
        {
            // Undo.RecordObject(parent, "FSM (AddChild)");
            parent.Add(child);
            // EditorUtility.SetDirty(parent);
        }

        public void AddChild(FSM.Action parent, FSM.Action child)
        {
            // Undo.RecordObject(parent, "FSM (AddChild)");
            parent.nextAction = child;
            // EditorUtility.SetDirty(parent);
        }

        public Action CreateAction(System.Type type){
            var obj = new GameObject("Action");
            obj.AddComponent(type);
            obj.transform.SetParent(this.transform,false);
            var cmp = obj.GetComponent<Action>();
            cmp.guid = GUID.Generate().ToString();
            return cmp;
        }

        public void RefreshStates(){
            states.Clear();
            var _states = GetComponentsInChildren<State>();
            foreach (var state in _states)
            {
                if (state is FirstState)
                {
                    firstState = state;
                }
                states.Add(state);
            }

            decisionMakers.Clear();
            var _decisionMakers = GetComponentsInChildren<DecisionMaker>();
            foreach (var decisionMaker in decisionMakers)
            {
                decisionMakers.Add(decisionMaker);
            }
        }

        public void RefreshStatesPos(){

            //TODO 重排UI
            Debug.Log("重排UI");
            int oid = 0;//用来分页的id 
            foreach (var state in states)
            {
                int row = oid%10;
                int col = oid/10;
                oid+=1;
                Debug.Log("格子:"+row+":"+col);
                Vector2 newPos = new Vector2(row*300,col*300);
                state.position = newPos;
                state.guid = GUID.Generate().ToString();
                ResetActionPos(state,newPos);
            }
        }

        public void ResetActionPos(State state,Vector2 np){
            
            Vector2 actionPos = new Vector2(np.x+150,np.y+50);
            int aid = 0;//用这个id来控制竖排间距
            foreach(var action in state.inActions){
                Vector2 newPos = new Vector2(actionPos.x,actionPos.y+aid*50);
                aid+=1;
                action.position = newPos;
                action.guid = GUID.Generate().ToString();
            }

            foreach(var action in state.outActions){
                Vector2 newPos = new Vector2(actionPos.x,actionPos.y+aid*50);
                aid+=1;
                action.position = newPos;
                action.guid = GUID.Generate().ToString();
            }
        }

#endif
        #endregion Editor Compatibility
    }
}
