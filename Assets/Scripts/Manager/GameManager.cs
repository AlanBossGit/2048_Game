using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class GameManager
{
    
    public static bool isVictory;
    public static bool isFailure;
    public static int levelNum;
    public static int levelIndex;
    private static int currentScore;
    public static int bestScore;
    public static bool isCanControll;
    public static int fatory;
    public static bool hadMove = false;
    public static List<BlockItem> MovingBlockList = new List<BlockItem>();
    public static List<Sprite> BlockSpriteList = new List<Sprite>();
    public static List<Sprite> NumSpriteList = new List<Sprite>();
    public static BlockItem[,] BlockList;

    public static int CurrentScore
    {
        get => currentScore;
        set
        {
            currentScore = value;
            MessageManager.TriggerEvent(DefineManager.UpdateGameScore);
        }
    }

    public static bool isEmpty(int x, int y)
    {
        if (x < 0 || x >= 4 || y < 0 || y >= 4)
        {
            return false;
        }
        else if (BlockList[x, y] != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static bool GameFailure()
    {
        for (int i = 0; i < GameManager.fatory; i++)
        {
            for (int j = 0; j < GameManager.fatory; j++)
            {
                if (GameManager.BlockList[i, j] == null)
                {
                    return false;
                }
            }
        }

        for (int x = 0; x < GameManager.fatory; x++)
        {
            for (int y = 0; y < GameManager.fatory - 1; y++)
            {
                if (GameManager.BlockList[x, y].power != 11 && GameManager.BlockList[x, y].power == GameManager.BlockList[x, y + 1].power)
                {
                    return false;
                }
            }
        }

        for (int y = 0; y < GameManager.fatory; y++)
        {
            for (int x = 0; x < GameManager.fatory - 1; x++)
            {
                if (GameManager.BlockList[x, y].power != 11 && GameManager.BlockList[x, y].power == GameManager.BlockList[x + 1, y].power)
                {
                    return false;
                }
            }
        }
        return true;
    }
    #region Json文件信息的存取方法
    /// <summary>
    /// 从指定路径获取文件，再转换成文本形式，自动存入一个对象中
    /// </summary>
    /// <typeparam name="T">列表对象，对应数据类链表</typeparam>
    /// <param name="fileName">文件名，路径</param>
    /// <returns></returns>
    public static T LoadObjectFormJsonFife<T>(string fileName)
    {
        //设置路径
        string path = Application.persistentDataPath + "/" + fileName;
        Debug.Log("Json路径：" + path);

        //如果路径不存在
        if (!File.Exists(path))
        {
            return default(T);
        }
        //从文件中获取内容
        string strJson = File.ReadAllText(path);

        //如果文件内容不存在
        if (String.IsNullOrEmpty(strJson))
        {
            return default(T);
        }

        //根据文件内容给对象的属性赋值
        T obj = JsonUtility.FromJson<T>(strJson);
        //返回对象
        return obj;
    }

    /// <summary>
    /// 保存对象信息，先装换成Json格式，在保存到制定路径的文件去
    /// </summary>
    /// <param name="obj">列表对象</param>
    /// <param name="fileName">文件名</param>
    public static void SaveObjectToJsonFile(object obj, string fileName)
    {
        //把对象转换成Json格式
        string strJson = JsonUtility.ToJson(obj);
        //设置路径
        string path = Application.persistentDataPath + "/" + fileName;
        //把Json数据写到指定路径的文件去
        File.WriteAllText(path, strJson, Encoding.UTF8);
    }
    #endregion

    public static void PlayClickAudio()
    {
        //MessageManager.TriggerEvent(DefineManager.PlayOtherAudio, DefineManager.ClickAudio);
        MessageManager.TriggerEvent(DefineManager.PlayAudio, ResourcesContainers.GetResourcesContainers().clickClip);
    }

    public static void PlayGameOverAudio()
    {
        //MessageManager.TriggerEvent(DefineManager.PlayOtherAudio, DefineManager.ClickAudio);
        MessageManager.TriggerEvent(DefineManager.PlayAudio, ResourcesContainers.GetResourcesContainers().failureClip);
    }
}
