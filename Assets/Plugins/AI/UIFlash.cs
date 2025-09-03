using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class UIFlash : MonoBehaviour
{
    // Start is called before the first frame update
    public  Image image;
    void Start()
    {
        FlshIE = Flsh();
        StartCoroutine(FlshIE);
    }
    private void OnDisable()
    {
        image.color= new Color32(255, 255, 255, 255);
        StopCoroutine(FlshIE);
    }
   

    IEnumerator FlshIE;
    IEnumerator Flsh()
    {
        while (true)
        {
            image.DOColor(new Color32(152, 152, 152, 255), 0.3f);
            yield return new WaitForSeconds(0.3f);
            image.DOColor(new Color32(255, 255, 255, 255), 0.3f);
            yield return new WaitForSeconds(0.3f);

        }
        
    }
}
