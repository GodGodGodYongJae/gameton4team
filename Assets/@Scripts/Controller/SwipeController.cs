using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeController
{
    private Vector2 touchBeganPos, touchEndPos, touchDif;
    private float swipeSensitivity;
    Player.PlayerActionKey ActionKey;

    public SwipeController()
    {
        touchBeganPos = Vector2.zero;
        touchEndPos = Vector2.zero;
        touchDif = Vector2.zero; ;
        swipeSensitivity = 500.0f;
        Managers.UpdateAction += MUpdate;
    }
    void MUpdate()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                touchBeganPos = touch.position;
            }
            if (touch.phase == TouchPhase.Ended)
            {
                touchEndPos = touch.position;
                touchDif = (touchEndPos - touchBeganPos);
                if (touchDif.y > 0 && Mathf.Abs(touchDif.y) > Mathf.Abs(touchDif.x) && Mathf.Abs(touchDif.y) > swipeSensitivity)
                {
                    Debug.Log(Mathf.Abs(touchDif.y));
                    ActionKey = Player.PlayerActionKey.Jump;
                }
                else
                {

                    ActionKey = Player.PlayerActionKey.Direction;
                }
                Managers.Events.PostNotification(Define.GameEvent.playerEvents, null, ActionKey);
            }
        }
    }
}
