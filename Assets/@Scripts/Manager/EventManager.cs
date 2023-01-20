using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
public class EventManager 
{
    public delegate void OnEvent(Define.GameEvent eventType,Component Sender, object param = null);
    private Dictionary<Define.GameEvent, List<OnEvent>> Listeners = new Dictionary<GameEvent, List<OnEvent>>();

    public void AddListener(Define.GameEvent eventType,OnEvent Listener)
    {
        List<OnEvent> ListenList = null;
        if(Listeners.TryGetValue(eventType, out ListenList))
        {
            ListenList.Add(Listener);
            return;
        }
        ListenList = new List<OnEvent>();
        ListenList.Add(Listener);
        Listeners.Add(eventType, ListenList);
    }

    public void PostNotification(Define.GameEvent eventType,Component Sender, object param = null)
    {
        List<OnEvent> ListenList = null;
        if (!Listeners.TryGetValue(eventType, out ListenList))
            return;
        for (int i = 0; i < ListenList.Count; i++)
        {
            ListenList?[i](eventType,Sender,param);
        }
    }
    public void RemoveEvent(Define.GameEvent eventType) => Listeners.Remove(eventType);
    public void RemoveAll()
    {
        Listeners.Clear();
    }
}
