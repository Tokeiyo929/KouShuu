using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangToggleImage : MonoBehaviour
{
   public Sprite SelectImg;
   public Sprite UnSelectImg;

    void Awake()
    {
        GetComponent<Toggle>().onValueChanged.AddListener((isOn)=>
        {
            if(isOn)
            {
                GetComponent<Image>().sprite=SelectImg;
            }
            else{
                GetComponent<Image>().sprite=UnSelectImg;
            }
        });
    }
}
