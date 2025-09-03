using System;
using System.Collections;
using UnityEngine;
using UltEvents;

namespace FSM
{
    public class YieldUltEventAction: Action
    {
        [SerializeField]
        private UltEvent _Event;
        
        IEnumerator Run(IEnumerator rator){
            yield return rator;
            nextAction?.Execute();
        }

        public override void Execute()
        {
            base.Execute();
            _Event.Invoke();
        }
        public void ReciveRator(IEnumerator rator){
            StartCoroutine(Run(rator));
        }

    }
}