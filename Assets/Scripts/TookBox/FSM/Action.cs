using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{


    public class Action : MonoBehaviour
    {
        public Action nextAction;

        public string guid;


        [HideInInspector] public Vector2 position;


        public virtual void Execute()
        {

            Debug.Log("调试: 执行Action: " + this.name + " 在State: " + GetComponentInParent<Machine>().currentSate.name);
        }
    }
}