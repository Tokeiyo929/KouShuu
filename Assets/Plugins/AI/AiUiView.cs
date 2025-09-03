using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class AiUiView : MonoBehaviour
{
    public Animator ai_animator;
    public Animator ai_animator1;
    //public Animator ai_animator2;
    //public Animator ai_animator3;
    public Image botbg;
    public GameObject tip1, tip2;

    [Header("�����س����¼�")]
    public TMP_InputField inputField;
    public Button targetButton;
    //public GameObject tip3, tip4;
    private void Awake()
    {
        inputField.onEndEdit.AddListener(HandleSubmit);


    }

    void HandleSubmit(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            targetButton.onClick.Invoke(); // ������ť�ĵ���¼�
            inputField.ActivateInputField(); // ���¼�������򣬷�ֹʧȥ����
        }
    }

    private void OnEnable()
    {
        OnShow();
    }
    public void OnShow()
    {
        //ai_animator.Play("����");
        //ai_animator1.Play("����");
        //ai_animator2.Play("����");
        //ai_animator3.Play("����");
        idleie = IdleIE();
        StartCoroutine(idleie);
    }
    private void OnDisable()
    {
        botbg.color = new Color(botbg.color.r, botbg.color.g, botbg.color.b, 0);
        StopCoroutine(idleie);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StopIdle()
    {
        StopCoroutine(idleie);
        tip1.SetActive(false);
        tip2.SetActive(false);
        //tip3.SetActive(false);
        //tip4.SetActive(false);
    }
    public IEnumerator idleie;
    IEnumerator IdleIE()
    {
        botbg.DOFade(0.3f, 1f);
        yield return new WaitForSeconds(0.8f);
        while (true)
        {
            int a = Random.Range(0, 3);
            switch (a)
            {
                case 0:
                    tip1.SetActive(false);
                    tip2.SetActive(false);
                    //tip3.SetActive(false);
                    //tip4.SetActive(false);
                    yield return new WaitForSeconds(1.5f);
                    break;
                case 1: 
                    //ai_animator.Play("�ȴ�");
                    //ai_animator1.Play("�ȴ�");
                    //ai_animator2.Play("����");
                    //ai_animator3.Play("����");
                    if (!MicPhoneUi.isshow)
                    {
                        tip1.SetActive(true);
                        tip2.SetActive(false);
                        //tip3.SetActive(true);
                        //tip4.SetActive(false);
                        yield return new WaitForSeconds(1f);
                    }
                    yield return new WaitForSeconds(2.40f);
                    break;
                case 2:    
                    //ai_animator.Play("˼��");
                    //ai_animator1.Play("˼��");
                    //ai_animator2.Play("����");
                    //ai_animator3.Play("����");
                    if (!MicPhoneUi.isshow)
                    {
                        tip1.SetActive(false);
                        tip2.SetActive(true);
                        //tip4.SetActive(true);
                        //tip3.SetActive(false);
                        yield return new WaitForSeconds(1f);
                    }
                    yield return new WaitForSeconds(1.15f);
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(1f);
        }
      
    }
}
