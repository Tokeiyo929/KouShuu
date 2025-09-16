using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PanelElementInitial : MonoBehaviour
{
    public UnityEvent onInit;

    public UnityEvent onDisableEvents;

    void OnEnable()
    {
        onInit?.Invoke();
    }
    public void TriggerCloseCanvas()
    {
        onDisableEvents?.Invoke();
    }
    private void OnDisable()
    {
        TriggerCloseCanvas();
    }
}
