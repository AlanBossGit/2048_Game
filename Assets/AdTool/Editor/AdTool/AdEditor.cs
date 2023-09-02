using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace Ad.Tool
{
    public class AdEditor : Editor
    {
        //目标路径：
        private static string targetPath = Application.streamingAssetsPath + "/AdIDText/";
        //平台：
        private static string[] arrPlatform = { "iOS", "Android" };

        [MenuItem("Tools/广告工具/1-广告位初始化")]
        private static void AdInit()
        {
            if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
            string tipContent = "";
            string tempStr = "";
            //遍历平台，生成对于文件
            for (int i = 0; i < arrPlatform.Length; i++)
            {
                string newPath = targetPath + arrPlatform[i] + "AdID.json";
                if (File.Exists(newPath))
                {
                    tempStr = "默认广告位文件已存在\n";
                    tipContent += arrPlatform[i] + "平台:" + newPath + "\n";
                    continue;
                }
                else
                {
                    tempStr = "默认广告位文件生成路径为：\n";
                    tipContent += arrPlatform[i] + "平台:" + newPath + "\n";
                }
                //创建该文件
                File.WriteAllText(newPath, CreateText(arrPlatform[i]));
            }
            EditorUtility.DisplayDialog("温馨提示:", tempStr + tipContent, "已阅读");
        }

        [MenuItem("Tools/广告工具/2-文件路径提示")]
        private static void TipMenu()
        {
            string tipContent = "";
            for (int i = 0; i < arrPlatform.Length; i++)
            {
                string newPath = targetPath + arrPlatform[i] + "AdID.json";
                tipContent += arrPlatform[i] + "平台:" + newPath + "\n";
            }
            EditorUtility.DisplayDialog("温馨提示:", "默认广告位文件生成路径为：" + tipContent, "已阅读");
        }

        /// <summary>
        /// 创建广告位文本
        /// </summary>
        private static string CreateText(string pf)
        {
            string content = "{\n";
            string testKey = "Test";
            string deviceID = "";
            string bannerID = "";
            string rewardID = "";
            string InterstitialID = "";
            string RewardedInterstitialID = "";
            switch (pf)
            {
                case "iOS":
                    deviceID = "96e23e80653bb28980d3f40beb58915c";
                    bannerID = "ca-app-pub-3940256099942544/2934735716";
                    rewardID = "ca-app-pub-3940256099942544/1712485313";
                    InterstitialID = "ca-app-pub-3940256099942544/4411468910";
                    RewardedInterstitialID = "ca-app-pub-3940256099942544/6978759866";
                    break;
                case "Android":
                    deviceID = "75EF8D155528C04DACBBA6F36F433035";
                    bannerID = "ca-app-pub-3940256099942544/6300978111";
                    rewardID = "ca-app-pub-3940256099942544/5224354917";
                    InterstitialID = "ca-app-pub-3940256099942544/1033173712";
                    RewardedInterstitialID = "ca-app-pub-3940256099942544/5354046379";
                    break;
            }
            string deveceContent = "\"DeviceID\":\""+deviceID+"\",\n";
            string bannerContent = "\"Banner\":\n[\n[\"" + testKey + "\",\"" + bannerID + "\"]\n],\n";
            string rewardContent = "\"Reward\":\n[\n[\"" + testKey + "\",\"" + rewardID + "\"]\n],\n";
            string InterstitialContent = "\"Interstitial\":\n[\n[\"" + testKey + "\",\"" + InterstitialID + "\"]\n],\n";
            string RewardedInterstitialContent = "\"RewardedInterstitial\":\n[\n[\"" + testKey + "\",\"" + RewardedInterstitialID + "\"]\n]\n";
            content += deveceContent+ bannerContent + rewardContent + InterstitialContent + RewardedInterstitialContent + "}";
            return content;
        }
    }
}
