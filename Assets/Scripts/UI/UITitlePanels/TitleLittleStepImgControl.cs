using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TitleLittleStepImgControl : MonoBehaviour
{
    public Sprite UnHightLightSelectImg;
    public Sprite HightLightImg;

    private Image _image;
    void Awake()
    {
        _image=GetComponent<Image>();
    }

    public void SetHightLight(bool isHightLight)
    {
        _image.sprite=isHightLight?HightLightImg:UnHightLightSelectImg;
    }
}
