using HighlightPlus;
using System;
using UnityEngine;
using UnityEngine.Events;

public class ClickableObject : MonoBehaviour
{
    [Header("瞬时执行")]
    [Tooltip("动画播放持续时间（秒），不要填0")]
    public float animationDuration = 0.0f;
    public bool playTimeLine = false;
    public float timeLineStartTime = 0f; // 时间线开始时间点
    public TimelineController timelineController;
    public HighlightEffect outline;
    public UnityEvent onShunKanClickEvents;

    [Header("延时执行")]
    [Tooltip("延迟执行 onClickEvents 的时间")]
    public float delayTime = 0.5f;
    public UnityEvent onClickEvents;

    [Header("相机偏差修正")]
    public bool isBias = false;
    public float cameraBiasTime = 0f;
    public UnityEvent onBiasCameraEvents;

    [Header("其他")]
    public Animator animator;
    public string triggerName = "None";

    public bool deactiveAfterAnimation = false;
    public bool showUIAfterAnimation = false;
    public bool enterSceneAfterAnimation = false;
    public bool showGameObjectAfterAnimation = false;
    public bool hideGameObjectAfterAnimation = false;

    public GameObject uiPreset;  // 每个物体独立UI预设
    public GameObject showGameObject;
    public GameObject showGameObject02;
    public GameObject hideGameObject;
    public GameObject hideGameObject02;
    public bool HasBeenClicked { get; set; } = false;



    public event Action<ClickableObject> OnClickCompleted;

    public bool IsActive { get; private set; } = false;

    public void Activate()
    {
        IsActive = true;
        if (outline != null) outline.enabled = true;
    }

    public void Deactivate()
    {
        IsActive = false;
        if (outline != null) outline.enabled = false;
    }

    public void TriggerAction()
    {
        Deactivate();

        if (animator != null)
            animator.SetTrigger(triggerName);

        MonoBehaviour mb = Camera.main.GetComponent<MonoBehaviour>();
        if (mb != null)
            mb.StartCoroutine(WaitForSeconds(animationDuration, () =>
            {
                if (showUIAfterAnimation)
                    ShowUI();

                if (showGameObjectAfterAnimation)
                {
                    showGameObject.SetActive(true);
                    if (showGameObject02 != null)
                        showGameObject02.SetActive(true);
                }

                if (hideGameObjectAfterAnimation)
                {
                    hideGameObject.SetActive(false);
                    if (hideGameObject02 != null)
                        hideGameObject02.SetActive(false);
                }

                if (enterSceneAfterAnimation)
                    EnterNewScene();

                if (deactiveAfterAnimation)
                    gameObject.SetActive(false);

                if (playTimeLine)
                    timelineController.PlayTimelineAtTime(timeLineStartTime);

                onShunKanClickEvents?.Invoke();

                OnClickCompleted?.Invoke(this);
            }));

        // 延迟执行 onClickEvents
        if (delayTime > 0)
        {
            mb.StartCoroutine(WaitForSeconds(delayTime, () =>
            {
                onClickEvents?.Invoke();
            }));
        }
        else
        {
            onClickEvents?.Invoke();
        }

        if (isBias)
        {
            mb.StartCoroutine(WaitForSeconds(cameraBiasTime, () =>
            {
                onBiasCameraEvents?.Invoke();
            }));
        }
    }

    // 保留的 OnUIButtonClicked 方法
    public void OnUIButtonClicked()
    {
        if (!IsActive) return;

        // 触发所有在 onClickEvents 中注册的事件
        onClickEvents?.Invoke();

        // 以下是原来的点击处理逻辑
        ClickManager clickManager = FindObjectOfType<ClickManager>();
        if (clickManager != null)
        {
            clickManager.TryClickFromUI(this);
        }
    }

    private System.Collections.IEnumerator WaitForSeconds(float seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }

    public void EnterNewScene()
    {
        // 这里可以添加进入新场景的逻辑
        // 例如：SceneManager.LoadScene("NewSceneName");
        //ProcessFlowManager.Instance.ShowNextStepButton(true);
    }

    private void ShowUI()
    {
        if (uiPreset != null)
        {
            //instantiatedUI = Instantiate(uiPrefab, UIManager.Instance.uiCanvas.transform);
            //UIManager.Instance.ShowPopup(uiPreset);
        }
    }
    public void MovePlayerCamera(Transform ts)
    {
        var camera = GameObject.FindGameObjectWithTag("PlayerCamera");
        camera.GetComponent<SmoothCameraMover>().MoveToTransform(ts);
    }
}
