using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowWinner : MonoBehaviour
{
    private string P1Win;
    // Start is called before the first frame update
    void Start()
    {
        P1Win = PlayerPrefs.GetString("P1W");
        if(P1Win == "1")
        {
            GameObject.Find("chic-white").SetActive(false);
            GameObject.Find("chic").SetActive(true);
        }
        else
        {
            GameObject.Find("chic").SetActive(false);
            GameObject.Find("chic-white").SetActive(true);
        }
        P1Win = "0";
         PlayerPrefs.DeleteAll();
    }
}
