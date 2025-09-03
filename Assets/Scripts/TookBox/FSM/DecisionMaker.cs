using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public class DecisionMaker : MonoBehaviour
    {
        private Machine machine;
        public bool Decide(State newState)
        {

            if(machine==null){
                machine = GetComponentInParent<Machine>();
            }

            if (machine.currentSate == newState)
            {
                return false;
            }
            else
            {
                if (machine.currentSate.nextStates!=null && machine.currentSate.nextStates.Count>0)
                {
                    if (machine.currentSate.nextStates.IndexOf(newState) > -1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }
        

        public DecisionMaker Inject(Machine machine){
            this.machine = machine;
            return this;
        }

    }

}

