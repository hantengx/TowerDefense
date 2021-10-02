using System;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class TestEvents : MonoBehaviour
    {
        public int gameObjectAmount = 20;
        public int testEventAmount = 100;
        private Button button;
        
        private void Start()
        {
            for (int i = 0; i < gameObjectAmount; i++)
            {
                var obj = new GameObject();
                obj.AddComponent<EventListener>();
                obj.transform.SetParent(transform);
            }

            button = gameObject.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnClick);
            }
        }

        private void OnClick()
        {
            Debug.Log("onClick");
            var testEvent = new TestEvent(new Hashtable());
            for (int i = 0; i < testEventAmount; i++)
            {
                EventManager.Instance.Raise(testEvent);
            }
        }

        private void FixedUpdate()
        {
            EventManager.Instance.Update();
        }
    }
}