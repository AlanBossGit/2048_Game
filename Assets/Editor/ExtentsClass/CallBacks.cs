﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class CallBacks : MonoBehaviour {

    private static string TmpMethodNamesKey = "FinishedNames_2";

    public static void Set<T>(string methodName, string arg = null)
    {
        var type = typeof(T);
        string methoedName = type.FullName + "." + methodName + "(" + arg + ")";
        AddTmpMethodName(methoedName);
    }

    static string[] GetTmpMethodNameArr()
    {
        return GetTmpMethodNames().Split(';').Where(value => value != null && value.Trim() != "").ToArray();
    }

    static void AddTmpMethodName(string value)
    {
        SetTmpMethodNames(GetTmpMethodNames() + ";" + value);
    }

    static string GetTmpMethodNames()
    {
        if (EditorPrefs.HasKey(TmpMethodNamesKey))
            return EditorPrefs.GetString(TmpMethodNamesKey);
        return "";
    }

    static void SetTmpMethodNames(string value)
    {
        EditorPrefs.SetString(TmpMethodNamesKey, value);
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        var methods = GetTmpMethodNameArr();
        foreach (var method in methods)
        {
            var pointIndex = method.LastIndexOf(".");
            var left = method.LastIndexOf("(");
            var right = method.LastIndexOf(")");
            string className = method.Substring(0, pointIndex);
            string methodName = method.Substring(pointIndex + 1, left - pointIndex - 1);
            string argName = method.Substring(left + 1, right - left - 1);
            Type type = Type.GetType(className);
            if (type == null)
            {
                Debug.LogError("type == null");
                continue;
            }
            var method1 = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { }, null);
            var method2 = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { typeof(string) }, null);
            if (method1 == null && method2 == null)
            {
                Debug.LogError("method1 == null && method2 == null");
                continue;
            }
            if (method1 != null && argName.Trim() == "")
                method1.Invoke(null, null);
            if (method2 != null && argName.Trim() != "")
                method2.Invoke(null, new object[] { argName });
        }
        SetTmpMethodNames("");
    }


}
