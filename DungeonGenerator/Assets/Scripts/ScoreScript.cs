using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreScript : MonoBehaviour
{
    public Text MyScoreText;
    public static int ScoreNum;

    // Start is called before the first frame update
    void Start()
    {
        ScoreNum = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        MyScoreText.text = "Enemies killed: " + ScoreNum + "/10";
        if (ScoreNum == 10)
        {
            SceneManager.LoadScene("Einde");
        }
    }
}
