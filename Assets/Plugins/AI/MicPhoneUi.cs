using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class MicPhoneUi : MonoBehaviour
{
   // public InputActionReference menu;
   // public InputActionReference menu,menu1;
    public GameObject ui;
    public static MicPhoneUi instance; 
    public static bool isshow;
    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        //if (menu != null)
        //{
        //    menu.action.Enable();
        //    menu.action.performed += ToggleMenu2;
        //}
        //if (menu1 != null)
        //{
        //    menu1.action.Enable();
        //    menu1.action.performed += ToggleMenu2;
        //}
    }

    

    //private void ToggleMenu(InputAction.CallbackContext context)
    //{
    //    ui.SetActive(!ui.active);
    //   // debugtext.text += "按下菜单按钮1，当前页面状态" + ui.active;
    //}

    //private void ToggleMenu2(InputAction.CallbackContext context)
    //{
    //    if (currentie != null)
    //        StopCoroutine(currentie);
    //    currentie = ToggleMenuIE(!ui.active);
    //    StartCoroutine(currentie);
    //}


    public void ToggleMenu()
    {
        if (currentie != null)
            StopCoroutine(currentie);
        currentie = ToggleMenuIE(!ui.active);
        StartCoroutine(currentie);
    }
    IEnumerator currentie;
    IEnumerator ToggleMenuIE(bool ison)
    {
        if (ison)
        {
            ui.SetActive(true);
            ui.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            yield return new WaitForSeconds(0.5f);
            isshow = true;
       }
        else
        {
            ui.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
            yield return new WaitForSeconds(0.5f);
            ui.SetActive(false);
            isshow = false;
        }
        yield return null;
    }

    public void StopAI()
    {
        Debug.Log("停止");
        GptStreamWrapper.instance.Close();
       GptStreamWrapper.iscanspeak = true;
    }
}

