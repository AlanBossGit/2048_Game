using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    public class LoaderItem
    {
        public string Path;

        public UnityAction<WWW> OnSuccess;

        public UnityAction<WWW> OnFail;

        public WWW Result;

        //加载次数
        public int LoadCount = 0;

        //执行回调
        public void Callback()
        {
            if (Result != null)
            {
                if (!string.IsNullOrEmpty(Result.error))
                {
                    if (OnFail != null)
                    {
                        OnFail.Invoke(Result);
                    }
                }
                else
                {
                    if (OnSuccess != null)
                    {
                        OnSuccess.Invoke(Result);
                    }
                }
            }
        }
    }

    public class LoadManager : MonoBehaviour
    {
        private static LoadManager instance;
        static object _lock = new object();
        static string _name = "LoadManager";

        /// <summary>
        /// 等待加载
        /// </summary>
        private Dictionary<string, LoaderItem> WaitList;

        /// <summary>
        /// 已加载
        /// </summary>
        private Dictionary<string, LoaderItem> LoadedList;

        protected string[] WaitKeys = { };

        public LoadManager()
        {
            WaitList = new Dictionary<string, LoaderItem>();
            LoadedList = new Dictionary<string, LoaderItem>();
        }

        public static LoadManager Instance
        {
            get
            {
                lock (_lock)
                {
                    GameObject obj = GameObject.Find(_name);
                    if (obj == null)
                    {
                        obj = new GameObject(_name);
                        DontDestroyOnLoad(obj);
                        instance = obj.AddComponent<LoadManager>();
                    }
                    else
                    {
                        instance = obj.GetComponent<LoadManager>();
                    }
                }

                return instance;
            }
        }

        public void Awake()
        {
            StartCoroutine(RunTask());
        }

        private string FormatPath(string path, bool isStreamingAssets = true)
        {
#if UNITY_EDITOR||UNITY_IOS
            if (!path.StartsWith("file://", System.StringComparison.Ordinal))
            {
                path = "file://" + path;
            }
#endif

            return path;
        }

        /// <summary>
        /// 加载声音资源
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="onSuccess">Success.</param>
        /// <param name="onFail">Fail.</param>
        public static void GetAudio(string path, UnityAction<AudioClip> onSuccess, UnityAction<WWW> onFail = null)
        {
            GetAsset(Instance.FormatPath(path), (www) =>
            {
                onSuccess.Invoke(www.GetAudioClip());
            }, onFail);
        }

        /// <summary>
        /// 加载图片资源
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="onSuccess">Success.</param>
        /// <param name="onFail">Fail.</param>
        public static void GetImage(string path, UnityAction<Sprite> onSuccess, UnityAction<WWW> onFail = null)
        {
            GetAsset(Instance.FormatPath(path), (www) =>
            {
                Sprite sp = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
                onSuccess.Invoke(sp);
            }, onFail);
        }

        /// <summary>
        /// 加载StreamingAsset资源
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="onSuccess">Success.</param>
        /// <param name="onFail">Fail.</param>
        public static void GetAsset(string path, UnityAction<WWW> onSuccess, UnityAction<WWW> onFail = null)
        {
            LoaderItem item = Instance.GetItem(path);

            if (item != null)
            {
                item.OnSuccess = onSuccess;
                item.OnFail = onFail;
                item.Callback();
            }
            else
            {
                item = new LoaderItem
                {
                    Path = path,
                    OnSuccess = onSuccess,
                    OnFail = onFail
                };
                Instance.WaitList.Add(path, item);
            }
        }

        /// <summary>
        /// 是否已加载过
        /// </summary>
        /// <returns><c>true</c>, if exists was ised, <c>false</c> otherwise.</returns>
        /// <param name="path">Path.</param>
        public bool IsExists(string path)
        {
            return LoadedList.ContainsKey(path);
        }

        /// <summary>
        /// 获取加载过的资源
        /// </summary>
        /// <returns>The item.</returns>
        /// <param name="path">Path.</param>
        public LoaderItem GetItem(string path)
        {
            if (IsExists(path))
            {
                return LoadedList[path];
            }

            return null;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <returns>The task.</returns>
        private IEnumerator RunTask()
        {
            int loadCount;//已加载的计数

            while (true)
            {
                loadCount = 0;

                yield return new WaitUntil(() => WaitList.Count > 0);

                foreach (LoaderItem item in WaitList.Values)
                {
                    if (item.LoadCount > 0)
                    {
                        loadCount++;

                        //全部执行完
                        if (WaitList.Count == loadCount)
                        {
                            foreach (KeyValuePair<string, LoaderItem> kv in WaitList)
                            {
                                LoadedList.Add(kv.Key, kv.Value);
                            }
                            WaitList.Clear();
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    item.Result = new WWW(item.Path);
                    yield return LoadUntil(item.Result);
                    item.LoadCount++;
                    item.Callback();
                }
            }
        }

        private IEnumerator LoadUntil(WWW w)
        {
            while (!w.isDone) yield return w;
        }
    }
}
