using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fizz.UI
{
    public class FizzTooltipComponent : MonoBehaviour
    {
        [SerializeField] FizzTooltipItem botton;

        private Action<string> _callback;
        private List<GameObject> _options;

        void Awake()
        {
            _options = new List<GameObject>();
        }

        public void SetupMenu(Dictionary<string, string> items, Action<string> callback)
        {
            Reset();

            _callback = callback;
            foreach (KeyValuePair<string, string> item in items)
            {
                FizzTooltipItem button = Instantiate(botton);
                button.gameObject.SetActive(true);
                button.transform.SetParent(transform, false);
                button.transform.localScale = Vector3.one;

                button.SetupButton(item.Key, item.Value, ButtonPressed);
                _options.Add(button.gameObject);
            }
        }

        private void Reset()
        {
            foreach (GameObject go in _options)
                Destroy(go);
            _options.Clear();
        }

        private void ButtonPressed(string action)
        {
            _callback(action);
        }
    }
}
