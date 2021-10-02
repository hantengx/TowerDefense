using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class EventManager
    {
        private static EventManager instance;

        public static EventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventManager();
                }

                return instance;
            }
        }
        
        public class GameEvent
        {
            public Hashtable param;
            public GameEvent(Hashtable p)
            {
                this.param = p;
            }
            public GameEvent()
            {

            }
        }

        private class EventTimer
        {
            public GameEvent e;
            public float timer;
        }

        public delegate void EventDelegate<T>(T e) where T : GameEvent;
        private Dictionary<System.Type, System.Delegate> delegates = new Dictionary<System.Type, System.Delegate>();
        private ArrayList list = new ArrayList();

        public void AddListener<T>(EventDelegate<T> del) where T : GameEvent
        {
            if (delegates.ContainsKey(typeof(T)))
            {
                System.Delegate tempDel = delegates[typeof(T)];

                delegates[typeof(T)] = System.Delegate.Combine(tempDel, del);
            }
            else
            {

                delegates[typeof(T)] = del;
            }
        }


        public void RemoveListener<T>(EventDelegate<T> del) where T : GameEvent
        {
            if (delegates.ContainsKey(typeof(T)))
            {

                var currentDel = System.Delegate.Remove(delegates[typeof(T)], del);

                if (currentDel == null)
                {
                    delegates.Remove(typeof(T));
                }
                else
                {
                    delegates[typeof(T)] = currentDel;
                }
            }
        }
        public void Update()
        {
            foreach (EventTimer t in list)
            {
                if (Time.time >= t.timer)
                {
                    Raise(t.e);
                    list.Remove(t);
                    break;
                }

            }

        }
        public void Raise(GameEvent e, float delay)
        {
            var t = new EventTimer();
            t.timer = Time.time + delay;
            t.e = e;
            list.Add(t);
        }

        public void Raise<T>(T e) where T : GameEvent
        {
            if (e == null)
            {
                Debug.Log("Invalid event argument: " + e.GetType().ToString());
                return;
            }

            if (!delegates.ContainsKey(e.GetType())) return;
            var del = delegates[e.GetType()];
            foreach (var subDel in del.GetInvocationList())
            {
                if (subDel.Target.Equals(null))
                {
                    var currentDel = System.Delegate.Remove(del, subDel);

                    if (currentDel == null)
                    {
                        delegates.Remove(e.GetType());
                    }
                    else
                    {
                        delegates[e.GetType()] = currentDel;
                    }
                }
                else
                {
                    if (subDel is EventDelegate<T> callBack)
                    {
                        callBack.Invoke(e);
                    }
                    else
                    {
                        subDel.DynamicInvoke(e);
                    }
                }
            }
        }

    }
}