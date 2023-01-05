using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static EventManager;


    public interface IListener
    {
         public void Listene(Define.GameEvent eventType, Component Sender, object param = null);
    }
