using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PanelElementInitial : MonoBehaviour
{
    public UnityEvent onInit;

    void OnEnable()
    {
        onInit?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
