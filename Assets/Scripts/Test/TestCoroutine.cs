using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class TestCoroutine : MonoBehaviour
    {
        private DateTime m_Time;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Test());
        }

        /// <summary>
        /// gameObject.setActive(false) will stop
        /// </summary>
        /// <returns></returns>
        private IEnumerator Test()
        {
            int count = 10;
            while (count-- > 0)
            {
                Debug.Log(count);
                yield return new WaitForSeconds(1);
            }
        }
    }
}

