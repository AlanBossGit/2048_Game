using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 消息管理器，观察者模式的应用
/// </summary>
public class MessageManager
{
    //消息存放字典，字符串为键，委托为值
    private static Dictionary<string, Delegate> _router = new Dictionary<string, Delegate>();

    public Dictionary<string, Delegate> Router
    {
        get { return _router; }
    }

    /// <summary>
    /// 添加监听方法
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="handle"></param>
    public static void AddListener(string eventName, Action handle)
    {
        if (!_router.ContainsKey(eventName))
        {
            _router.Add(eventName, null);
        }

        Delegate del = _router[eventName];

        if (del != null && del.GetType() != handle.GetType())
        {
            throw new Exception("类型不一致");
        }

        _router[eventName] = (Action)_router[eventName] + handle;
    }

    public static void AddListener<T>(string eventName, Action<T> handle)
    {
        if (!_router.ContainsKey(eventName))
        {
            _router.Add(eventName, null);
        }

        Delegate del = _router[eventName];

        if (del != null && del.GetType() != handle.GetType())
        {
            throw new Exception("类型不一致");
        }

        _router[eventName] = (Action<T>)_router[eventName] + handle;
    }

    /// <summary>
    /// 移除监听方法
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="handle"></param>
    public static void RemoveListener(string eventName, Action handle)
    {
        if (_router.ContainsKey(eventName))
        {
            Delegate del = _router[eventName];

            if (del != null && del.GetType() != handle.GetType())
            {
                throw new Exception("类型不一致");
            }

            _router[eventName] = (Action)_router[eventName] - handle;

            if (_router.ContainsKey(eventName) && _router[eventName] == null)
            {
                _router.Remove(eventName);
            }
        }
    }

    public static void RemoveListener<T>(string eventName, Action<T> handle)
    {
        if (_router.ContainsKey(eventName))
        {
            Delegate del = _router[eventName];

            if (del != null && del.GetType() != handle.GetType())
            {
                throw new Exception("类型不一致");
            }

            _router[eventName] = (Action<T>)_router[eventName] - handle;

            if (_router.ContainsKey(eventName) && _router[eventName] == null)
            {
                _router.Remove(eventName);
            }
        }
    }
    /// <summary>
    /// 触发监听方法
    /// </summary>
    /// <param name="eventName"></param>
    public static void TriggerEvent(string eventName)
    {
        Delegate del;

        if (!_router.TryGetValue(eventName, out del))
        {
            return;
        }

        var callbacks = del.GetInvocationList();

        for (int i = 0; i < callbacks.Length; i++)
        {
            Action callback = callbacks[i] as Action;

            if (callback == null)
            {
                throw new Exception("触发不到");
            }

            try
            {
                callback();
            }
            catch
            {

            }
        }
    }

    public static void TriggerEvent<T>(string eventName, T arg)
    {
        Delegate del;

        if (!_router.TryGetValue(eventName, out del))
        {
            return;
        }

        var callbacks = del.GetInvocationList();

        for (int i = 0; i < callbacks.Length; i++)
        {
            Action<T> callback = callbacks[i] as Action<T>;

            if (callback == null)
            {
                throw new Exception("触发不到");
            }

            try
            {
                callback(arg);
            }
            catch
            {

            }
        }
    }
}
