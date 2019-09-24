using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fizz.UI
{
    public class FizzHypercasualTagTabView : MonoBehaviour
    {
        [SerializeField] Text TitleLabel;
        [SerializeField] Image BGImage;
        [SerializeField] Button button;

        public Action<FizzHypercasualTagTabView> OnTabClick;

        public string Tag { get { return tabTag; } }

        private string tabTag = string.Empty;

        public void SetTag (string tag)
        {
            this.tabTag = tag;

            TitleLabel.text = tabTag.ToUpper ();
        }

        public void SetSelected (bool selected)
        {
            BGImage.gameObject.SetActive (selected);
            button.interactable = !selected;
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
            if (OnTabClick != null)
                OnTabClick.Invoke (this);
        }
    }
}