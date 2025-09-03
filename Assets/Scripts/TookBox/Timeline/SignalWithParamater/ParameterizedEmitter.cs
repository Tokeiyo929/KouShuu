using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class ParameterizedEmitter<T>: SignalEmitter{
    public T parameter;
}