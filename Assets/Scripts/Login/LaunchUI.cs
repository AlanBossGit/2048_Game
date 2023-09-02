using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LaunchUI : MonoBehaviour
{
    private Image BG;
    private Image board;
    private Button BtnStart;
    private Button BtnRank;
    private Button numStartButton;
    void Awake()
    {
        //AdManager.Instance.ShowAdInterstitial("I1",(state)=> {

        //    switch (state)
        //    {
        //        case AdState.AdSucceed:
        //        case AdState.AdDefeated:
        //        case AdState.AdExit:
        //            AdManager.Instance.ShowAdBanner("B1", GoogleMobileAds.Api.AdPosition.Bottom);
        //            break;
        //    }     
        //});
        //查找组件
        Init();
        //设置关卡数量
        GameManager.levelNum = 3;
        //设置方块格子行列数
        GameManager.fatory = 4;
        //生命方块数组长度
        GameManager.BlockList = new BlockItem[GameManager.fatory, GameManager.fatory];
        //赋值方块图片数组
        GameManager.BlockSpriteList = ResourcesContainers.GetResourcesContainers().BlockSpriteList;
        GameManager.NumSpriteList = ResourcesContainers.GetResourcesContainers().NumSpriteList;
        //实例化关卡列表，生成Json文件
        LevelManager.Instance.LoadLevelList();
        //加载streamingAssets路径下的开始封面
        InitStartBg();
        InitBoardSprite();
        //声明场景管理器对象
        ScenesManager sm = ScenesManager.Instance;
        
    }

    void Init()
    {
        BG = transform.Find("UI/BG").GetComponent<Image>();
        board = transform.Find("UI/Board").GetComponent<Image>();
        BtnStart = transform.Find("UI/BtnStart").GetComponent<Button>();
        numStartButton = transform.Find("UI/NumBtnStart").GetComponent<Button>();
        BtnRank = transform.Find("UI/BtnRank").GetComponent<Button>();
    }

    void Start()
    {
        //开始游戏按钮点击触发音效，并加载选关场景
        BtnStart.onClick.AddListener(() =>
        {
            GameManager.PlayClickAudio();
            //MessageManager.TriggerEvent(DefineManager.OpenLevelScene);
            MessageManager.TriggerEvent(DefineManager.OpenGameScene);
            //AdManager.Instance.HideAdBanner();
            PlayInfo.Instance.SetLevelType(LevelType.SHuZI);
        });

        numStartButton.onClick.AddListener(()=> {
            GameManager.PlayClickAudio();
            //MessageManager.TriggerEvent(DefineManager.OpenLevelScene);
            MessageManager.TriggerEvent(DefineManager.OpenGameScene);
            //AdManager.Instance.HideAdBanner();
            PlayInfo.Instance.SetLevelType(LevelType.SHuiGuo);
        });

        //排行榜按钮点击
        BtnRank.onClick.AddListener(() =>
        {
            GameManager.PlayClickAudio();
            //ToDo  排行榜
        });

        //播放背景音乐
        AudioManager.Instance.PlayBGMAudio();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            transform.parent.transform.Find("ExitPanel").gameObject.SetActive(true);
        }
    }

    private void InitStartBg()
    {
        string url = Application.streamingAssetsPath + DefineManager.BackGround;

        Framework.LoadManager.GetImage(url, (sp) =>
        {
           // BG.sprite = sp;
        });
    }

    private void InitBoardSprite()
    {
        string url = Application.streamingAssetsPath + DefineManager.BoardSprite;

        Framework.LoadManager.GetImage(url, (sp) =>
        {
            board.sprite = sp;
            board.SetNativeSize();
        });
    }
}
