using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text text;
    BestScore bS;
    public static int score = 0;
    public static int bestScore = 0;
    //점수로 보상줄 수 있음
    public void Start()
    {
        score = 0;
        SetScore();
        bS = FindObjectOfType<BestScore>();

    }

    public void GetKillScore()
    {
        score += 10;
        if (score > bestScore)
        {
            bestScore = score;
        }
        SetScore();
        bS.SetBestScore();
    }

    public void GetDistanceScore()
    {
        score += (int)GroundController.chatperSize;
        if (score > bestScore)
        {
            bestScore = score;
        }
        SetScore();
        bS.SetBestScore();
    }

    public void SetScore()
    {
        text.text = score.ToString();
    }
}
