using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitUI : MonoBehaviour
{
    private Button BtnSure;
    private Button BtnCancel;


    void Awake()
    {
        BtnSure = transform.Find("UI/BtnSure").GetComponent<Button>();
        BtnCancel = transform.Find("UI/BtnCancel").GetComponent<Button>();
    }

    void Start()
    {
        BtnSure.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        BtnCancel.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
        });
    }
}
