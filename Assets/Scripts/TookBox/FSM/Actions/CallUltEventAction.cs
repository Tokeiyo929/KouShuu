using UnityEngine;
using UltEvents;

namespace FSM
{
    public class CallUltEventAction : Action
    {
        [SerializeField]
        private UltEvent _Event;
        public override void Execute()
        {
            base.Execute();
            _Event?.Invoke();
            nextAction?.Execute();
        }
    }
}