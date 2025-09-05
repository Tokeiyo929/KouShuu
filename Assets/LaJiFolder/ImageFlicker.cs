using UnityEngine;
using DG.Tweening; // ���� DOTween �����ռ�
using UnityEngine.UI;

public class ImageFlicker : MonoBehaviour
{
    public Image targetImage; // Ŀ�� Image
    public float flickerDuration = 0.5f; // ÿ����˸�ĳ���ʱ��
    public float flickerInterval = 1f; // ��˸���

    void Start()
    {
        // ��ʼ����˸Ч��
        StartFlicker();
    }

    void StartFlicker()
    {
        // ����ͼƬ��˸��ѭ������
        targetImage.DOFade(0f, flickerDuration) // ͸���ȴӵ�ǰ�� 0
            .SetLoops(-1, LoopType.Yoyo) // ����ѭ����Yoyo ��ʾ�� 0 �� 1 �ٻص� 0
            .SetDelay(flickerInterval); // ��˸���ʱ��
    }
}
