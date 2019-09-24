using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fizz.UI
{
    public class FizzTooltipItem : MonoBehaviour
    {
        [SerializeField] Button button;
        [SerializeField] Text text;

        private Action<string> _clickEvent;
        private string _id;

        public void SetupButton(string id, string btnText, Action<string> clickEvent)
        {
            _id = id;
            _clickEvent = clickEvent;
            text.text = btnText;

            button.onClick.AddListener(delegate
            {
                _clickEvent.Invoke(_id);
            });
        }
    }
}