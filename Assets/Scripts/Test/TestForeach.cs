using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class TestForeach : MonoBehaviour
    {
        public int count = 100;

        private int[] intArray;
        private List<int> intList;
        private ArrayList arrayList;
        
        private void Awake()
        {
            intArray = new int[count];
            intList = new List<int>();
            arrayList = new ArrayList();
            
            for (int i = 0; i < count; i++)
            {
                intArray[i] = i;
                intList.Add(i);
                arrayList.Add(i);
            }
            
            // InvokeRepeating(nameof(Test), 0f, 1f);
        }

        private void Update()
        {
            Test();
        }

        private void Test()
        {
            foreach (var item in intArray)
            {
                // Debug.Log(item);
            }

            foreach (var i in intList)
            {
                // Debug.Log(i);
            }

            //only this have gc, get enumerator
            foreach (var item in arrayList)
            {
                // Debug.Log(item);
            }
        }
    }
}