using UnityEngine;
using System.Collections;

namespace Framework
{
    public class AudioManager : SingletonMono<AudioManager>
    {
        //背景声音
        private AudioSource audioSource;
        [SerializeField]
        private AudioClip audioClip;

        //其他音效
        private AudioSource otherSource;
        [SerializeField]
        private AudioClip[] otherClips;
        
        private void Awake()
        {
            CreateAudioSource();
            InitOtherSource();
            MessageManager.AddListener<string>(DefineManager.PlayOtherAudio, PlayOtherAudio);
            MessageManager.AddListener<AudioClip>(DefineManager.PlayAudio,PlayAudio);
        }

        private void OnDestroy()
        {
            MessageManager.RemoveListener<string>(DefineManager.PlayOtherAudio, PlayOtherAudio);
            MessageManager.RemoveListener<AudioClip>(DefineManager.PlayAudio, PlayAudio);
        }

        /// <summary>
        /// 创建AudioSource组件对象
        /// </summary>
        private void CreateAudioSource()
        {
            //生成三个GameObject，设置他们之间的层级关系
            GameObject go = new GameObject("Audio");
            go.transform.SetParent(transform);

            GameObject backgroundGo = new GameObject("AudioSource");
            GameObject otherGo = new GameObject("OtherSource");

            backgroundGo.transform.SetParent(go.transform);
            otherGo.transform.SetParent(go.transform);

            //添加AudioSource组件
            if (backgroundGo.GetComponent<AudioSource>() == null)
            {
                audioSource = backgroundGo.AddComponent<AudioSource>();
            }
            else
            {
                audioSource = backgroundGo.GetComponent<AudioSource>();
            }

            if (otherGo.GetComponent<AudioSource>() == null)
            {
                otherSource = otherGo.AddComponent<AudioSource>();
            }
            else
            {
                otherSource = otherGo.GetComponent<AudioSource>();
            }
        }

        /// <summary>
        /// 初始化音效资源
        /// </summary>
        private void InitOtherSource()
        {
            audioSource.playOnAwake = false;
            otherSource.playOnAwake = false;
            //加载音效片段资源
            otherClips = Resources.LoadAll<AudioClip>("Audio");
        }

        /// <summary>
        /// 播放背景音乐的方法
        /// </summary>
        public void PlayBGMAudio()
        {
            string URL = Application.streamingAssetsPath + DefineManager.BGM;
            LoadManager.GetAudio(URL, (ac) =>
            {
                audioSource.clip = ac;
                audioSource.loop = true;
                audioSource.Play();
            });
        }

        /// <summary>
        /// 停止播放背景音乐的方法
        /// </summary>
        public void StopBGMAudio()
        {
            audioSource.Stop();
        }

        /// <summary>
        /// 播放其他音效的方法
        /// </summary>
        /// <param name="audioName"></param>
        public void PlayOtherAudio(string audioName)
        {
            if (otherClips != null && otherClips.Length > 0)
            {
                //遍历所有的otherClips
                //如果发现名字是要播放的audioName，就播放该片段
                foreach (var clip in otherClips)
                {
                    if (clip.name == audioName)
                    {
                        otherSource.clip = clip;
                        otherSource.Play();
                        return;
                    }
                }
                Debug.LogWarning("查找不到PlayOtherAudio的音效芯片，音效名字是: >>> " + audioName);
            }
            else
            {
                Debug.LogError("其他音效对象PlayOtherAudio为空");
            }
        }

        public void PlayAudio(AudioClip audioClip)
        {
            otherSource.clip = audioClip;
            otherSource.Play();
        }
    }
}

