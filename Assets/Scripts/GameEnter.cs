using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
public class GameEnter : MonoBehaviour
{
    public Image load;
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("Launch");
    }
}
