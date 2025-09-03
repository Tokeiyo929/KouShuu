using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Events;
using System;
using System.Linq;

public class SignalReceiverWithBool : MonoBehaviour, INotificationReceiver
{
    public SignalAssetEventPair[] signalAssetEventPairs;
    [Serializable]
    public class SignalAssetEventPair
    {
        public SignalAsset signalAsset;
        public ParameterizedEvent events;

        [Serializable]
        public class ParameterizedEvent : UnityEvent<bool> { }
    }
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (notification is ParameterizedEmitter<bool> boolEmitter)
        {
            var matches = signalAssetEventPairs.Where(x => ReferenceEquals(x.signalAsset, boolEmitter.asset));
            foreach (var m in matches)
            {
                m.events.Invoke(boolEmitter.parameter);
            }
        }
    }
}
