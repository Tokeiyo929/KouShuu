using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightPlus;
using UnityEngine.UI;
using QFramework.Example;

public class HightlightManage : MonoBehaviour
{
    public static HightlightManage instance;
    public GameObject[] choices;
    private HighlightEffect[] effects;

    public GameObject rightAnswer;

    [SerializeField]private Button submitButton;
    [SerializeField]private GameObject John;
    [SerializeField]private Image img_correct;
    [SerializeField]private Image img_wrong;

    private GameObject JohnClothing_DailyTShirts;
    private GameObject JohnClothing_DailyPants;
    private GameObject JohnClothing_CasualSuit;
    private GameObject JohnClothing_LeatherSuit;
    private GameObject JohnClothing_Suit;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
        }

        effects = new HighlightEffect[choices.Length];
        for(int i = 0; i < choices.Length; i++)
        {
            effects[i] = choices[i].GetComponent<HighlightEffect>();
        }

        JohnClothing_DailyTShirts = John.transform.GetChild(1).gameObject;
        JohnClothing_DailyPants = John.transform.GetChild(2).gameObject;
        JohnClothing_CasualSuit = John.transform.GetChild(0).gameObject;
        JohnClothing_LeatherSuit = John.transform.GetChild(3).gameObject;
        JohnClothing_Suit = John.transform.GetChild(12).gameObject;

        submitButton.onClick.AddListener(OnClickSubmitButton);
    }
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < effects.Length; i++)
        {
            effects[i].Refresh();
            effects[i].SetHighlighted(true);
        }

        JohnClothing_DailyTShirts.SetActive(true);
        JohnClothing_DailyPants.SetActive(true);
        JohnClothing_CasualSuit.SetActive(false);
        JohnClothing_LeatherSuit.SetActive(false);
        JohnClothing_Suit.SetActive(false);
    }

    public void submitAnswer()
    {
        JohnClothing_DailyPants.SetActive(false);
        JohnClothing_DailyTShirts.SetActive(false);
        JohnClothing_CasualSuit.SetActive(false);
        JohnClothing_LeatherSuit.SetActive(false);
        JohnClothing_Suit.SetActive(true);

        for(int i = 0; i < effects.Length; i++)
        {
            if(effects[i].highlighted)
            {
                if(effects[i].gameObject.name == rightAnswer.name)//选择正确
                {
                    //effects[i].SetHighlighted(true);
                    choices[i].GetComponent<HighlightControl>().SetGlowColor(Color.green);
                    img_correct.gameObject.SetActive(true);
                    img_wrong.gameObject.SetActive(false);

                    Global.ScoreList[1] = 4;
                }
                else//选择错误
                {
                    //effects[i].SetHighlighted(true);
                    choices[i].GetComponent<HighlightControl>().SetGlowColor(Color.red);
                    rightAnswer.GetComponent<HighlightControl>().SetGlowColor(Color.green);
                    rightAnswer.GetComponent<HighlightEffect>().SetHighlighted(true);
                    img_wrong.gameObject.SetActive(true);
                    img_correct.gameObject.SetActive(false);

                    Global.ScoreList[1] = 0;
                }
                return;
            }
        }
    }

    public void ChooseClothing(HighlightEffect curChosed,GameObject clothing)
    {
        JohnClothing_DailyPants.SetActive(false);
        JohnClothing_DailyTShirts.SetActive(false);
        JohnClothing_CasualSuit.SetActive(false);
        JohnClothing_LeatherSuit.SetActive(false);
        JohnClothing_Suit.SetActive(false);

        clothing.SetActive(true);

        for(int i = 0; i < effects.Length; i++)
        {
            if(effects[i].gameObject.name == curChosed.gameObject.name)
            {
                effects[i].SetHighlighted(true);
            }
            else
            {
                effects[i].SetHighlighted(false);
            }
        }
    }

    private void OnClickSubmitButton()
    {
        submitAnswer();
    }
}
