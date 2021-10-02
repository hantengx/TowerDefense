using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class EventListener : MonoBehaviour
    {
        private int number;
        private void Start()
        {
            EventManager.Instance.AddListener<TestEvent>(OnTestEvent);

            number = transform.GetSiblingIndex();
        }

        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener<TestEvent>(OnTestEvent);
        }

        private void OnTestEvent(TestEvent e)
        {
            // Debug.Log("listener: " + number);
        }
    }
}