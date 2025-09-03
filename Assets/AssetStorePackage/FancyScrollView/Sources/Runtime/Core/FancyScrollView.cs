/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Collections.Generic;
using UnityEngine;

namespace FancyScrollView
{
    /// <summary>
    /// 实现滚动视图的抽象基类.
    /// 支持无限滚动和捕捉功能.
    /// 如果不需要 <see cref="FancyScrollView{TItemData, TContext}.Context"/>，
    /// 请使用 <see cref="FancyScrollView{TItemData}"/> 代替.
    /// </summary>
    /// <typeparam name="TItemData">项目数据类型.</typeparam>
    /// <typeparam name="TContext"><see cref="Context"/> 的类型.</typeparam>
    public abstract class FancyScrollView<TItemData, TContext> : MonoBehaviour where TContext : class, new()
    {
        /// <summary>
        /// 单元格之间的间隔.
        /// </summary>
        [SerializeField, Range(1e-2f, 1f)] protected float cellInterval = 0.2f;

        /// <summary>
        /// 滚动位置的基准.
        /// </summary>
        /// <remarks>
        /// 例如，指定 <c>0.5</c> 且滚动位置为 <c>0</c> 时，第一个单元格将放置在中央.
        /// </remarks>
        [SerializeField, Range(0f, 1f)] protected float scrollOffset = 0.5f;

        /// <summary>
        /// 是否循环放置单元格.
        /// </summary>
        /// <remarks>
        /// 设为 <c>true</c> 时，最后一个单元格后面排列第一个单元格，第一个单元格前面排列最后一个单元格.
        /// 实现无限滚动时请指定 <c>true</c>.
        /// </remarks>
        [SerializeField] protected bool loop = false;

        /// <summary>
        /// 作为单元格父元素的 <c>Transform</c>.
        /// </summary>
        [SerializeField] protected Transform cellContainer = default;

        readonly IList<FancyCell<TItemData, TContext>> pool = new List<FancyCell<TItemData, TContext>>();

        /// <summary>
        /// 是否已初始化.
        /// </summary>
        protected bool initialized;

        /// <summary>
        /// 当前滚动位置.
        /// </summary>
        protected float currentPosition;

        /// <summary>
        /// 单元格的预制体.
        /// </summary>
        protected abstract GameObject CellPrefab { get; }

        /// <summary>
        /// 项目列表数据.
        /// </summary>
        protected IList<TItemData> ItemsSource { get; set; } = new List<TItemData>();

        /// <summary>
        /// <typeparamref name="TContext"/> 的实例.
        /// 在单元格和滚动视图之间共享同一个实例. 用于信息传递和状态保持.
        /// </summary>
        protected TContext Context { get; } = new TContext();

        /// <summary>
        /// 执行初始化.
        /// </summary>
        /// <remarks>
        /// 在第一次生成单元格之前调用.
        /// </remarks>
        protected virtual void Initialize() { }

        /// <summary>
        /// 根据传入的项目列表更新显示内容.
        /// </summary>
        /// <param name="itemsSource">项目列表.</param>
        protected virtual void UpdateContents(IList<TItemData> itemsSource)
        {
            ItemsSource = itemsSource;
            Refresh();
        }

        /// <summary>
        /// 强制更新单元格布局.
        /// </summary>
        protected virtual void Relayout() => UpdatePosition(currentPosition, false);

        /// <summary>
        /// 强制更新单元格布局和显示内容.
        /// </summary>
        protected virtual void Refresh() => UpdatePosition(currentPosition, true);

        /// <summary>
        /// 更新滚动位置.
        /// </summary>
        /// <param name="position">滚动位置.</param>
        protected virtual void UpdatePosition(float position) => UpdatePosition(position, false);

        void UpdatePosition(float position, bool forceRefresh)
        {
            if (!initialized)
            {
                Initialize();
                initialized = true;
            }

            currentPosition = position;

            var p = position - scrollOffset / cellInterval;
            var firstIndex = Mathf.CeilToInt(p);
            var firstPosition = (Mathf.Ceil(p) - p) * cellInterval;

            if (firstPosition + pool.Count * cellInterval < 1f)
            {
                ResizePool(firstPosition);
            }

            UpdateCells(firstPosition, firstIndex, forceRefresh);
        }

        void ResizePool(float firstPosition)
        {
            Debug.Assert(CellPrefab != null);
            Debug.Assert(cellContainer != null);

            var addCount = Mathf.CeilToInt((1f - firstPosition) / cellInterval) - pool.Count;
            for (var i = 0; i < addCount; i++)
            {
                var cell = Instantiate(CellPrefab, cellContainer).GetComponent<FancyCell<TItemData, TContext>>();
                if (cell == null)
                {
                    throw new MissingComponentException(string.Format(
                        "在 {2} 中未找到 FancyCell<{0}, {1}> 组件.",
                        typeof(TItemData).FullName, typeof(TContext).FullName, CellPrefab.name));
                }

                cell.SetContext(Context);
                cell.Initialize();
                cell.SetVisible(false);
                pool.Add(cell);
            }
        }

        void UpdateCells(float firstPosition, int firstIndex, bool forceRefresh)
        {
            for (var i = 0; i < pool.Count; i++)
            {
                var index = firstIndex + i;
                var position = firstPosition + i * cellInterval;
                var cell = pool[CircularIndex(index, pool.Count)];

                if (loop)
                {
                    index = CircularIndex(index, ItemsSource.Count);
                }

                if (index < 0 || index >= ItemsSource.Count || position > 1f)
                {
                    cell.SetVisible(false);
                    continue;
                }

                if (forceRefresh || cell.Index != index || !cell.IsVisible)
                {
                    cell.Index = index;
                    cell.SetVisible(true);
                    cell.UpdateContent(ItemsSource[index]);
                }

                cell.UpdatePosition(position);
            }
        }

        int CircularIndex(int i, int size) => size < 1 ? 0 : i < 0 ? size - 1 + (i + 1) % size : i % size;

#if UNITY_EDITOR
        bool cachedLoop;
        float cachedCellInterval, cachedScrollOffset;

        void LateUpdate()
        {
            if (cachedLoop != loop ||
                cachedCellInterval != cellInterval ||
                cachedScrollOffset != scrollOffset)
            {
                cachedLoop = loop;
                cachedCellInterval = cellInterval;
                cachedScrollOffset = scrollOffset;

                UpdatePosition(currentPosition);
            }
        }
#endif
    }

    /// <summary>
    /// <see cref="FancyScrollView{TItemData}"/> 的上下文类.
    /// </summary>
    public sealed class NullContext { }

    /// <summary>
    /// 实现滚动视图的抽象基类.
    /// 支持无限滚动和捕捉功能.
    /// </summary>
    /// <typeparam name="TItemData"></typeparam>
    /// <seealso cref="FancyScrollView{TItemData, TContext}"/>
    public abstract class FancyScrollView<TItemData> : FancyScrollView<TItemData, NullContext> { }
}
