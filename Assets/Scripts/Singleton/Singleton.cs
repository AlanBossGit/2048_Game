using UnityEngine;
using System.Collections;

namespace Framework
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        //单例对象
        private static T _instance;

        public static T Instance
        {
            get
            {
                //如果单例对象为空则新建，并进行实例
                if (_instance == null)
                {
                    _instance = new T();
                    _instance.Init();
                }
                return _instance;
            }
        }

        protected Singleton()
        {

        }

        protected virtual void Init()
        {

        }
    }
}

