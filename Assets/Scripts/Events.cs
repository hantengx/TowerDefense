using System.Collections;
using UnityEngine;
using static Assets.Scripts.EventManager;

namespace Assets.Scripts
{
    public class Events
    {

    }

    public class UIClickGrid : GameEvent
    {
        public UIClickGrid(Hashtable p) : base(p)
        {
        }
    }

    public class TestEvent : GameEvent
    {
        public TestEvent(Hashtable p) : base(p)
        {
        }
    }
}