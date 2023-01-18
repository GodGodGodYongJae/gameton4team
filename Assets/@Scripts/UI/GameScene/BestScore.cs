using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestScore : MonoBehaviour
{
    public Text text;

    public void SetBestScore()
    {
        text.text = Score.bestScore.ToString();
    }
}
