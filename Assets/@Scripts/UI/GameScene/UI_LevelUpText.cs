using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UI_LevelUpText : MonoBehaviour
{
    private SpriteRenderer sprite;

    Sequence mySequence;
    private float duration = 3;
    protected CancellationTokenSource cts = new CancellationTokenSource();
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
 
    private void OnEnable()
    {
        if (cts != null)
            cts.Dispose();
        cts = new();
        sprite.color = new Color(255, 255, 255, 255);
        
        tweenText().Forget();
    }
    private void OnDisable()
    {
        cts.Cancel();
    }

    async UniTaskVoid tweenText()
    {

        
        await UniTask.Yield(PlayerLoopTiming.LastUpdate);

        mySequence = DOTween.Sequence();
        await mySequence.Append(transform.DOMove(new Vector3(transform.position.x, transform.position.y + 3), duration)).
Join(sprite.DOColor(new Color(255, 255, 255, 0), duration));


        await mySequence.WithCancellation(cts.Token);
        this.gameObject.SetActive(false);
    }
}
