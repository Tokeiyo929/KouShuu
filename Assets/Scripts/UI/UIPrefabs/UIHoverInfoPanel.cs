using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIHoverInfoPanelData : UIPanelData
	{
	}
	public partial class UIHoverInfoPanel : UIPanel
	{
		[Header("鼠标跟随设置")]
		public Vector2 mouseOffset = new Vector2(20f, -20f); // 鼠标偏移量，避免遮挡
		public float smoothSpeed = 10f; // 跟随平滑度
		public bool enableBoundaryCheck = true; // 是否启用边界检测
		public Vector2 boundaryPadding = new Vector2(10f, 10f); // 边界内边距
		
		private RectTransform imgBgRectTransform;
		private Canvas parentCanvas;
		private Camera uiCamera;
		private bool isFollowingMouse = false;
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIHoverInfoPanelData ?? new UIHoverInfoPanelData();
			// please add init code here
			Global.CurrentKeyword.RegisterWithInitValue(newValue=>
			{
				Text_Tips.text=newValue;
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			InitializeMouseFollow();
		}
		
		/// <summary>
		/// 初始化鼠标跟随功能
		/// </summary>
		private void InitializeMouseFollow()
		{
			// 获取Img_BG的RectTransform
			if (Img_BG != null)
			{
				imgBgRectTransform = Img_BG.GetComponent<RectTransform>();
			}
			else
			{
				Debug.LogError("Img_BG 未找到！请检查Designer设置");
				return;
			}
			
			// 获取父Canvas
			parentCanvas = GetComponentInParent<Canvas>();
			if (parentCanvas == null)
			{
				Debug.LogError("未找到父Canvas！");
				return;
			}
			
			// 获取UI摄像机
			uiCamera = parentCanvas.worldCamera;
			if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				uiCamera = null; // Overlay模式不需要摄像机
			}
			
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			// 面板打开时先立即设置到鼠标位置，然后开始跟随
			SetToMousePositionImmediately();
			StartFollowingMouse();
		}
		
		protected override void OnShow()
		{
			// 面板显示时先立即设置到鼠标位置，然后开始跟随
			SetToMousePositionImmediately();
			StartFollowingMouse();
		}
		
		protected override void OnHide()
		{
			// 面板隐藏时停止跟随鼠标
			StopFollowingMouse();
		}
		
		protected override void OnClose()
		{
			// 面板关闭时停止跟随鼠标
			StopFollowingMouse();
		}
		
		private void Update()
		{
			if (isFollowingMouse && imgBgRectTransform != null)
			{
				UpdateMouseFollow();
			}
		}
		
		/// <summary>
		/// 开始跟随鼠标
		/// </summary>
		public void StartFollowingMouse()
		{
			isFollowingMouse = true;
		}
		
		/// <summary>
		/// 停止跟随鼠标
		/// </summary>
		public void StopFollowingMouse()
		{
			isFollowingMouse = false;
		}
		
		/// <summary>
		/// 立即设置到鼠标位置（无平滑过渡）
		/// </summary>
		private void SetToMousePositionImmediately()
		{
			if (imgBgRectTransform == null || parentCanvas == null) return;
			
			// 获取鼠标屏幕坐标
			Vector2 mousePosition = Input.mousePosition;
			
			// 根据鼠标位置设置pivot
			SetPivotBasedOnMousePosition(mousePosition);
			
			// 添加偏移量
			Vector2 targetScreenPosition = mousePosition + mouseOffset;
			
			// 转换为Canvas坐标
			Vector2 canvasPosition;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
				parentCanvas.GetComponent<RectTransform>(), 
				targetScreenPosition, 
				uiCamera, 
				out canvasPosition))
			{
				// 边界检测
				if (enableBoundaryCheck)
				{
					canvasPosition = ClampToCanvasBounds(canvasPosition);
				}
				
				// 立即设置位置，不使用平滑过渡
				imgBgRectTransform.anchoredPosition = canvasPosition;
			}
		}
		
		/// <summary>
		/// 更新鼠标跟随位置
		/// </summary>
		private void UpdateMouseFollow()
		{
			// 获取鼠标屏幕坐标
			Vector2 mousePosition = Input.mousePosition;
			
			// 根据鼠标位置设置pivot
			SetPivotBasedOnMousePosition(mousePosition);
			
			// 添加偏移量
			Vector2 targetScreenPosition = mousePosition + mouseOffset;
			
			// 转换为Canvas坐标
			Vector2 canvasPosition;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
				parentCanvas.GetComponent<RectTransform>(), 
				targetScreenPosition, 
				uiCamera, 
				out canvasPosition))
			{
				// 边界检测
				if (enableBoundaryCheck)
				{
					canvasPosition = ClampToCanvasBounds(canvasPosition);
				}
				
				// 平滑移动
				if (smoothSpeed > 0)
				{
					Vector2 currentPosition = imgBgRectTransform.anchoredPosition;
					Vector2 smoothPosition = Vector2.Lerp(currentPosition, canvasPosition, Time.deltaTime * smoothSpeed);
					imgBgRectTransform.anchoredPosition = smoothPosition;
				}
				else
				{
					// 直接设置位置
					imgBgRectTransform.anchoredPosition = canvasPosition;
				}
			}
		}
		
		/// <summary>
		/// 将位置限制在Canvas边界内
		/// </summary>
		private Vector2 ClampToCanvasBounds(Vector2 position)
		{
			if (parentCanvas == null || imgBgRectTransform == null) return position;
			
			// 获取Canvas的RectTransform
			RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
			if (canvasRect == null) return position;
			
			// 获取Canvas的尺寸
			Vector2 canvasSize = canvasRect.sizeDelta;
			Vector2 imgSize = imgBgRectTransform.sizeDelta;
			
			// 计算边界
			float minX = -canvasSize.x * 0.5f + imgSize.x * 0.5f + boundaryPadding.x;
			float maxX = canvasSize.x * 0.5f - imgSize.x * 0.5f - boundaryPadding.x;
			float minY = -canvasSize.y * 0.5f + imgSize.y * 0.5f + boundaryPadding.y;
			float maxY = canvasSize.y * 0.5f - imgSize.y * 0.5f - boundaryPadding.y;
			
			// 限制位置
			position.x = Mathf.Clamp(position.x, minX, maxX);
			position.y = Mathf.Clamp(position.y, minY, maxY);
			
			return position;
		}
		
		/// <summary>
		/// 根据鼠标位置设置pivot
		/// </summary>
		private void SetPivotBasedOnMousePosition(Vector2 mouseScreenPosition)
		{
			if (imgBgRectTransform == null) return;
			
			// 获取屏幕宽度
			float screenWidth = Screen.width;
			
			// 判断鼠标在左半边还是右半边
			bool isMouseOnLeftHalf = mouseScreenPosition.x < screenWidth * 0.5f;
			
			if (isMouseOnLeftHalf)
			{
				// 左半边：设置pivot为(0,1) - 左上角
				imgBgRectTransform.pivot = new Vector2(0f, 1f);
			}
			else
			{
				// 右半边：设置pivot为(1,1) - 右上角
				imgBgRectTransform.pivot = new Vector2(1f, 1f);
			}
		}
		
		/// <summary>
		/// 设置鼠标偏移量
		/// </summary>
		/// <param name="offset">偏移量</param>
		public void SetMouseOffset(Vector2 offset)
		{
			mouseOffset = offset;
		}
		
		/// <summary>
		/// 设置跟随平滑度
		/// </summary>
		/// <param name="speed">平滑度（0为立即跟随，值越大越平滑）</param>
		public void SetSmoothSpeed(float speed)
		{
			smoothSpeed = speed;
		}
		
		/// <summary>
		/// 启用/禁用边界检测
		/// </summary>
		/// <param name="enable">是否启用</param>
		public void SetBoundaryCheck(bool enable)
		{
			enableBoundaryCheck = enable;
		}
		
		/// <summary>
		/// 手动设置Img_BG位置到鼠标位置（立即生效）
		/// </summary>
		public void SetToMousePosition()
		{
			if (imgBgRectTransform == null) return;
			
			Vector2 mousePosition = Input.mousePosition + (Vector3)mouseOffset;
			Vector2 canvasPosition;
			
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
				parentCanvas.GetComponent<RectTransform>(), 
				mousePosition, 
				uiCamera, 
				out canvasPosition))
			{
				if (enableBoundaryCheck)
				{
					canvasPosition = ClampToCanvasBounds(canvasPosition);
				}
				
				imgBgRectTransform.anchoredPosition = canvasPosition;
			}
		}
	}
}
