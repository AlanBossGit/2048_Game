using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 事件管理器，封装鼠标各种事件
/// </summary>
public class EventManager : EventTrigger
{
    #region 重写鼠标事件
    //声明鼠标事件委托
    public Action<PointerEventData> onClick;
    public Action<PointerEventData> onDown;
    public Action<PointerEventData> onUp;
    public Action<PointerEventData> onEnter;
    public Action<PointerEventData> onExit;
    public Action<PointerEventData> onDrag;
    public Action<PointerEventData> onBeginDrag;
    public Action<PointerEventData> onEndDrag;

    //重写点击响应事件
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
        {
            onClick(eventData);
        }
    }
    //重写按下响应事件
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null)
        {
            onDown(eventData);
        }
    }
    //重写抬起响应事件
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null)
        {
            onUp(eventData);
        }
    }
    //重写进入响应事件
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null)
        {
            onEnter(eventData);
        }
    }
    //重写退出响应事件
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null)
        {
            onExit(eventData);
        }
    }
    //重写拖拽响应事件
    public override void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null)
        {
            onDrag(eventData);
        }
    }
    //重写开始拖拽响应事件
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (onBeginDrag != null)
        {
            onBeginDrag(eventData);
        }
    }
    //重写结束拖拽响应事件
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null)
        {
            onEndDrag(eventData);
        }
    }
    #endregion

    //为物体添加事件监听方法
    public static EventManager Get(GameObject go)
    {
        if (go.GetComponent<EventManager>() == null)
        {
            return go.AddComponent<EventManager>();
        }
        else
        {
            return go.GetComponent<EventManager>();
        }
    }

    //扩展Get方法，Transform、MonoBehaviour
    public static EventManager Get(Transform trans)
    {
        return Get(trans.gameObject);
    }

    public static EventManager Get(MonoBehaviour mono)
    {
        return Get(mono.gameObject);
    }
}
