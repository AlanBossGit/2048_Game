using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelType
{
    SHuiGuo,
    SHuZI
}
public class PlayInfo : SingletonMono<PlayInfo>
{
    private LevelType type;

    public void SetLevelType(LevelType ty)
    {
        type = ty;
    }

    public LevelType GetLevelType()
    {
        return type;
    }



}