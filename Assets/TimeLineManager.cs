using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using FSM;

namespace QFramework.Example
{
    [MonoSingletonPath("TimeLineManager")]
    public class TimeLineManager : MonoBehaviour,ISingleton
    {
        //public string targetSceneName = "GC"; // 有 Timeline 和人物的场景
		private PlayableDirector targetDirector;
        private Machine FsmManager;
        [SerializeField]
        private List<GameObject> AddCanvasList;
        private ClickManager clickManager;

        private string currentSceneName{get;set;}

        private bool isSceneLoaded = false;

       void ISingleton.OnSingletonInit()
        {
        }
        //单例
        private static TimeLineManager instance;
        public static TimeLineManager Instance
        {
            get
            {
                if (!instance)
                {
                    var uiRoot = UIRoot.Instance;
                    Debug.Log("currentUIRoot:" + uiRoot);
                    instance = MonoSingletonProperty<TimeLineManager>.Instance;
                }
                return instance;
            }
        }

        public void LoadScene(string sceneName)
        {
            if (isSceneLoaded && currentSceneName == sceneName)
            {
                Debug.Log("Scene already loaded.");
                return;
            }

            if (currentSceneName != null && IsSceneLoaded(currentSceneName))
            {
                // 卸载场景并确保其完成
                StartCoroutine(UnloadSceneAsync(currentSceneName, () =>
                {
                    currentSceneName = sceneName;
                    isSceneLoaded = false;
                    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                    StartCoroutine(FindDirectorAfterLoad());
                }));
            }
            else
            {
                currentSceneName = sceneName;
                isSceneLoaded = false;
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                StartCoroutine(FindDirectorAfterLoad());
            }

            //clickManager.ResetCamera();
        }

        private bool IsSceneLoaded(string sceneName)
        {
            // 检查场景是否已加载
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                if (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).name == sceneName)
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerator UnloadSceneAsync(string sceneName, System.Action onComplete)
        {
            // 检查场景是否已加载
            if (!IsSceneLoaded(sceneName))
            {
                Debug.LogWarning("Scene to unload is not loaded: " + sceneName);
                onComplete?.Invoke();
                yield break;
            }

            AsyncOperation unloadOp = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            while (!unloadOp.isDone)
            {
                yield return null;
            }
            onComplete?.Invoke();
        }



        public void UnloadScene(string sceneName)
        {
            //1.先卸载场景里的ui，而不是新场景的ui
            foreach (GameObject GO in AddCanvasList)
            {
                GO.SetActive(false);
            }
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            currentSceneName = null;
        }

        private IEnumerator FindDirectorAfterLoad()
        {
            // 等场景真正加载完成
            yield return null;

            Scene loadedScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(currentSceneName);
            while (!loadedScene.isLoaded)
            {
                yield return null;
            }

            foreach (GameObject root in loadedScene.GetRootGameObjects())
            {
                var director = root.GetComponent<PlayableDirector>();
                if (director != null)
                {
                    targetDirector = director;
                    FsmManager = root.GetComponentInChildren<Machine>();
                    isSceneLoaded = true;
                    Debug.Log("Found targetDirector: " + targetDirector.name);
                    yield break;
                }
            }

            Debug.LogWarning("PlayableDirector not found in scene!");
        }

        public void PlayTimeline(double time)
        {
            StartCoroutine(PlayWhenReady(time));
        }

        private IEnumerator PlayWhenReady(double time)
        {
            // 等待 targetDirector 准备好
            while (!isSceneLoaded || targetDirector == null)
            {
                yield return null;
            }

            targetDirector.time = time;
            targetDirector.Play();
            Debug.Log("Playing timeline at time: " + time);
        }

        public void ChangeToState(string stateName)
        {
            StartCoroutine(ChangeToStateWhenReady(stateName));
        }

        private IEnumerator ChangeToStateWhenReady(string stateName)
        {
            while (!isSceneLoaded || FsmManager == null)
            {
                yield return null;
            }
            FsmManager.ChangeToStateByName(stateName);
            Debug.Log("Changing to state: " + stateName);
        }

        public string GetCurrentSceneName()
        {
            return currentSceneName;
        }
    }
}
