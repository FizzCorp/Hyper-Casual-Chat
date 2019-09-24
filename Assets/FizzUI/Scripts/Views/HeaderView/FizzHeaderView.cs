using Fizz.UI.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Fizz.UI
{
    public class FizzHeaderView : FizzBaseComponent
    {
        [SerializeField] Text TitleLabel;
        [SerializeField] Button ChannelsButton;
        [SerializeField] Button CloseButton;

        public UnityEvent OnClose;
        public UnityEvent OnChannel;

        public void SetVisibility (bool visible)
        {
            CanvasGroup cg = gameObject.GetComponent<CanvasGroup> ();
            if (cg != null)
            {
                cg.alpha = visible ? 1 : 0;
            }
        }

        public void SetTitleText (string title)
        {
            TitleLabel.text = title;
        }

        public void SetChannelButtonVisibility (bool visible)
        {
            ChannelsButton.gameObject.SetActive (visible);
        }

        public void SetCloseButtonVisibility (bool visible)
        {
            CloseButton.gameObject.SetActive (visible);
        }

        public void Reset ()
        {
            TitleLabel.text = string.Empty;
        }

        protected override void OnEnable ()
        {
            base.OnEnable ();

            CloseButton.onClick.AddListener (HandleCloseButton);
            ChannelsButton.onClick.AddListener (HandleChannelsButton);
        }

        protected override void OnDisable ()
        {
            base.OnDisable ();

            CloseButton.onClick.RemoveListener (HandleCloseButton);
            ChannelsButton.onClick.RemoveListener (HandleChannelsButton);
        }

        private void HandleCloseButton ()
        {
            if (OnClose != null)
            {
                OnClose.Invoke ();
            }
        }

        private void HandleChannelsButton ()
        {
            if (OnChannel != null)
            {
                OnChannel.Invoke ();
            }
        }
    }
}