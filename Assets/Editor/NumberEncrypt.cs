using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;

public class NumberEncrypt
{
    /// <summary>
    /// 文件名
    /// </summary>
    static string scriptName = "ZTXJ_Manager";

    /// <summary>
    /// 脚本目录
    /// </summary>
    static string scriptPath = "Assets/Scripts";

    /// <summary>
    /// 备份
    /// </summary>
    static string BakStringFile = "Assets//Editor/ZTXJ_Manager.txt";

    /// <summary>
    /// 分隔符
    /// </summary>
    static string strSplit = @"~=~";

    /// <summary>
    /// 排除面板标签或关键词,例如Unity面板属性[Header("xxxx")]
    /// </summary>
    static string[] ignoreTags = {
            //Unity标签
            "[Header", "[Tooltip","[FormerlySerializedAs","[ContextMenuItem","[AddComponentMenu","[MenuItem","[ContextMenu","[CreateAssetMenu","[PreferenceItem","[SyncVar",
            //c#语法
            "[DllImport","const ", "case ", "///","void","Resources"," file://","/*","LoadAsset","Application.streamingAssetsPath","Application.dataPath"
        };

    /// <summary>
    /// 忽略某些字符串
    /// </summary>
    static string[] ignoreStr = { @"\n", @"\r" };

    /// <summary>
    /// 生成的字符串脚本
    /// </summary>
    static string StringFile = "";

    /// <summary>
    /// 找到包含匹配字符串的脚本
    /// </summary>
    static List<string> matchFiles;

    /// <summary>
    /// 字符串映射列表
    /// </summary>
    static Dictionary<string, byte[]> stringMap;

    [MenuItem("Tools/JmConfuse/转数字/字符串加密(数字)")]
    static void Encrpt()
    {
        //混淆字符串后生成的文件
        string targetScripts = scriptPath + "/" + scriptName + ".cs";
        if (File.Exists(targetScripts))
        {
            EditorUtility.DisplayDialog("提示", "字符串已混淆，需先恢复！！！", "确定", "取消");
            return;
        }
        if (!File.Exists(BakStringFile))
        {
            File.Create(BakStringFile);
        }
        //if (File.Exists(BakStringFile)) Decrypt();//先恢复原来字符串
        matchFiles = new List<string>();
        stringMap = new Dictionary<string, byte[]>();
        //查找所有脚本
        List<string> scriptFiles = new List<string>();
        FindScriptFiles(ref scriptFiles, scriptPath);
        //查引号中的字符串
        List<string> findStrings = FinScriptStrings(scriptFiles);
        //生成字符串类
        GenerateScript(findStrings);
        //替换脚本中的字符串
        ReplaceStringToStatic(matchFiles, stringMap);
    }

    [MenuItem("Tools/JmConfuse/转数字/字符串解密(数字)")]
    static void Decrypt()
    {
        //混淆字符串后生成的文件
        string targetScripts = scriptPath + "/" + scriptName + ".cs";
        if (!File.Exists(targetScripts))
        {
            EditorUtility.DisplayDialog("提示", "字符串还未加密", "确定", "取消");
            return;
        }
        string scriptFile = GetScriptFilePath();
        //查找所有脚本
        List<string> scriptFiles = new List<string>();
        FindScriptFiles(ref scriptFiles, scriptPath);

        //恢复字符串
        ReplaceStaticToString(scriptFiles);

        //移除备份文件，超过一定长度，才删除
        if (File.Exists(BakStringFile) && File.ReadAllLines(BakStringFile).Length > 2000)
        {
            File.Delete(BakStringFile);
        }

        //移除manager
        if (File.Exists(scriptFile))
        {
            File.Delete(scriptFile);
        }
    }

    /// <summary>
    /// 生成类文件
    /// </summary>
    /// <param name="strings"></param>
    static void GenerateScript(List<string> strings)
    {
        string content = "using System;\nusing System.Text;\n\npublic static class {0} \n{{ \n{1} \n}}";

        string comment = "\t/// <summary>\n\t/// {0}\n\t/// </summary>\n";
        string tpl = "\tpublic static string {0} {{ get {{ return Decode({1}); }} }}\n\n";
        string func = "\tstatic string Decode(byte[] str) { return Encoding.UTF8.GetString(str); }";
        string prop = "";
        string key;
        byte[] value;
        string byteContent = "new byte[]{";
        foreach (string str in strings)
        {
            value = Encode(str);
            int index = UnityEngine.Random.Range(0, 1000000);
            key = "_" + Md5(str + index);

            //遍历字节数组
            for (int i = 0; i < value.Length; i++)
            {
                byteContent += value[i] + ",";
            }
            byteContent = byteContent.TrimEnd(",".ToCharArray()) + "}";
            //加入映射表
            if (stringMap != null && !stringMap.ContainsKey(key))
            {
                stringMap.Add(key, value);//生成属性
                prop += string.Format(comment, str) + string.Format(tpl, key, byteContent);
                byteContent = "new byte[]{";
            }
            else
            {
                byteContent = "new byte[]{";
                continue;
            }
        }
        prop += func;
        content = string.Format(content, scriptName, prop);
        prop = "";
        string outfile = GetScriptFilePath();
        File.WriteAllText(outfile, content);
        Debug.Log("字符串编码成功：" + outfile);
    }

    /// <summary>
    /// 替换文件中的字符串
    /// </summary>
    /// <param name="scriptFiles"></param>
    /// <param name="strMap"></param>
    static void ReplaceStringToStatic(List<string> scriptFiles, Dictionary<string, byte[]> strMap)
    {
        string[] arrLine;

        string tpl = "{0}.{1}";
        string backup = "";
        //遍历所有的脚本路径文件：
        foreach (string path in scriptFiles)
        {
            string content = "";//新内容
            arrLine = File.ReadAllLines(path);
            for (int i = 0; i < arrLine.Length; i++)
            {
                bool isIgnore = false;
                foreach (string tags in ignoreTags)
                {
                    if (arrLine[i].Contains(tags))
                    {
                        isIgnore = true;
                        break;
                    }
                }
                //如果存在与限制标签的
                if (isIgnore)
                {
                    content += (arrLine[i] + "\n");
                    continue;
                }
                //满足替换条件的：
                foreach (KeyValuePair<string, byte[]> data in strMap)
                {
                    string oldStr = string.Format("\"{0}\"", Decode(data.Value));
                    string newStr = string.Format(tpl, scriptName, data.Key);
                    arrLine[i] = arrLine[i].Replace(oldStr, newStr);
                }
                content += (arrLine[i] + "\n");
            }

            File.WriteAllText(path, content);
        }

        //备份到文件
        foreach (KeyValuePair<string, byte[]> data in strMap)
        {
            backup += string.Format("{0}{1}{2}\n", string.Format(tpl, scriptName, data.Key), strSplit, string.Format("\"{0}\"", Decode(data.Value)));
        }
        if (!File.Exists(BakStringFile))
        {
            File.Create(BakStringFile);
        }
        File.AppendAllText(BakStringFile, backup, System.Text.Encoding.UTF8);
        Debug.Log("替换字符串完成");
    }

    /// <summary>
    /// 将文件中的静态加密字符串属性替换为字符串
    /// </summary>
    /// <param name="scriptFiles"></param>
    static void ReplaceStaticToString(List<string> scriptFiles)
    {
        string content;
        string tpl = "{0}.{1}";
        Dictionary<string, string> backupMaps = new Dictionary<string, string>();

        //从备份文件中恢复
        if (File.Exists(BakStringFile))
        {
            string[] bakLines = File.ReadAllLines(BakStringFile);

            if (bakLines.Length > 0)
            {
                int index = 0;
                foreach (string line in bakLines)
                {
                    string[] split = Regex.Split(line, strSplit);
                    if (string.IsNullOrEmpty(split[0]) == false && !backupMaps.ContainsKey(split[0]))
                    {
                        index++;
                        backupMaps.Add(split[0], split[1].TrimEnd('\n'));
                    }
                }
            }
        }
        //如果文本为空
        if (backupMaps.Count == 0)
        {
            //通过生成的脚本，还原字符串
            Dictionary<string, byte[]> strmap = RecoveryStringFromScript();
            //从类中还原
            foreach (KeyValuePair<string, byte[]> data in strmap)
            {
                backupMaps.Add(string.Format(tpl, scriptName, data.Key), string.Format("\"{0}\"", Decode(data.Value)));
            }
        }

        //还原替换
        if (backupMaps.Count > 0)
        {
            foreach (string path in scriptFiles)
            {
                content = File.ReadAllText(path);

                if (content.Contains(scriptName) == false)
                {
                    continue;
                }

                foreach (KeyValuePair<string, string> data in backupMaps)
                {
                    content = content.Replace(data.Key, data.Value);
                }

                File.WriteAllText(path, content);
            }
        }
        File.Delete(BakStringFile);
        scriptName = "";
        BakStringFile = "";
        Debug.Log("恢复字符串完成");
    }

    /// <summary>
    /// 将字恢复原到脚本
    /// </summary>
    /// <returns></returns>
    static Dictionary<string, byte[]> RecoveryStringFromScript()
    {
        //通过生成的脚本，还原字符串
        string scriptFile = GetScriptFilePath();
        Dictionary<string, byte[]> map = new Dictionary<string, byte[]>();

        if (File.Exists(scriptFile))
        {
            string content = File.ReadAllText(scriptFile);
            string pattern = "public static string (.*) { get {.*\\{(.*)\\}.*}";

            foreach (Match match in Regex.Matches(content, pattern))
            {
                if (!map.ContainsKey(match.Groups[1].Value))
                {
                    string arr = match.Groups[2].Value.Replace(",", "");
                    byte[] by = Encoding.UTF8.GetBytes(arr);
                    map.Add(match.Groups[1].Value, by);
                }
            }
        }
        return map;
    }

    /// <summary>
    /// 解码
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    static string Decode(byte[] str)
    {
        return Encoding.UTF8.GetString(str);
    }

    /// <summary>
    /// 获取生成后的脚本文件路径
    /// </summary>
    /// <returns></returns>
    static string GetScriptFilePath()
    {
        return Path.Combine(Path.GetFullPath("."), scriptPath + "/" + scriptName + ".cs");
    }

    /// <summary>
    /// 获取脚本文件
    /// </summary>
    /// <param name="path"></param>
    static void FindScriptFiles(ref List<string> files, string path, string subfix = ".cs")
    {
        DirectoryInfo dir = new DirectoryInfo(path);

        foreach (FileInfo NextFile in dir.GetFiles())
        {
            if (NextFile.FullName.Contains(scriptName))
            {
                continue;
            }

            if (NextFile.FullName.EndsWith(subfix, StringComparison.Ordinal))
            {
                files.Add(NextFile.FullName);
            }
        }
        //查找子文件夹
        foreach (DirectoryInfo NextFolder in dir.GetDirectories())
        {
            FindScriptFiles(ref files, NextFolder.FullName, subfix);
        }
    }

    /// <summary>
    /// 收集所有脚本文件的字符串
    /// </summary>
    static List<string> FinScriptStrings(List<string> scriptFiles)
    {
        List<string> result = new List<string>();
        if (scriptFiles.Count > 0)
        {
            //排除Header标签
            string pattern = "(?<!case\\s+)\"(.*)\"";
            foreach (string path in scriptFiles)
            {
                //排除之前生成的字符串类
                if (path.Contains(scriptName))
                {
                    continue;
                }

                //逐行检查
                string[] content = File.ReadAllLines(path);
                bool isMatch = false;
                foreach (string line in content)
                {
                    bool isIgnore = false;
                    //排除需要忽略的字符串
                    foreach (string tags in ignoreTags)
                    {
                        if (line.Contains(tags))
                        {
                            isIgnore = true;
                            break;
                        }
                    }

                    //排除特定标签
                    if (isIgnore) continue;
                    //正则查找：
                    foreach (Match match in Regex.Matches(line, pattern))
                    {

                        string matchStr = match.Groups[1].Value;
                        //排除需要忽略的内容
                        isIgnore = false;
                        foreach (string ig in ignoreStr)
                        {
                            if (matchStr.Contains(ig))
                            {
                                isIgnore = true;
                                break;
                            }
                        }

                        //排除特定标签
                        if (isIgnore) continue;

                        //添加进集合
                        if (!result.Contains(matchStr))
                        {
                            result.Add(matchStr);
                            isMatch = true;
                        }
                    }
                }
                if (isMatch)
                {
                    //存取所有的字符串路径
                    matchFiles.Add(path);
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 编码
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    static byte[] Encode(string str)
    {
        byte[] bStr = System.Text.Encoding.UTF8.GetBytes(str);
        return bStr;
    }

    /// <summary>
    /// 取Md5值
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    static string Md5(string str)
    {
        byte[] result = System.Text.Encoding.Default.GetBytes(str);
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] output = md5.ComputeHash(result);
        int index = UnityEngine.Random.Range(5, output.Length);
        str = BitConverter.ToString(output, 1, index-1).Replace("-", ""); ;
        return str;
    }
}
