using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class OverUI : MonoBehaviour
{
    private Button BtnReplay;
    //private Button BtnBackLevel;
    private Button BtnReturnHomepage;
    private Text ShowText;
    private Text BestScore;
    private Text CurrentScore;

    void Awake()
    {
        BtnReplay = transform.Find("UI/BtnReplay").GetComponent<Button>();
        //BtnBackLevel = transform.Find("UI/BtnBackLevel").GetComponent<Button>();
        BtnReturnHomepage = transform.Find("UI/BtnReturnHomepage").GetComponent<Button>();
        ShowText = transform.Find("UI/ShowText").GetComponent<Text>();
        BestScore = transform.Find("UI/BestScore").GetComponent<Text>();
        CurrentScore = transform.Find("UI/CurrentScore").GetComponent<Text>();

        if (GameManager.isVictory && !GameManager.isFailure)
        {
            ShowText.text = "恭喜过关";

        }
        else if (GameManager.isFailure && !GameManager.isVictory)
        {
            ShowText.text = "游戏结束";
        }
        //AdManager.Instance.ShowAdInterstitial("I1");
    }

    void Start()
    {
        BestScore.text = GameManager.bestScore.ToString();
        CurrentScore.text = GameManager.CurrentScore.ToString();

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
        //    ResetParameter();
        //    GameManager.PlayClickAudio();
        //    MessageManager.TriggerEvent(DefineManager.OpenLevelScene);
        //});
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            transform.parent.transform.Find("ExitPanel").gameObject.SetActive(true);
        }
    }

    void ResetParameter()
    {
        GameManager.CurrentScore = 0;
        GameManager.isVictory = false;
        GameManager.isFailure = false;
        GameManager.BlockList = new BlockItem[GameManager.fatory, GameManager.fatory];
    }
}
