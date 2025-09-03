using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightPlus;


public class HighlightControl : MonoBehaviour
{
    private HighlightEffect effect;
    
    public GameObject rightAnswer;

    [SerializeField]private GameObject clothing;

    void Awake()
    {
        effect = this.GetComponent<HighlightEffect>();
    }

    // Start is called before the first frame update
    void Start()
    {
        effect.Refresh();
        effect.SetHighlighted(true);
    }
    
    void OnMouseDown()
    {
        //clothing.SetActive(true);
        HightlightManage.instance.ChooseClothing(effect,clothing);
    }
    
    public void SetGlowColor(Color color)
    {
        if (effect.glowPasses != null && effect.glowPasses.Length > 0)
        {
            // 遍历所有glowPasses，设置颜色
            for (int i = 0; i < effect.glowPasses.Length; i++)
            {
                effect.glowPasses[i].color = color;
            }
            
            // 更新材质属性以应用更改
            effect.UpdateMaterialProperties(true);
        }
    }
}
