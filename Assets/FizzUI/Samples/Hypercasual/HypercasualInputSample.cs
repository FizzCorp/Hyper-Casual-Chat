using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fizz.Demo
{
    using UI;

    // Hyper-casual Input sample is designed to demonstrate the usage of FizzChatView with hyper-casual Input View. 
    // FizzHypercasualInputView is used as a static keyboard which will show hyper-casual phrases and sticker. 
    
    public class HypercasualInputSample : MonoBehaviour
    {
        [SerializeField] FizzChatView ChatView;

        private void Awake ()
        {
            SetupChatView ();
        }

        private void OnEnable ()
        {
            AddHypercasualInputChannel ();
        }

        private void OnDisable ()
        {
            RemoveHypercasualInputChannel ();
        }

        private void SetupChatView ()
        {
            ChatView.EnableFetchHistory = true;
            ChatView.ShowMessageTranslation = true;
            ChatView.ShowChannelsButton = false;

            ChatView.onClose.AddListener (() => gameObject.SetActive (false));
        }

        private void AddHypercasualInputChannel ()
        {
            ChatView.AddChannel ("hypercasual-channel", true);
        }

        void RemoveHypercasualInputChannel ()
        {
            ChatView.RemoveChannel ("hypercasual-channel");
        }

        public void HandleClose ()
        {
            SceneManager.LoadScene ("SceneSelector");
        }
    }
}