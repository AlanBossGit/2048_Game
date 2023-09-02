using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LevelManager : SingletonMono<LevelManager>
{
    //关卡数目
    public int levelNum;
    //关卡信息列表
    public LevelList levelList;
    
    /// <summary>
    /// 保存关卡信息json文件
    /// </summary>
    public void SaveLevelList()
    {
        GameManager.SaveObjectToJsonFile(levelList, "LevelListText.Json");
    }

    /// <summary>
    /// 加载关卡信息json文件
    /// </summary>
    public void LoadLevelList()
    {
        levelList = GameManager.LoadObjectFormJsonFife<LevelList>("LevelListText.Json");

        //关卡信息链表为空时添加默认关卡信息
        if (levelList == null)
        {
            //实例化关卡json对象
            levelList = new LevelList();
            //为json文件添加默认关卡
            AddDefaultLevel();
        }
    }

    /// <summary>
    /// 添加默认关卡
    /// </summary>
    public void AddDefaultLevel()
    {
        for (int i = 0; i < GameManager.levelNum; i++)
        {
            //实例化关卡数据对象
            LevelModel level = new LevelModel();
            //为关卡数据赋值，ID 和 解锁情况
            level.levelID = i;
            level.isUnlock = false;                 

            //第一关默认解锁
            if (i == 0)
            {
                level.isUnlock = true;
            }

            //添加到关卡信息列表
            levelList.levelData.Add(level);
            //保存到关卡信息文件json中
            SaveLevelList();                       
        }
        Debug.Log("关卡json数量为：" + levelList.levelData.Count);
    }

    /// <summary>
    /// 得到所有的关卡信息
    /// </summary>
    /// <returns></returns>
    public List<LevelModel> GetAllLevelListData()
    {
        return levelList.levelData;
    }

    /// <summary>
    /// 根据id获取关卡信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public LevelModel GetLevelDataById(int id)
    {
        return levelList.levelData[id];
    }

    /// <summary>
    /// 根据id改变关卡信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isEasyPass"></param>
    /// <param name="isNormalPass"></param>
    /// <param name="isHardPass"></param>
    /// <param name="isUnlock"></param>
    public void ChangeLevelDataById(int id, bool isUnlock)
    {
        //遍历关卡信息链表，找到与传进来的id相同的关卡ID，修改解锁情况，最后保存
        for (int i = 0; i < levelList.levelData.Count; i++)
        {
            if (levelList.levelData[i].levelID == id)
            {
                levelList.levelData[i].isUnlock = isUnlock;
            }
        }
        SaveLevelList();
    }
}
