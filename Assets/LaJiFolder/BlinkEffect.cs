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
        DoBlink();
    }
    /// <summary>
    /// 执行一次完整的眨眼效果（黑->亮）
    /// </summary>
    public void DoBlink()
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

    // 一个可选的公共方法，用于场景切换或特殊时刻的“闭眼-睁眼”效果
    // 比如从A场景切换到B场景，先黑屏，加载完场景后再亮屏
    public void DoBlinkWithCallback(System.Action onMidBlinkCallback)
    {
        blinkImage.gameObject.SetActive(true);
        Color endColor = blinkImage.color;
        endColor.a = 0f;
        blinkImage.color = endColor;

        Sequence sequence = DOTween.Sequence();
        // 渐黑
        sequence.Append(blinkImage.DOFade(1f, blinkDuration / 2));
        // 在变黑最严重的时候执行回调（例如加载新场景）
        sequence.AppendCallback(() => onMidBlinkCallback?.Invoke());
        // 渐亮
        sequence.Append(blinkImage.DOFade(0f, blinkDuration / 2));
        sequence.Play();
    }

    // 在Inspector中调试用
    [ContextMenu("Test Blink")]
    private void TestBlink()
    {
        DoBlink();
    }
}