using UnityEngine;
using DG.Tweening; // 引入 DOTween 命名空间
using UnityEngine.UI;

public class ImageFlicker : MonoBehaviour
{
    public Image targetImage; // 目标 Image
    public float flickerDuration = 0.5f; // 每次闪烁的持续时间
    public float flickerInterval = 1f; // 闪烁间隔

    void Start()
    {
        // 初始化闪烁效果
        StartFlicker();
    }

    void StartFlicker()
    {
        // 设置图片闪烁的循环动画
        targetImage.DOFade(0f, flickerDuration) // 透明度从当前到 0
            .SetLoops(-1, LoopType.Yoyo) // 无限循环，Yoyo 表示从 0 到 1 再回到 0
            .SetDelay(flickerInterval); // 闪烁间隔时间
    }
}
