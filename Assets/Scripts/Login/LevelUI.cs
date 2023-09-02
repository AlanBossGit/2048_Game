using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    //全部解锁按钮
    private Button BtnUnLock;
    //关卡实例化出来时挂载的父物体
    private Transform Content;
    //关卡预设
    private GameObject LevelItemPre;

    void Awake()
    {
        //查找组件
        Init();
        //通过资源容器加载关卡预设
        LevelItemPre = ResourcesContainers.GetResourcesContainers().levelItemPre;
        //LevelItemPre = Resources.Load<GameObject>("LevelItem");
    }

    void Init()
    {
        BtnUnLock = transform.Find("UI/BtnUnLock").GetComponent<Button>();
        Content = transform.Find("UI/Scroll View/Viewport/Content");
    }

    void Start()
    {
        BtnUnLock.onClick.AddListener(()=>
        {
            MessageManager.TriggerEvent(DefineManager.PlayOtherAudio, DefineManager.ClickAudio);

            //遍历关卡信息链表，依次修改所有关卡为已解锁
            for (int i = 0; i < LevelManager.Instance.levelList.levelData.Count; i++)
            {
                LevelManager.Instance.ChangeLevelDataById(i, true);
            }

            for (int i = 0; i < Content.childCount; i++)
            {
                //根据关卡信息文件中的关卡解锁情况，解锁对应关卡，按钮灰态或者高亮，锁头图片显示或者隐藏
                if (LevelManager.Instance.levelList.levelData[i].isUnlock)
                {
                    Content.GetChild(i).GetComponent<LevelItem>().BtnLevel.interactable = true;
                    Content.GetChild(i).GetComponent<LevelItem>().TextLevel.enabled = true;
                    Content.GetChild(i).GetComponent<LevelItem>().Lock.gameObject.SetActive(false);
                }
                else
                {
                    Content.GetChild(i).GetComponent<LevelItem>().BtnLevel.interactable = false;
                    Content.GetChild(i).GetComponent<LevelItem>().TextLevel.enabled = false;
                    Content.GetChild(i).GetComponent<LevelItem>().Lock.gameObject.SetActive(true);
                }
            }
        });

        //实例化出关卡item
        InitLevelList();
    }
    
    /// <summary>
    /// 实例化关卡item
    /// </summary>
    void InitLevelList()
    {
        //遍历关卡管理器的关卡信息链表，依次实例化出关卡item
        for (int i = 0; i < LevelManager.Instance.levelList.levelData.Count; i++)
        {
            //实例化出关卡item，并挂载到滑动列表的Content下
            GameObject LevelItemGo = Instantiate(LevelItemPre, Content);
            //给每个item的下标index赋值
            LevelItemGo.GetComponent<LevelItem>().index = i;
            //给每个item的文本显示Text赋值
            LevelItemGo.GetComponent<LevelItem>().TextLevel.text = (i + 1).ToString();

            //根据关卡信息文件中的关卡解锁情况，解锁对应关卡，按钮灰态或者高亮，锁头图片显示或者隐藏
            if (LevelManager.Instance.levelList.levelData[i].isUnlock)
            {
                LevelItemGo.GetComponent<LevelItem>().BtnLevel.interactable = true;
                LevelItemGo.GetComponent<LevelItem>().TextLevel.enabled = true;
                LevelItemGo.GetComponent<LevelItem>().Lock.gameObject.SetActive(false);
            }
            else
            {
                LevelItemGo.GetComponent<LevelItem>().BtnLevel.interactable = false;
                LevelItemGo.GetComponent<LevelItem>().TextLevel.enabled = false;
                LevelItemGo.GetComponent<LevelItem>().Lock.gameObject.SetActive(true);
            }
        }
    }
}
