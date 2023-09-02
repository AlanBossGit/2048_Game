using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager _instance;

    public Transform LaunchUI;
    public Transform GameUI;
    public Transform LevelUI;
    public Transform PauseUI;

    void Awake()
    {
        _instance = this;
        //LaunchUI = transform.Find("LaunchUI");
        GameUI = transform.Find("GamePanel");
        //LevelUI = transform.Find("LevelUI");
        PauseUI = transform.Find("PausePanel");

    }

    void Start()
    {
        //LaunchUI.transform.localPosition = new Vector3(0, 0, 0);
        GameUI.transform.localPosition = new Vector3(0, 0, 0);
        //LevelUI.transform.localPosition = new Vector3(1920, 1080, 0);
        PauseUI.transform.localPosition = new Vector3(1920, 1080, 0);
    }
}
