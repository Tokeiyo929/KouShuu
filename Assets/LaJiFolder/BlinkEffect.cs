using UnityEngine;
using UnityEngine.UI; // ��Ҫ����UI�����ռ�������Image
using DG.Tweening; // ����DOTween�����ռ�

public class BlinkEffect : MonoBehaviour
{
    [Header("References")]
    public Image blinkImage; // ��ק��ֵ�����������ǵ�BlinkOverlay Image

    [Header("Settings")]
    public float blinkDuration = 0.2f; // ���ν��ڻ�����ʱ��

    private void OnEnable()
    {
        DoBlink();
    }
    /// <summary>
    /// ִ��һ��������գ��Ч������->����
    /// </summary>
    public void DoBlink()
    {
        // ȷ��Image�Ǽ����
        blinkImage.gameObject.SetActive(true);

        // �Ƚ���ɫ����Ϊ��ȫ͸���������ظ�����ʱ״̬����
        Color endColor = blinkImage.color;
        endColor.a = 0f;
        blinkImage.color = endColor;

        // �������ж���
        Sequence blinkSequence = DOTween.Sequence();

        // ��һ�������ٽ��� (Alpha 0 -> 1)
        blinkSequence.Append(blinkImage.DOFade(1f, blinkDuration / 2)); // ʹ��һ���ʱ����

        // �ڶ��������ٽ��� (Alpha 1 -> 0)
        blinkSequence.Append(blinkImage.DOFade(0f, blinkDuration / 2)); // ʹ����һ���ʱ�����

        // ������ɺ��ѡ�����Image���������Ҫһֱ���ڣ�
        blinkSequence.OnComplete(() => {
            // blinkImage.gameObject.SetActive(false); 
            // ͨ����������͸��״̬���ɣ����ÿ��ܻᵼ����Ҫʱ�޷����̼���
        });

        // ��������
        blinkSequence.Play();
    }

    // һ����ѡ�Ĺ������������ڳ����л�������ʱ�̵ġ�����-���ۡ�Ч��
    // �����A�����л���B�������Ⱥ����������곡����������
    public void DoBlinkWithCallback(System.Action onMidBlinkCallback)
    {
        blinkImage.gameObject.SetActive(true);
        Color endColor = blinkImage.color;
        endColor.a = 0f;
        blinkImage.color = endColor;

        Sequence sequence = DOTween.Sequence();
        // ����
        sequence.Append(blinkImage.DOFade(1f, blinkDuration / 2));
        // �ڱ�������ص�ʱ��ִ�лص�����������³�����
        sequence.AppendCallback(() => onMidBlinkCallback?.Invoke());
        // ����
        sequence.Append(blinkImage.DOFade(0f, blinkDuration / 2));
        sequence.Play();
    }

    // ��Inspector�е�����
    [ContextMenu("Test Blink")]
    private void TestBlink()
    {
        DoBlink();
    }
}