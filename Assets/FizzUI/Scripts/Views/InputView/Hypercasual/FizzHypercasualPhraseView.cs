using UnityEngine;
using UnityEngine.UI;

namespace Fizz.UI
{
    using Extentions;
    using System;

    public class FizzHypercasualPhraseView : MonoBehaviour
    {
        [SerializeField] Text phraseLabel;

        public Action<FizzHypercasualPhraseView> OnPhraseClick;

        public FizzHypercasualPhraseDataItem PhraseData { get { return data; } }

        private FizzHypercasualPhraseDataItem data;
        private Button button;

        public void SetPhraseData (FizzHypercasualDataItem dataItem)
        {
            data = (FizzHypercasualPhraseDataItem)dataItem;

            phraseLabel.text = data.GetLocalizedContent (Application.systemLanguage);
        }

        private void Awake ()
        {
            button = gameObject.GetComponent<Button> ();
        }

        private void OnEnable ()
        {
            button.onClick.AddListener (OnClick);
        }

        private void OnDisable ()
        {
            button.onClick.RemoveListener (OnClick);
        }

        private void OnClick ()
        {
            if (OnPhraseClick != null)
                OnPhraseClick.Invoke (this);
        }
    }
}