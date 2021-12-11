using System;
using UnityEngine;

namespace Test
{
    public class TestDelegate : MonoBehaviour
    {
        private void Awake()
        {
            var actions = new Action[3];
 
            // var j = 0;
            // foreach (var c in "bar")
            //     actions[j++] = () => Debug.Log(c);
            
            //闭包引用传递，
            for (int i = 0; i < actions.Length; i++)
            {
                var tmp = i;
                actions[i] = () => Debug.Log(tmp);
            }
 
 
            foreach (var a in actions)
                a();
        }
    }
}