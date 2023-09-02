using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 关卡数据类
/// </summary>
[Serializable]                  //可序列化的，加上这个才可以写入到json文件中
public class LevelModel
{
    //关卡ID
    public int levelID;
    //是否解锁关卡
    public bool isUnlock;
}

/// <summary>
/// 关卡信息链表类
/// </summary>
[Serializable]
public class LevelList
{
    //关卡信息链表，存储关卡信息
    public List<LevelModel> levelData = new List<LevelModel>();
}