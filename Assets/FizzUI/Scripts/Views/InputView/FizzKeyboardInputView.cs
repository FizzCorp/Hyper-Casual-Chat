using Fizz.UI.Components;
using Fizz.UI.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Fizz.UI
{
    public class FizzKeyboardInputView : FizzInputView
    {
        /// <summary>
        /// The background image.
        /// </summary>
        [SerializeField] Image BackgroundImage;
        /// <summary>
        /// The input field for Editor.
        /// </summary>
        [SerializeField] InputFieldWithEmoji MessageInputField;
        /// <summary>
        /// The send button.
        /// </summary>
        [SerializeField] Button SendButton;

        private void Awake ()
        {
            Initialize ();
        }

        protected override void OnEnable ()
        {
            base.OnEnable ();

            SendButton.onClick.AddListener (HandleOnSend);
            MessageInputField.onDone.AddListener (HandleOnSend);

#if UNITY_EDITOR
            MessageInputField.onEndEdit.AddListener (OnDoneButtonPress);
#endif
        }

        protected override void OnDisable ()
        {
            base.OnDisable ();

            SendButton.onClick.RemoveListener (HandleOnSend);
            MessageInputField.onDone.RemoveListener (HandleOnSend);

#if UNITY_EDITOR
            MessageInputField.onEndEdit.RemoveListener (OnDoneButtonPress);
#endif

            MessageInputField.DeactivateInputField ();
        }

        void OnApplicationPause (bool pauseState)
        {
            if (pauseState)
            {
                MessageInputField.DeactivateInputField ();
            }
        }

        #region Public Methods

        public override void Reset ()
        {
            base.Reset ();

            MessageInputField.ResetText ();
        }

        #endregion

        #region Methods

        void Initialize ()
        {
            UpdatePlaceholderText ();
        }

        void HandleOnSend ()
        {
            string messageText = MessageInputField.text.Trim ();
            OnSendMessage.Invoke (messageText);
            Reset ();
        }

        void UpdatePlaceholderText ()
        {
            if (MessageInputField.placeholder != null)
            {
                Text placeHolderText = MessageInputField.placeholder.GetComponent<Text> ();
                if (placeHolderText != null)
                {
                    placeHolderText.text = Registry.Localization.GetText ("Message_PlaceHolderTypeMsg");
                }
            }
        }

        public void OnDoneButtonPress (string text)
        {
            OnSendMessage.Invoke (text.Trim ());
            Reset ();
        }

        #endregion
    }
}
