using System;
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
    public MonsterData monsterdata;
    public void Start()
    {
        score = 0;
        SetScore();
        bS = FindObjectOfType<BestScore>();
        Managers.Events.AddListener(Define.GameEvent.monsterDestroy, GetKillScore);
    }

    private void GetKillScore(Define.GameEvent eventType, Component Sender, object param)
    {
        if(eventType == Define.GameEvent.monsterDestroy)
        {
            Monster monster = (Monster)Sender;
            score += monster.MonsterData.Score;
            SetScore();
            bS.SetBestScore();
           
        }
        
    }

  
    public void GetDistanceScore()
    {
        score += -1 * (int)GroundController.chatperSize;
        SetScore();
        bS.SetBestScore();
    }

    public void SetScore()
    {
        if (score > bestScore)
        {
            bestScore = score;
        }
        text.text = score.ToString();
    }
}
