using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "ResourcesContainers")]
public class ResourcesContainers : ScriptableObject
{
    public static ResourcesContainers GetResourcesContainers()
    {
        return Resources.Load<ResourcesContainers>("ResourcesContainers");
    }
    
    public List<Sprite> BlockSpriteList = new List<Sprite>();
    public List<Sprite> NumSpriteList = new List<Sprite>();

    public AudioClip clickClip;
    public AudioClip failureClip;
     
    public GameObject blockItemPre;
    public GameObject levelItemPre;

}
