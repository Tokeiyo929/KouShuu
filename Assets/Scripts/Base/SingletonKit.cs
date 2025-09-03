using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using UnityEngine.Localization.Settings;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
	public partial class SingletonKit : ViewController
	{
		void Start()
		{
			// Code Here
			var hyperlinkManager = HyperlinkManager.Instance;
			var objectHoverManager = ObjectHoverManager.Instance;
			var gameManager = GameManager.Instance;
			var hintDatabase = GameData.Instance;
            var cameraManager = CameraController.Instance;
		}
	}
	#region 超链接检测
	[MonoSingletonPath("UI/HyperlinkManager")]
	public class HyperlinkManager : MonoBehaviour,ISingleton
	{
		public static HyperlinkManager Instance=>MonoSingletonProperty<HyperlinkManager>.Instance;
		
		[Header("超链接检测设置")]
		public LayerMask textLayerMask = -1;
		public Camera uiCamera;
		
		private Dictionary<string, List<LinkInfo>> linkCache = new Dictionary<string, List<LinkInfo>>();
		private string currentHoveredLink = "";
		
		public void OnSingletonInit()
		{
			Debug.Log("HyperlinkManager Init - 超链接检测功能已启动");
			
			// 自动检测Canvas的渲染模式并设置合适的摄像机
			if (uiCamera == null)
			{
				var canvas = FindObjectOfType<Canvas>();
				if (canvas != null)
				{
					switch (canvas.renderMode)
					{
						case RenderMode.ScreenSpaceOverlay:
							// Overlay模式不需要摄像机，设置为null
							uiCamera = null;
							Debug.Log("检测到Canvas为Overlay模式，不使用摄像机");
							break;
							
						case RenderMode.ScreenSpaceCamera:
							// Camera模式使用Canvas指定的摄像机
							uiCamera = canvas.worldCamera ?? Camera.main;
							Debug.Log($"检测到Canvas为Camera模式，使用摄像机: {uiCamera.name}");
							break;
							
						case RenderMode.WorldSpace:
							// World Space模式使用Canvas指定的摄像机
							uiCamera = canvas.worldCamera ?? Camera.main;
							Debug.Log($"检测到Canvas为WorldSpace模式，使用摄像机: {uiCamera.name}");
							break;
					}
				}
				else
				{
					// 如果没有找到Canvas，默认使用主摄像机
					uiCamera = Camera.main;
					Debug.Log("未找到Canvas，使用主摄像机");
				}
			}
		}
		
		private void Update()
		{
			DetectLinkUnderMouse();
		}
		
		/// <summary>
		/// 检测鼠标下的超链接
		/// </summary>
		private void DetectLinkUnderMouse()
		{
			// 检测鼠标位置下的UI元素
			PointerEventData pointerData = new PointerEventData(EventSystem.current)
			{
				position = Input.mousePosition
			};
			
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerData, results);
			
			string detectedLink = "";
			
			foreach (var result in results)
			{
				// 检查是否是Text组件
				var text = result.gameObject.GetComponent<Text>();
				if (text != null)
				{
					detectedLink = DetectLinkInText(text, Input.mousePosition);
					if (!string.IsNullOrEmpty(detectedLink)) break;
				}
				
				// 检查是否是TextMeshPro组件
				var tmpText = result.gameObject.GetComponent<TextMeshProUGUI>();
				if (tmpText != null)
				{
					detectedLink = DetectLinkInTMPText(tmpText, Input.mousePosition);
					if (!string.IsNullOrEmpty(detectedLink)) break;
				}
			}
			
			// 如果检测到的链接发生变化，输出Debug信息
			if (currentHoveredLink != detectedLink)
			{
				if (!string.IsNullOrEmpty(currentHoveredLink))
				{
					Debug.Log($"鼠标离开链接: {currentHoveredLink}");
					OnLinkExit(currentHoveredLink);
				}
				
				if (!string.IsNullOrEmpty(detectedLink))
				{
					//Debug.Log($"鼠标进入链接: {detectedLink}");
					OnLinkEnter(detectedLink);
				}
				
				currentHoveredLink = detectedLink;
			}
		}
		
		/// <summary>
		/// 检测普通Text组件中的超链接
		/// </summary>
		private string DetectLinkInText(Text text, Vector2 screenPosition)
		{
			if (string.IsNullOrEmpty(text.text)) return "";
			
			// 解析文本中的链接
			var links = ParseLinks(text.text);
			if (links.Count == 0) return "";
			
			// 将屏幕坐标转换为文本的本地坐标
			RectTransform rectTransform = text.GetComponent<RectTransform>();
			Vector2 localPoint;
			// 对于Overlay模式，uiCamera为null是正确的
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				rectTransform, screenPosition, uiCamera, out localPoint);
			
			// 简化的碰撞检测（实际项目中可能需要更精确的字符位置计算）
			// 这里使用近似方法，基于文本的整体区域
			Rect textRect = rectTransform.rect;
			if (textRect.Contains(localPoint))
			{
				// 返回第一个找到的链接（简化处理）
				return links.Count > 0 ? links[0].keyword : "";
			}
			
			return "";
		}
		
		/// <summary>
		/// 检测TextMeshPro组件中的超链接
		/// </summary>
		private string DetectLinkInTMPText(TextMeshProUGUI tmpText, Vector2 screenPosition)
		{
			if (string.IsNullOrEmpty(tmpText.text)) return "";
			
			// TMP有内置的链接检测功能
			// 对于Overlay模式，uiCamera为null是正确的
			int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmpText, screenPosition, uiCamera);
			if (linkIndex != -1)
			{
				var linkInfo = tmpText.textInfo.linkInfo[linkIndex];
				return linkInfo.GetLinkID();
			}
			
			return "";
		}
		
		/// <summary>
		/// 解析文本中的链接标签
		/// </summary>
		private List<LinkInfo> ParseLinks(string text)
		{
			var links = new List<LinkInfo>();
			
			// 正则表达式匹配 <link="关键词"><u>关键词</u></link>
			string pattern = @"<link=""([^""]*)""><u>([^<]*)</u></link>";
			var matches = Regex.Matches(text, pattern);
			
			foreach (Match match in matches)
			{
				if (match.Groups.Count >= 3)
				{
					string keyword = match.Groups[1].Value;
					string displayText = match.Groups[2].Value;
					
					links.Add(new LinkInfo
					{
						keyword = keyword,
						displayText = displayText,
						startIndex = match.Index,
						length = match.Length
					});
				}
			}
			
			return links;
		}
		
		/// <summary>
		/// 当鼠标进入链接时触发
		/// </summary>
		public virtual void OnLinkEnter(string linkKeyword)
		{
			Debug.Log($"[链接事件] 鼠标进入: {linkKeyword}");
			
			// 在这里可以添加更多操作，比如改变鼠标样式、显示提示等
			Global.CurrentKeyword.Value = GameData.Instance.hintDatabase_SO.GetHint(linkKeyword, Global.CurrentLanguage.Value);
			if(UIKit.GetPanel<UIHoverInfoPanel>()==null)
			{
				UIKit.OpenPanel<UIHoverInfoPanel>(UILevel.PopUI, null, null, "UIPrefabs/UIHoverInfoPanel");
			}
		}
		
		/// <summary>
		/// 当鼠标离开链接时触发
		/// </summary>
		public virtual void OnLinkExit(string linkKeyword)
		{
			Debug.Log($"[链接事件] 鼠标离开: {linkKeyword}");
			// 在这里可以添加更多操作
			if(UIKit.GetPanel<UIHoverInfoPanel>()!=null)
			{
				UIKit.ClosePanel<UIHoverInfoPanel>();
			}
		}
		
		/// <summary>
		/// 当点击链接时触发（需要在需要的地方手动调用）
		/// </summary>
		public virtual void OnLinkClick(string linkKeyword)
		{
			Debug.Log($"[链接事件] 点击链接: {linkKeyword}");
			// 在这里处理链接点击事件
		}
		
		/// <summary>
		/// 获取当前鼠标悬停的链接
		/// </summary>
		public string GetCurrentHoveredLink()
		{
			return currentHoveredLink;
		}
	}
	
	/// <summary>
	/// 链接信息结构体
	/// </summary>
	[System.Serializable]
	public class LinkInfo
	{
		public string keyword;      // 链接关键词
		public string displayText;  // 显示文本
		public int startIndex;      // 在原文本中的起始位置
		public int length;          // 链接标签的长度
	}
	
	/// <summary>
	/// 物体悬停管理器 - 检测鼠标悬停在特定Tag的物体上
	/// </summary>
	[MonoSingletonPath("UI/ObjectHoverManager")]
	public class ObjectHoverManager : MonoBehaviour, ISingleton
	{
		public static ObjectHoverManager Instance => MonoSingletonProperty<ObjectHoverManager>.Instance;
		
		[Header("物体悬停检测设置")]
		public string targetTag = "Hoverable"; // 目标标签
		public LayerMask objectLayerMask = -1; // 物体层级遮罩
		public Camera raycastCamera; // 用于射线检测的摄像机
		
		private string currentHoveredObjectName = "";
		private GameObject currentHoveredObject = null;
		private string currentClickedObjectName = ""; // 当前点击的物体名称
		private GameObject currentClickedObject = null; // 当前点击的物体
		
		public void OnSingletonInit()
		{
			Debug.Log("ObjectHoverManager Init - 物体悬停检测功能已启动");
			
			// 如果没有指定摄像机，使用主摄像机
			if (raycastCamera == null)
			{
				raycastCamera = Camera.main;
				if (raycastCamera != null)
				{
					Debug.Log($"ObjectHoverManager: 使用主摄像机进行射线检测");
				}
				else
				{
					Debug.LogWarning("ObjectHoverManager: 未找到主摄像机，请手动指定raycastCamera");
				}
			}
		}
		
		private void Update()
		{
			DetectObjectUnderMouse();
			HandleMouseClick();
		}
		
		/// <summary>
		/// 检测鼠标下的物体
		/// </summary>
		private void DetectObjectUnderMouse()
		{
			if (raycastCamera == null) return;
			
			// 从摄像机发射射线到鼠标位置
			Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			string detectedObjectName = "";
			GameObject detectedObject = null;
			
			// 射线检测
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, objectLayerMask))
			{
				GameObject hitObject = hit.collider.gameObject;
				
				// 检查物体是否有指定的标签
				if (hitObject.CompareTag(targetTag))
				{
					detectedObjectName = hitObject.name;
					detectedObject = hitObject;
				}
			}
			
			// 如果检测到的物体发生变化，触发相应事件
			if (currentHoveredObjectName != detectedObjectName)
			{
				// 鼠标离开之前的物体
				if (!string.IsNullOrEmpty(currentHoveredObjectName) && currentHoveredObject != null)
				{
					Debug.Log($"鼠标离开物体: {currentHoveredObjectName}");
					OnObjectExit(currentHoveredObjectName, currentHoveredObject);
				}
				
				// 鼠标进入新的物体（现在只做记录，不触发面板显示）
				if (!string.IsNullOrEmpty(detectedObjectName) && detectedObject != null)
				{
					Debug.Log($"鼠标进入物体: {detectedObjectName}");
					OnObjectEnter(detectedObjectName, detectedObject);
				}
				
				currentHoveredObjectName = detectedObjectName;
				currentHoveredObject = detectedObject;
			}
		}
		
		/// <summary>
		/// 处理鼠标点击事件
		/// </summary>
		private void HandleMouseClick()
		{
            int mouseButton = 0;
            if(targetTag=="Hoverable")
            {
                mouseButton = 0;
            }
            else if(targetTag=="TeaSet")
            {
                mouseButton = 1;
            }
			if (Input.GetMouseButtonDown(mouseButton)) // 按下左键
			{
				if (!string.IsNullOrEmpty(currentHoveredObjectName) && currentHoveredObject != null)
				{
					// 点击到了Hoverable物体
					Debug.Log($"点击物体: {currentHoveredObjectName}");
					OnObjectClick(currentHoveredObjectName, currentHoveredObject);
					
					// 记录当前点击的物体
					currentClickedObjectName = currentHoveredObjectName;
					currentClickedObject = currentHoveredObject;
					
					// 显示提示面板
					Global.CurrentKeyword.Value = GameData.Instance.hintDatabase_SO.GetHint(currentHoveredObjectName, Global.CurrentLanguage.Value);
					if(UIKit.GetPanel<UIHoverInfoPanel>()==null)
					{
						UIKit.OpenPanel<UIHoverInfoPanel>(UILevel.PopUI, null, null, "UIPrefabs/UIHoverInfoPanel");
					}
				}
			}
		}
		
		/// <summary>
		/// 当鼠标进入物体时触发
		/// </summary>
		/// <param name="objectName">物体名称</param>
		/// <param name="gameObject">物体GameObject</param>
		public virtual void OnObjectEnter(string objectName, GameObject gameObject)
		{
			Debug.Log($"[物体悬停事件] 鼠标进入: {objectName}");
			// 现在只记录悬停状态，不立即显示面板
		}
		
		/// <summary>
		/// 当鼠标离开物体时触发
		/// </summary>
		/// <param name="objectName">物体名称</param>
		/// <param name="gameObject">物体GameObject</param>
		public virtual void OnObjectExit(string objectName, GameObject gameObject)
		{
			Debug.Log($"[物体悬停事件] 鼠标离开: {objectName}");
			
			// 如果离开的是当前点击的物体，关闭面板
			if (objectName == currentClickedObjectName && gameObject == currentClickedObject)
			{
				if(UIKit.GetPanel<UIHoverInfoPanel>()!=null)
				{
					UIKit.ClosePanel<UIHoverInfoPanel>();
				}
				
				// 清除点击记录
				currentClickedObjectName = "";
				currentClickedObject = null;
			}
		}
		
		/// <summary>
		/// 当点击物体时触发（需要在需要的地方手动调用）
		/// </summary>
		/// <param name="objectName">物体名称</param>
		/// <param name="gameObject">物体GameObject</param>
		public virtual void OnObjectClick(string objectName, GameObject gameObject)
		{
			Debug.Log($"[物体悬停事件] 点击物体: {objectName}");
			// 在这里处理物体点击事件
		}
		
		/// <summary>
		/// 获取当前鼠标悬停的物体名称
		/// </summary>
		public string GetCurrentHoveredObjectName()
		{
			return currentHoveredObjectName;
		}
		
		/// <summary>
		/// 获取当前鼠标悬停的物体
		/// </summary>
		public GameObject GetCurrentHoveredObject()
		{
			return currentHoveredObject;
		}
		
		/// <summary>
		/// 设置目标标签
		/// </summary>
		/// <param name="tag">新的目标标签</param>
		public void SetTargetTag(string tag)
		{
			targetTag = tag;
			Debug.Log($"ObjectHoverManager: 目标标签已设置为 {tag}");
		}
		
		/// <summary>
		/// 设置射线检测摄像机
		/// </summary>
		/// <param name="camera">用于射线检测的摄像机</param>
		public void SetRaycastCamera(Camera camera)
		{
			raycastCamera = camera;
			Debug.Log($"ObjectHoverManager: 射线检测摄像机已设置为 {camera.name}");
		}
	}
	#endregion

	#region GameMnager
	[MonoSingletonPath("Game/GameManager")]
	public class GameManager : MonoBehaviour,ISingleton
	{
		public static GameManager Instance=>MonoSingletonProperty<GameManager>.Instance;
		
		public void OnSingletonInit()
		{
			Global.CurrentLanguage.Register(newValue=>
			{
				//StartCoroutine(SetLocalse(newValue==GlobalEnums.Language.Chinese?0:1));
				
			}).UnRegisterWhenGameObjectDestroyed(gameObject);

            
            // 先初始化ResKit，然后再打开UI
            
            
            
		}


		// private IEnumerator InitializeGame()
		// {
		// 	// 等待ResKit初始化完成
		// 	yield return StartCoroutine(ResKit.InitAsync());
			
		// }

		//chinese:0  English:1
		IEnumerator SetLocalse(int id)
		{
			yield return LocalizationSettings.InitializationOperation;
			LocalizationSettings.SelectedLocale=LocalizationSettings.AvailableLocales.Locales[id];
			yield return new WaitForSeconds(0.3f);
			RefreshPage();
		}

		public void RefreshPage()
		{
			//找到所有recttransform
			RectTransform[] rectTransforms = FindObjectsOfType<RectTransform>();
			foreach (RectTransform rectTransform in rectTransforms)
			{
				//刷新
				LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			}
			
		}

	}
	
	[MonoSingletonPath("Game/AudioManager")]
    public class AudioManager : MonoBehaviour,ISingleton
    {
        public static AudioManager Instance=>MonoSingletonProperty<AudioManager>.Instance;

        [Header("音频设置")]
        [Range(0, 1)] public float voiceVolume = 1f;
        [Range(0, 1)] public float musicVolume = 1f;
        
        private AudioSource voiceSource;
        private AudioSource musicSource;
        
        // 音频缓存
        private Dictionary<string, AudioClip> audioCache = new Dictionary<string, AudioClip>();

        public void OnSingletonInit()
        {
            // 创建语音AudioSource
            GameObject voiceObj = new GameObject("VoiceSource");
            voiceObj.transform.SetParent(transform);
            voiceSource = voiceObj.AddComponent<AudioSource>();
            voiceSource.playOnAwake = false;
            voiceSource.volume = voiceVolume;
            
            // 创建音乐AudioSource
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            
            Debug.Log("AudioManager: 初始化完成");
        }
        
        /// <summary>
        /// 播放音频文件
        /// </summary>
        /// <param name="audioName">音频文件名（不需要扩展名）</param>
        /// <param name="loop">是否循环播放</param>
        /// <param name="onStart">播放开始回调</param>
        /// <param name="onFinish">播放结束回调</param>
        public void PlayAudio(string audioName, bool loop = false, System.Action onStart = null, System.Action onFinish = null)
        {
            if (string.IsNullOrEmpty(audioName))
            {
                Debug.LogWarning("AudioManager: 音频名称不能为空");
                return;
            }

            StartCoroutine(PlayAudioCoroutine(audioName, loop, onStart, onFinish));
        }
        
        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="musicName">音乐文件名（不需要扩展名）</param>
        /// <param name="loop">是否循环播放</param>
        /// <param name="volume">音量大小(0-1)</param>
        /// <param name="onStart">播放开始回调</param>
        /// <param name="onFinish">播放结束回调</param>
        public void PlayMusic(string musicName, bool loop = true, float volume = 1f, System.Action onStart = null, System.Action onFinish = null)
        {
            if (string.IsNullOrEmpty(musicName))
            {
                Debug.LogWarning("AudioManager: 音乐名称不能为空");
                return;
            }

            StartCoroutine(PlayMusicCoroutine(musicName, loop, volume, onStart, onFinish));
        }
        
        /// <summary>
        /// 异步播放音频协程
        /// </summary>
        private IEnumerator PlayAudioCoroutine(string audioName, bool loop, System.Action onStart, System.Action onFinish)
        {
            AudioClip clip = null;
            yield return StartCoroutine(LoadAudioClip(audioName, (loadedClip) => { clip = loadedClip; }));
            
            if (clip == null)
            {
                Debug.LogError($"AudioManager: 加载音频失败 - {audioName}");
                yield break;
            }
            
            // 停止当前播放的语音
            if (voiceSource.isPlaying)
            {
                voiceSource.Stop();
            }
            
            voiceSource.clip = clip;
            voiceSource.loop = loop;
            voiceSource.volume = voiceVolume;
            voiceSource.Play();
            
            onStart?.Invoke();
            Debug.Log($"AudioManager: 播放音频 Audio/{audioName}");
            
            // 如果需要监听播放结束
            if (onFinish != null && !loop)
            {
                StartCoroutine(WaitForAudioFinish(voiceSource, onFinish));
            }
        }
        
        /// <summary>
        /// 异步播放音乐协程
        /// </summary>
        private IEnumerator PlayMusicCoroutine(string musicName, bool loop, float volume, System.Action onStart, System.Action onFinish)
        {
            AudioClip clip = null;
            yield return StartCoroutine(LoadAudioClip(musicName, (loadedClip) => { clip = loadedClip; }));
            
            if (clip == null)
            {
                Debug.LogError($"AudioManager: 加载音乐失败 - {musicName}");
                yield break;
            }
            
            // 停止当前播放的音乐
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }
            
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.volume = musicVolume * volume;
            musicSource.Play();
            
            onStart?.Invoke();
            Debug.Log($"AudioManager: 播放音乐 Audio/{musicName}");
            
            // 如果需要监听播放结束
            if (onFinish != null && !loop)
            {
                StartCoroutine(WaitForAudioFinish(musicSource, onFinish));
            }
        }
        
        /// <summary>
        /// 从Resources/Audio文件夹加载音频
        /// </summary>
        private IEnumerator LoadAudioClip(string audioName, System.Action<AudioClip> onComplete)
        {
            string audioPath = $"Audio/{audioName}";
            
            // 检查缓存
            if (audioCache.ContainsKey(audioPath))
            {
                onComplete?.Invoke(audioCache[audioPath]);
                yield break;
            }
            
            // 异步加载音频
            ResourceRequest request = Resources.LoadAsync<AudioClip>(audioPath);
            yield return request;
            
            AudioClip clip = request.asset as AudioClip;
            if (clip != null)
            {
                // 缓存音频
                if (!audioCache.ContainsKey(audioPath))
                {
                    audioCache[audioPath] = clip;
                }
            }
            
            onComplete?.Invoke(clip);
        }
        
        /// <summary>
        /// 等待音频播放结束
        /// </summary>
        private IEnumerator WaitForAudioFinish(AudioSource source, System.Action onFinish)
        {
            while (source.isPlaying)
            {
                yield return null;
            }
            onFinish?.Invoke();
        }
        
        /// <summary>
        /// 停止当前播放的语音
        /// </summary>
        public void StopVoice()
        {
            if (voiceSource.isPlaying)
            {
                voiceSource.Stop();
                Debug.Log("AudioManager: 停止语音播放");
            }
        }
        
        /// <summary>
        /// 停止当前播放的背景音乐
        /// </summary>
        public void StopMusic()
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
                Debug.Log("AudioManager: 停止背景音乐播放");
            }
        }
        
        /// <summary>
        /// 暂停语音播放
        /// </summary>
        public void PauseVoice()
        {
            if (voiceSource.isPlaying)
            {
                voiceSource.Pause();
                Debug.Log("AudioManager: 暂停语音播放");
            }
        }
        
        /// <summary>
        /// 继续语音播放
        /// </summary>
        public void ResumeVoice()
        {
            if (!voiceSource.isPlaying && voiceSource.clip != null)
            {
                voiceSource.UnPause();
                Debug.Log("AudioManager: 继续语音播放");
            }
        }
        
        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public void PauseMusic()
        {
            if (musicSource.isPlaying)
            {
                musicSource.Pause();
                Debug.Log("AudioManager: 暂停背景音乐");
            }
        }
        
        /// <summary>
        /// 继续背景音乐播放
        /// </summary>
        public void ResumeMusic()
        {
            if (!musicSource.isPlaying && musicSource.clip != null)
            {
                musicSource.UnPause();
                Debug.Log("AudioManager: 继续背景音乐播放");
            }
        }
        
        /// <summary>
        /// 停止所有音频播放
        /// </summary>
        public void StopAll()
        {
            StopVoice();
            StopMusic();
            Debug.Log("AudioManager: 停止所有音频播放");
        }
        
        /// <summary>
        /// 设置语音音量
        /// </summary>
        /// <param name="volume">音量大小(0-1)</param>
        public void SetVoiceVolume(float volume)
        {
            voiceVolume = Mathf.Clamp01(volume);
            if (voiceSource != null)
            {
                voiceSource.volume = voiceVolume;
            }
            Debug.Log($"AudioManager: 设置语音音量为 {volume}");
        }
        
        /// <summary>
        /// 设置音乐音量
        /// </summary>
        /// <param name="volume">音量大小(0-1)</param>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
            {
                musicSource.volume = musicVolume;
            }
            Debug.Log($"AudioManager: 设置音乐音量为 {volume}");
        }
        
        /// <summary>
        /// 检查语音是否正在播放
        /// </summary>
        public bool IsVoicePlaying()
        {
            return voiceSource != null && voiceSource.isPlaying;
        }
        
        /// <summary>
        /// 检查音乐是否正在播放
        /// </summary>
        public bool IsMusicPlaying()
        {
            return musicSource != null && musicSource.isPlaying;
        }
        
        /// <summary>
        /// 清理音频缓存
        /// </summary>
        public void ClearAudioCache()
        {
            audioCache.Clear();
            Debug.Log("AudioManager: 清理音频缓存");
        }
        
        private void OnDestroy()
        {
            ClearAudioCache();
        }
    }
	

    [MonoSingletonPath("Game/SceneManager")]
    public class SceneManager : MonoBehaviour,ISingleton
    {
        public static SceneManager Instance=>MonoSingletonProperty<SceneManager>.Instance;
        
        #region 功能索引 - 快速查找方法
        /*
        ═══════════════════════════════════════════════════════════════
                                  功能索引
        ═══════════════════════════════════════════════════════════════
        
        ┌─ 常规场景管理 (scenes列表)
        │   ├─ StartScene()                    // 开始加载第一个场景
        │   ├─ NextScene()                     // 加载下一个场景
        │   ├─ LastScene()                     // 加载上一个场景
        │   └─ LoadSceneByName(string)         // 通过名称加载场景
        │
        ┌─ 额外场景管理 (scenes列表外)
        │   ├─ LoadExtraScene(string)          // 加载额外场景
        │   └─ UnloadExtraScene()              // 强制卸载额外场景
        │
        ┌─ 卸载操作
        │   └─ UnloadCurrentScene()            // 卸载当前场景(优先额外场景)
        │
        ┌─ 状态查询
        │   ├─ GetCurrentSceneName()           // 获取当前常规场景名称
        │   ├─ GetCurrentExtraSceneName()      // 获取当前额外场景名称
        │   ├─ GetCurrentSceneIndex()          // 获取当前场景索引
        │   ├─ IsTransitioning()               // 检查常规场景是否在切换
        │   └─ IsExtraSceneTransitioning()     // 检查额外场景是否在切换
        │
        ┌─ 场景列表管理
        │   ├─ GetAllScenes()                  // 获取所有场景列表
        │   ├─ SetScenesList(List<string>)     // 设置场景列表
        │   ├─ AddScene(string)                // 添加场景到列表
        │   └─ RemoveScene(string)             // 从列表移除场景
        │
        └─ 内部方法 (私有)
            ├─ LoadSceneByIndex(int)           // 根据索引加载场景
            ├─ SwitchToScene(string)           // 切换到指定场景
            ├─ SwitchToExtraScene(string)      // 切换到额外场景
            ├─ LoadSceneAsync(string)          // 异步加载场景
            ├─ LoadExtraSceneAsync(string)     // 异步加载额外场景
            ├─ UnloadSceneAsync(string)        // 异步卸载场景
            ├─ UnloadCurrentSceneCoroutine()   // 卸载当前场景协程
            └─ UnloadCurrentExtraSceneCoroutine() // 卸载额外场景协程
        
        ═══════════════════════════════════════════════════════════════
        */
        #endregion
        
        [Header("场景列表")]
        public List<string> scenes = new List<string>() { "QH", "LC", "ST", "SY", "MD", "QD" };
        
        private int currentSceneIndex = -1; // -1 表示还没有开始加载场景
        private string currentLoadedScene = ""; // 跟踪当前加载的场景名称
        private bool isTransitioning = false; // 防止在切换期间重复操作
        
        // 新增：额外场景管理
        private string currentExtraScene = ""; // 跟踪当前加载的额外场景
        private bool isExtraSceneTransitioning = false; // 额外场景切换状态

        public void OnSingletonInit()
        {
            Debug.Log("SceneManager初始化完成");
        }

        /// <summary>
        /// 开始加载第一个场景
        /// </summary>
        public void StartScene()
        {
            if (scenes == null || scenes.Count == 0)
            {
                Debug.LogWarning("Scenes列表为空！");
                return;
            }

            if (isTransitioning)
            {
                Debug.Log("正在场景切换中，请等待...");
                return;
            }

            currentSceneIndex = 0;
            LoadSceneByIndex(currentSceneIndex);
        }

        /// <summary>
        /// 加载下一个场景
        /// </summary>
        public void NextScene()
        {
            if (scenes == null || scenes.Count == 0)
            {
                Debug.LogWarning("Scenes列表为空！");
                return;
            }

            if (currentSceneIndex == -1)
            {
                Debug.LogWarning("请先调用StartScene()开始场景！");
                return;
            }

            if (isTransitioning)
            {
                Debug.Log("正在场景切换中，请等待...");
                return;
            }

            // 检查是否还有下一个场景
            if (currentSceneIndex + 1 >= scenes.Count)
            {
                Debug.Log("已经是最后一个场景了！");
                return;
            }

            currentSceneIndex++;
            StartCoroutine(SwitchToScene(scenes[currentSceneIndex]));
        }

        /// <summary>
        /// 加载上一个场景
        /// </summary>
        public void LastScene()
        {
            if (scenes == null || scenes.Count == 0)
            {
                Debug.LogWarning("Scenes列表为空！");
                return;
            }

            if (currentSceneIndex <= 0)
            {
                Debug.Log("已经是第一个场景了！");
                return;
            }

            if (isTransitioning)
            {
                Debug.Log("正在场景切换中，请等待...");
                return;
            }

            currentSceneIndex--;
            StartCoroutine(SwitchToScene(scenes[currentSceneIndex]));
        }

        /// <summary>
        /// 通过场景名称加载场景
        /// </summary>
        /// <param name="sceneName">要加载的场景名称</param>
        public void LoadSceneByName(string sceneName)
        {
            // 查找场景是否存在
            int targetIndex = -1;
            for (int i = 0; i < scenes.Count; i++)
            {
                if (scenes[i] == sceneName)
                {
                    targetIndex = i;
                    break;
                }
            }

            if (targetIndex == -1)
            {
                Debug.LogError($"场景 '{sceneName}' 不存在于场景列表中！");
                return;
            }

            if (isTransitioning)
            {
                Debug.Log("正在场景切换中，请等待...");
                return;
            }

            // 如果是当前场景，不需要切换
            if (sceneName == currentLoadedScene)
            {
                Debug.Log($"当前已经是场景 '{sceneName}'");
                return;
            }

            currentSceneIndex = targetIndex;
            StartCoroutine(SwitchToScene(sceneName));
        }

        /// <summary>
        /// 加载额外场景（不在scenes列表中的场景）
        /// </summary>
        /// <param name="sceneName">要加载的额外场景名称</param>
        public void LoadExtraScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("场景名称不能为空！");
                return;
            }

            if (isExtraSceneTransitioning)
            {
                Debug.Log("正在额外场景切换中，请等待...");
                return;
            }

            // 如果是当前额外场景，不需要切换
            if (sceneName == currentExtraScene)
            {
                Debug.Log($"当前已经是额外场景 '{sceneName}'");
                return;
            }

            StartCoroutine(SwitchToExtraScene(sceneName));
            currentExtraScene=sceneName;
        }

        /// <summary>
        /// 切换到额外场景（先卸载当前额外场景再加载新场景）
        /// </summary>
        private IEnumerator SwitchToExtraScene(string sceneName)
        {
            isExtraSceneTransitioning = true;

            // 先卸载当前额外场景
            if (!string.IsNullOrEmpty(currentExtraScene))
            {
                yield return StartCoroutine(UnloadSceneAsync(currentExtraScene));
                Debug.Log("调用第一个卸载");
                currentExtraScene = "";
            }

            // 然后卸载常规场景
            if (!string.IsNullOrEmpty(currentLoadedScene))
            {
                Debug.Log("准备卸载常规场景"+currentLoadedScene);
                if (isTransitioning)
                {
                    Debug.Log("正在场景切换中，无法卸载...");
                }

                StartCoroutine(UnloadCurrentSceneCoroutine());
            }
            
            // 加载新的额外场景
            yield return StartCoroutine(LoadExtraSceneAsync(sceneName));

            isExtraSceneTransitioning = false;
        }

        /// <summary>
        /// 异步加载额外场景
        /// </summary>
        private IEnumerator LoadExtraSceneAsync(string sceneName)
        {
            Debug.Log($"开始加载额外场景: {sceneName}");
            
            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            currentExtraScene = sceneName;
            Debug.Log($"额外场景 {sceneName} 加载完成！");
        }

        /// <summary>
        /// 根据索引加载场景
        /// </summary>
        private void LoadSceneByIndex(int index)
        {
            if (index < 0 || index >= scenes.Count)
            {
                Debug.LogError($"场景索引 {index} 超出范围！");
                return;
            }

            string sceneName = scenes[index];
            Debug.Log($"开始加载场景: {sceneName}");
            
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        /// <summary>
        /// 切换到指定场景（先卸载当前场景再加载新场景）
        /// </summary>
        private IEnumerator SwitchToScene(string sceneName)
        {
            isTransitioning = true;

            // 先卸载当前场景
            if (!string.IsNullOrEmpty(currentLoadedScene))
            {
                yield return StartCoroutine(UnloadSceneAsync(currentLoadedScene));
                Debug.Log("调用第二个卸载");
            }
            
            // 加载新场景
            yield return StartCoroutine(LoadSceneAsync(sceneName));

            isTransitioning = false;
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            Debug.Log($"开始加载场景: {sceneName}");
            
            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            currentLoadedScene = sceneName;
            Debug.Log($"场景 {sceneName} 加载完成！");
            Global.CurrentScene.Value=sceneName;
        }

        /// <summary>
        /// 异步卸载场景
        /// </summary>
        private IEnumerator UnloadSceneAsync(string sceneName)
        {
            Debug.Log($"开始卸载场景: {sceneName}");
            AsyncOperation unloadOp = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            
            while (!unloadOp.isDone)
            {
                yield return null;
            }
            
            Debug.Log($"场景 {sceneName} 卸载完成！");
        }

        /// <summary>
        /// 获取当前场景名称
        /// </summary>
        public string GetCurrentSceneName()
        {
            return currentLoadedScene==""?currentExtraScene:currentLoadedScene;
        }

        /// <summary>
        /// 获取当前额外场景名称
        /// </summary>
        public string GetCurrentExtraSceneName()
        {
            return currentExtraScene;
        }

        /// <summary>
        /// 获取当前场景索引
        /// </summary>
        public int GetCurrentSceneIndex()
        {
            return currentSceneIndex;
        }

        /// <summary>
        /// 获取所有场景列表
        /// </summary>
        public List<string> GetAllScenes()
        {
            return new List<string>(scenes);
        }

        /// <summary>
        /// 检查是否正在切换场景
        /// </summary>
        public bool IsTransitioning()
        {
            return isTransitioning;
        }

        /// <summary>
        /// 检查是否正在切换额外场景
        /// </summary>
        public bool IsExtraSceneTransitioning()
        {
            return isExtraSceneTransitioning;
        }

        /// <summary>
        /// 卸载当前场景（优先卸载额外场景，如果没有额外场景则卸载常规场景）
        /// </summary>
        public void UnloadCurrentScene()
        {
            // 优先卸载额外场景
            if (!string.IsNullOrEmpty(currentExtraScene))
            {
                if (isExtraSceneTransitioning)
                {
                    Debug.Log("正在额外场景切换中，无法卸载...");
                    return;
                }

                StartCoroutine(UnloadCurrentExtraSceneCoroutine());
                return;
            }
            
            // 如果没有额外场景，卸载常规场景
            else if (!string.IsNullOrEmpty(currentLoadedScene))
            {
                if (isTransitioning)
                {
                    Debug.Log("正在场景切换中，无法卸载...");
                    return;
                }

                StartCoroutine(UnloadCurrentSceneCoroutine());
            }
            else
            {
                Debug.Log("没有当前加载的场景可以卸载");
            }
        }

        /// <summary>
        /// 强制卸载额外场景
        /// </summary>
        public void UnloadExtraScene()
        {
            if (!string.IsNullOrEmpty(currentExtraScene))
            {
                if (isExtraSceneTransitioning)
                {
                    Debug.Log("正在额外场景切换中，无法卸载...");
                    return;
                }

                StartCoroutine(UnloadCurrentExtraSceneCoroutine());
            }
            else
            {
                Debug.Log("没有当前加载的额外场景可以卸载");
            }
        }

        /// <summary>
        /// 卸载当前额外场景的协程
        /// </summary>
        private IEnumerator UnloadCurrentExtraSceneCoroutine()
        {
            isExtraSceneTransitioning = true;
            yield return StartCoroutine(UnloadSceneAsync(currentExtraScene));
            Debug.Log("调用第三个卸载");
            currentExtraScene = "";
            isExtraSceneTransitioning = false;
            Debug.Log("额外场景已卸载");
        }

        /// <summary>
        /// 卸载当前场景的协程
        /// </summary>
        private IEnumerator UnloadCurrentSceneCoroutine()
        {
            isTransitioning = true;
            yield return StartCoroutine(UnloadSceneAsync(currentLoadedScene));
            Debug.Log("调用第四个卸载");
            currentLoadedScene = "";
            currentSceneIndex = -1;
            isTransitioning = false;
        }

        /// <summary>
        /// 设置场景列表
        /// </summary>
        public void SetScenesList(List<string> newScenes)
        {
            if (newScenes != null && newScenes.Count > 0)
            {
                scenes = new List<string>(newScenes);
                Debug.Log($"场景列表已更新，共 {scenes.Count} 个场景");
            }
            else
            {
                Debug.LogWarning("新场景列表为空或无效！");
            }
        }

        /// <summary>
        /// 添加场景到列表
        /// </summary>
        public void AddScene(string sceneName)
        {
            if (!string.IsNullOrEmpty(sceneName) && !scenes.Contains(sceneName))
            {
                scenes.Add(sceneName);
                Debug.Log($"已添加场景: {sceneName}");
            }
            else
            {
                Debug.LogWarning($"场景 {sceneName} 已存在或无效！");
            }
        }

        /// <summary>
        /// 从列表中移除场景
        /// </summary>
        public void RemoveScene(string sceneName)
        {
            if (scenes.Contains(sceneName))
            {
                scenes.Remove(sceneName);
                Debug.Log($"已移除场景: {sceneName}");
            }
            else
            {
                Debug.LogWarning($"场景 {sceneName} 不存在于列表中！");
            }
        }
    }
    #endregion

	#region 游戏数据
	[MonoSingletonPath("Game/GameData")]
	public class GameData : MonoBehaviour,ISingleton
	{
		public static GameData Instance=>MonoSingletonProperty<GameData>.Instance;
		private ResLoader mResLoader = ResLoader.Allocate();

		public HintDatabase hintDatabase_SO;
		public void OnSingletonInit()
		{
			//使用Resources/ScriptableObjects/Hint/HintDatabase_SO
			hintDatabase_SO = Resources.Load<HintDatabase>("ScriptableObjects/Hint/HintDatabase_SO");
			
			if (hintDatabase_SO == null)
			{
				Debug.LogError("HintDatabase 加载失败！请检查文件路径和名称");
			}
			else
			{
				Debug.Log("HintDatabase 加载成功");
                foreach (var entry in hintDatabase_SO.entries)
                {
                    Debug.Log($"Key: {entry.key}, Value: {entry.hintText_CN}, {entry.hintText_EN}");
                }
			}
		}

		
	}
	#endregion

}

