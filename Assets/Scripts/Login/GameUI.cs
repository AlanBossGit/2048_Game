using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    //暂停按钮
    private Button BtnPause;
    //重新开始按钮
    private Button BtnReStart;
    private Button HomeButton;

    //游戏当前分数文本
    private Text TextCurrentScore;
    //游戏最高分数文本
    private Text TextBestScore;
    //方块的父物体
    private Transform BlockContent;
    //方块的预设
    private GameObject BlockPre;
    //方块移动时的开始位置
    private Vector2 startPos;
    //方块移动时的结束位置
    private Vector2 endPos;
    //是否在滑动中
    private bool isDrag;

    void Awake()
    {
        //本地化存取最高分
        GameManager.bestScore = PlayerPrefs.GetInt("BestScore");
        //设置移动变量
        GameManager.isCanControll = true;

        BtnPause = transform.Find("UI/BtnPause").GetComponent<Button>();
        BtnReStart = transform.Find("UI/BtnReStart"). GetComponent<Button>();
        HomeButton = transform.Find("UI/BtnHome").GetComponent<Button>();
        TextCurrentScore = transform.Find("UI/Current/TextCurrentScore").GetComponent<Text>();
        TextBestScore = transform.Find("UI/Best/TextBestScore").GetComponent<Text>();
        BlockContent = transform.Find("UI/BlockContent");

        BlockPre = ResourcesContainers.GetResourcesContainers().blockItemPre;

        MessageManager.AddListener(DefineManager.SpawnBlock, SpawnBlock);
        MessageManager.AddListener(DefineManager.ReStartGame, RePlayGame);
        MessageManager.AddListener(DefineManager.UpdateGameScore, UpdateGameScore);

    }

    private void OnEnable()
    {
        //AdManager.Instance.ShowAdBanner("B1",GoogleMobileAds.Api.AdPosition.Bottom);
    }

    private void OnDestroy()
    {
        MessageManager.RemoveListener(DefineManager.SpawnBlock, SpawnBlock);
        MessageManager.RemoveListener(DefineManager.ReStartGame, RePlayGame);
        MessageManager.RemoveListener(DefineManager.UpdateGameScore, UpdateGameScore);
    }

    void Start()
    {
        TextBestScore.text = GameManager.bestScore.ToString();
        //初始化生成两个方块
        Instantiate(BlockPre, BlockContent);
        Instantiate(BlockPre, BlockContent);

        BtnPause.onClick.AddListener(() =>
        {
            Time.timeScale = 0;
            GameManager.PlayClickAudio();
            MessageManager.TriggerEvent(DefineManager.ShowPauseUI);
        });

        BtnReStart.onClick.AddListener(() =>
        {
            GameManager.CurrentScore = 0;
            GameManager.PlayClickAudio();
            RePlayGame();
        });

        HomeButton.onClick.AddListener(OnHomeButtonClick);


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            transform.parent.transform.Find("ExitPanel").gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            int bestScore = PlayerPrefs.GetInt("BestScore");
            bestScore = bestScore > GameManager.CurrentScore ? bestScore : GameManager.CurrentScore;
            PlayerPrefs.SetInt("BestScore", bestScore);
            GameManager.isFailure = true;
            print("失败");
            //打开失败场景
            GameManager.PlayGameOverAudio();
            MessageManager.TriggerEvent(DefineManager.OpenOverScene);
        }

        //如果已经移动过，或者正在移动数组个数为 0
        if (GameManager.hadMove && GameManager.MovingBlockList.Count == 0)
        {
            //触发消息生成方块
            MessageManager.TriggerEvent(DefineManager.SpawnBlock);
            GameManager.hadMove = false;

            //重置数组信息
            for (int i = 0; i < GameManager.fatory; i++)
            {
                for (int j = 0; j < GameManager.fatory; j++)
                {
                    if (GameManager.BlockList[i, j] != null)
                    {
                        GameManager.BlockList[i, j].hadCompound = false;
                    }
                }
            }
        }

        //没有方块在移动并且可移动变量为true
        if (GameManager.MovingBlockList.Count == 0 && GameManager.isCanControll)
        {
            int directionX = 0;
            int directionY = 0;

            //在Windows平台
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                #region 电脑键盘移动控制
                //向左
                if (Input.GetKeyDown(KeyCode.A))
                {
                    directionX = -1;
                    MoveBlock(directionX, directionY);
                }
                //向右
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    directionX = 1;
                    MoveBlock(directionX, directionY);
                }
                //向上
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    directionY = 1;
                    MoveBlock(directionX, directionY);
                }
                //向下
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    directionY = -1;
                    MoveBlock(directionX, directionY);
                }
                #endregion
            }
            //在安卓或者苹果平台
            else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                #region 手机移动控制
                if (Input.GetTouch(0).phase == TouchPhase.Began && !isDrag)
                {
                    startPos = Input.GetTouch(0).position;
                    isDrag = true;
                }

                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    isDrag = false;
                    endPos = Input.GetTouch(0).position;

                    float xDistance = Mathf.Abs(endPos.x - startPos.x);
                    float yDistance = Mathf.Abs(endPos.y - startPos.y);

                    //水平方向
                    if (xDistance > yDistance)
                    {
                        //向右
                        if (endPos.x - startPos.x > 0)
                        {
                            directionX = 1;
                            MoveBlock(directionX, directionY);
                        }
                        //向左
                        else if (endPos.x - startPos.x < 0)
                        {
                            directionX = -1;
                            MoveBlock(directionX, directionY);
                        }
                    }
                    else if (yDistance > xDistance)
                    {
                        //向上
                        if (endPos.y - startPos.y > 0)
                        {
                            directionY = 1;
                            MoveBlock(directionX, directionY);
                        }
                        //向下
                        else if (endPos.y - startPos.y < 0)
                        {
                            directionY = -1;
                            MoveBlock(directionX, directionY);
                        }
                    }
                }
                #endregion
            }
        }
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void RePlayGame()
    {
        //遍历销毁游戏对象，并置空方块数组
        for (int i = 0; i < GameManager.fatory; i++)
        {
            for (int j = 0; j < GameManager.fatory; j++)
            {
                if (GameManager.BlockList[i, j] != null)
                {
                    Destroy(GameManager.BlockList[i, j].gameObject);
                    GameManager.BlockList[i, j] = null;
                }
            }
        }
        //生成两个方块
        Instantiate(BlockPre, BlockContent);
        Instantiate(BlockPre, BlockContent);
    }

    /// <summary>
    /// 移动方块
    /// </summary>
    /// <param name="directionX"></param>
    /// <param name="directionY"></param>
    void MoveBlock(int directionX, int directionY)
    {
        //向右移动
        if (directionX == 1)
        {
            //遍历非空方块，调用方块的移动方法
            for (int y = 0; y < GameManager.fatory; y++)
            {
                for (int x = GameManager.fatory - 2; x >= 0; x--)
                {
                    if (GameManager.BlockList[x, y] != null)
                    {
                        GameManager.BlockList[x, y].Move(directionX, directionY);
                    }
                }
            }
        }
        //向左移动
        else if (directionX == -1)
        {
            for (int y = 0; y < GameManager.fatory; y++)
            {
                for (int x = 1; x < GameManager.fatory; x++)
                {
                    if (GameManager.BlockList[x, y] != null)
                    {
                        GameManager.BlockList[x, y].Move(directionX, directionY);
                    }
                }
            }
        }
        //向上移动
        else if (directionY == 1)
        {
            for (int x = 0; x < GameManager.fatory; x++)
            {
                for (int y = GameManager.fatory - 2; y >= 0; y--)
                {
                    if (GameManager.BlockList[x, y] != null)
                    {
                        GameManager.BlockList[x, y].Move(directionX, directionY);
                    }
                }
            }
        }
        //向下移动
        else if (directionY == -1)
        {
            for (int x = 0; x < GameManager.fatory; x++)
            {
                for (int y = 1; y < GameManager.fatory; y++)
                {
                    if (GameManager.BlockList[x, y] != null)
                    {
                        GameManager.BlockList[x, y].Move(directionX, directionY);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 生成方块
    /// </summary>
    void SpawnBlock()
    {
        Instantiate(BlockPre, BlockContent);
    }

    /// <summary>
    /// 更新分数显示
    /// </summary>
    void UpdateGameScore()
    {
        TextCurrentScore.text = GameManager.CurrentScore.ToString();
    }



    public void OnHomeButtonClick()
    {
        SceneManager.LoadScene("Launch");
        ///AdManager.Instance.HideAdBanner();
    }

}
