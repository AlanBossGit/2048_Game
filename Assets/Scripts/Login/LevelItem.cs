using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Framework;

public class LevelItem : MonoBehaviour
{
    //关卡预设的按钮
    public Button BtnLevel;
    //关卡的锁头图片
    public Image Lock;
    //关卡的文本显示
    public Text TextLevel;
    //关卡的下标
    public int index;

    void Awake()
    {
        BtnLevel = GetComponent<Button>();
        Lock = transform.Find("Lock").GetComponent<Image>();
        TextLevel = transform.Find("Text").GetComponent<Text>();
    }
    
    void Start()
    {
        BtnLevel.onClick.AddListener(() =>
        {
            GameManager.levelIndex = index;
            GameManager.PlayClickAudio();
            MessageManager.TriggerEvent(DefineManager.OpenGameScene);
        });
    }
}
