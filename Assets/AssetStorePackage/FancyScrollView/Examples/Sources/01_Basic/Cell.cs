/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.UI;

namespace FancyScrollView.Example01
{
    class Cell : FancyCell<ItemData>
    {
        [SerializeField] Animator animator = default;
        [SerializeField] Text message = default;
        [SerializeField] Button button = default; // 添加Button组件引用

        static class AnimatorHash
        {
            public static readonly int Scroll = Animator.StringToHash("scroll");
        }

        public override void Initialize()
        {
            base.Initialize();
            
            // 设置Button点击事件监听器
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClick);
            }
        }

        // Button点击事件处理方法
        private void OnButtonClick()
        {
            Debug.Log($"按钮被点击了！物体名称: {gameObject.name}");
        }

        public override void UpdateContent(ItemData itemData)
        {
            message.text = itemData.Message;
        }

        public override void UpdatePosition(float position)
        {
            currentPosition = position;

            if (animator.isActiveAndEnabled)
            {
                animator.Play(AnimatorHash.Scroll, -1, position);
            }

            animator.speed = 0;
        }

        // GameObject が非アクティブになると Animator がリセットされてしまうため
        // 現在位置を保持しておいて OnEnable のタイミングで現在位置を再設定します
        float currentPosition = 0;

        void OnEnable() => UpdatePosition(currentPosition);

        // 清理事件监听器，避免内存泄漏
        void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnButtonClick);
            }
        }
    }
}
