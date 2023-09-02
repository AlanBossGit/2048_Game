using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : Component
{
    private static GameObject go;
    private static string goName = "UnitySingletonObj";
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                //如果单例物体对象为空，进行查找
                if (go == null)
                {
                    go = GameObject.Find(goName);
                    //如果查找不到物体，新创建一个空物体
                    if (go == null)
                    {
                        go = new GameObject(goName);
                        DontDestroyOnLoad(go);
                    }
                }
                //当物体不为空时，添加单例脚本
                if (go != null)
                {
                    _instance = go.AddComponent<T>();
                }
                else
                {
                    Debug.LogError("UnitySingletonObj对象出现错误：>>>>>>" + typeof(T).Name);
                }
            }
            return _instance;
        }
    }
}
