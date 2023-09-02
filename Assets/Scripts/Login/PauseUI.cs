using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    private Button BtnContinue;
    private Button BtnReplay;
    private Button BtnReturnHomepage;
    //private Button BtnBackLevel;

    void Awake()
    {
        BtnContinue = transform.Find("UI/BtnContinue").GetComponent<Button>();
        BtnReplay = transform.Find("UI/BtnReplay").GetComponent<Button>();
        BtnReturnHomepage = transform.Find("UI/BtnReturnHomepage").GetComponent<Button>();
        //BtnBackLevel = transform.Find("UI/BtnBackLevel").GetComponent<Button>();

        MessageManager.AddListener(DefineManager.ShowPauseUI, Show);
    }

    void Start()
    {
        BtnContinue.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            GameManager.PlayClickAudio();
            Hide();
        });

        BtnReplay.onClick.AddListener(() =>
        {
            ResetParameter();
            GameManager.PlayClickAudio();
            MessageManager.TriggerEvent(DefineManager.ReStartGame);
            MessageManager.TriggerEvent(DefineManager.OpenGameScene);
        });

        BtnReturnHomepage.onClick.AddListener(() =>
        {
            ResetParameter();
            GameManager.PlayClickAudio();
            MessageManager.TriggerEvent(DefineManager.OpenLaunchScene);
        });

        //BtnBackLevel.onClick.AddListener(() =>
        //{
        //    Time.timeScale = 1;
        //    GameManager.PlayClickAudio();
        //    MessageManager.TriggerEvent(DefineManager.OpenLevelScene);
        //});
    }

    void ResetParameter()
    {
        Time.timeScale = 1;
        GameManager.CurrentScore = 0;
        GameManager.BlockList = new BlockItem[GameManager.fatory, GameManager.fatory];

    }

    private void OnDestroy()
    {
        MessageManager.RemoveListener(DefineManager.ShowPauseUI, Show);
    }

    void Show()
    {
        transform.localPosition = new Vector3(0, 0, 0);
    }

    void Hide()
    {
        transform.localPosition = new Vector3(1920, 1080, 0);
    }
}
