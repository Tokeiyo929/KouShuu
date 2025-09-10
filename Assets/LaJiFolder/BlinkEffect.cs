using UnityEngine;
using UnityEngine.UI; // 需要引用UI命名空间来操作Image
using DG.Tweening; // 引用DOTween命名空间

public class BlinkEffect : MonoBehaviour
{
    [Header("References")]
    public Image blinkImage; // 拖拽赋值，关联到我们的BlinkOverlay Image

    [Header("Settings")]
    public float blinkDuration = 0.2f; // 单次渐黑或渐亮的时间

    private void OnEnable()
    {
        // 确保Image是激活的
        blinkImage.gameObject.SetActive(true);

        // 先将颜色设置为完全透明，避免重复调用时状态错误
        Color endColor = blinkImage.color;
        endColor.a = 0f;
        blinkImage.color = endColor;

        // 创建序列动画
        Sequence blinkSequence = DOTween.Sequence();

        // 第一步：快速渐黑 (Alpha 0 -> 1)
        blinkSequence.Append(blinkImage.DOFade(1f, blinkDuration / 2)); // 使用一半的时间变黑

        // 第二步：快速渐亮 (Alpha 1 -> 0)
        blinkSequence.Append(blinkImage.DOFade(0f, blinkDuration / 2)); // 使用另一半的时间变亮

        // 动画完成后可选项：禁用Image（如果不需要一直存在）
        blinkSequence.OnComplete(() => {
            // blinkImage.gameObject.SetActive(false); 
            // 通常让它保持透明状态即可，禁用可能会导致需要时无法立刻激活
        });

        // 播放序列
        blinkSequence.Play();
    }
}