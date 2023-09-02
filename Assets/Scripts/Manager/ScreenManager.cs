using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 屏幕管理器，用于UI适配
/// </summary>
public class ScreenManager : MonoBehaviour {
    //摄像机组件
    [SerializeField]
    private Camera cam;
    //Canvas上的CanvasScaler组件
    private CanvasScaler cs;

    //倍数
    private float scaleSize = 1;

    //手机宽高
    private float width = 1080;
    private float height = 1920;

    private void Awake()
    {
        //获取组件
        cs = GetComponent<CanvasScaler>();

        //调节屏幕适配
        if (Screen.width * 1f / Screen.height > width / height)
        {
            //目前宽高比例比手机宽高比例大
            cs.matchWidthOrHeight = 1;
            cam.orthographicSize = height * scaleSize / 2 / 100;
        }
        else
        {
            //目前宽高比例比手机宽高比例小
            cs.matchWidthOrHeight = 0;
            float num = Screen.width * 1f / Screen.height;
            cam.orthographicSize = (width / num) * scaleSize / 2 / 100;
        }
    }
}
