using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        swipeSensitivity = 120.0f;
        Managers.UpdateAction += MUpdate;
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
}
    void MUpdate()
    {
        if(Time.timeScale == 1)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (IsPointerOverUIObject())
                {
                    return;
                }

                if (touch.phase == TouchPhase.Began)
                {
                    touchBeganPos = touch.position;
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    touchEndPos = touch.position;
                    touchDif = (touchEndPos - touchBeganPos);
                    if (touchDif.y > 0 && Mathf.Abs(touchDif.y) > Mathf.Abs(touchDif.x) && Mathf.Abs(touchDif.y) > swipeSensitivity)
                    {
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
}
