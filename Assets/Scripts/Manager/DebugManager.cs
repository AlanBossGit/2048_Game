using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager
{
    public static void DebugLog(string str)
    {
        Debug.Log(str);
    }

    public static void DebugWarning(string str)
    {
        Debug.LogWarning(str);
    }

    public static void DebugError(string str)
    {
        Debug.LogError(str);
    }
}
