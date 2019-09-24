using System.Collections.Generic;
using Fizz.Common;
using Fizz.UI.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Fizz.Demo
{
    public class ConnectionScript : MonoBehaviour
    {
        [SerializeField] InputField userIdInputField;
        [SerializeField] InputField userNameInputField;
        [SerializeField] Dropdown langCodeDropDown;
        [SerializeField] Toggle translationToggle;

        [SerializeField] Button connectButton;
        [SerializeField] Button disconnectButton;

        [SerializeField] Button launchButton;

        private readonly FizzChannelMeta hypercasualInputChannel = new FizzChannelMeta ("hypercasual-channel", "Global", "DEMO");

        private void Awake ()
        {
            SetupView ();
        }

        void OnEnable ()
        {
            AddListeners ();
        }

        void OnDisable ()
        {
            RemoveListeners ();
        }

        public void HandleConnect ()
        {
            try
            {
                if (FizzService.Instance.IsConnected)
                    return;

                FizzService.Instance.Open (
                    userIdInputField.text,                                  //UserId
                    userNameInputField.text,                                //UserName
                    FizzLanguageCodes.AllLanguages[langCodeDropDown.value], //LanguageCode
                    FizzServices.All,
                    translationToggle.isOn,                                 //Translation
                    (success) =>
                {
                    if (success)
                    {
                        FizzLogger.D ("FizzClient Opened Successfully!!");

                        FizzService.Instance.SubscribeChannel (hypercasualInputChannel);
                    }
                });
            }
            catch { FizzLogger.E ("Unable to connect to Fizz!"); }
        }

        public void HandleDisconnect ()
        {
            try
            {
                FizzService.Instance.Close ();
            }
            catch { FizzLogger.E ("Unable to disconnect to Fizz!"); }
        }

        private void SetupView ()
        {
            connectButton.gameObject.SetActive (!FizzService.Instance.IsConnected);
            disconnectButton.gameObject.SetActive (FizzService.Instance.IsConnected);

            launchButton.interactable = FizzService.Instance.IsConnected;

            SetupIdAndNameInputField ();
            SetupLanguageDropDown ();
            SetupTranslationToggle ();
        }

        private void SetupIdAndNameInputField ()
        {
            string userId = PlayerPrefs.GetString (USER_ID_KEY, System.Guid.NewGuid ().ToString ());
            string userName = PlayerPrefs.GetString (USER_NAME_KEY, "User");

            userIdInputField.text = userId;
            userNameInputField.text = userName;

            HandleUserCradChange (string.Empty);
        }

        private void SetupLanguageDropDown ()
        {
            langCodeDropDown.ClearOptions ();
            List<Dropdown.OptionData> optionsData = new List<Dropdown.OptionData> ();
            foreach (IFizzLanguageCode langCode in FizzLanguageCodes.AllLanguages)
            {
                optionsData.Add (new Dropdown.OptionData (langCode.Language));
            }
            langCodeDropDown.AddOptions (optionsData);

            langCodeDropDown.value = PlayerPrefs.GetInt (USER_LANG_CODE_KEY, 0);
        }

        private void SetupTranslationToggle ()
        {
            translationToggle.isOn = PlayerPrefs.GetInt (USER_TRANSLATION_KEY, 1) == 1;
        }

        private void AddListeners ()
        {
            try
            {
                FizzService.Instance.OnConnected += OnConnected;
                FizzService.Instance.OnDisconnected += OnDisconnected;
            }
            catch
            {
                FizzLogger.E ("Something went wrong with binding events with FizzService.");
            }

            userNameInputField.onEndEdit.AddListener (HandleUserCradChange);
            userNameInputField.onEndEdit.AddListener (HandleUserCradChange);

            langCodeDropDown.onValueChanged.AddListener (HandleLangCodeChange);
            translationToggle.onValueChanged.AddListener (HandleTranslationToggleChange);
        }

        private void RemoveListeners ()
        {
            try
            {
                FizzService.Instance.OnConnected -= OnConnected;
                FizzService.Instance.OnDisconnected -= OnDisconnected;
            }
            catch
            {
                FizzLogger.E ("Something went wrong with binding events with FizzService.");
            }

            userNameInputField.onEndEdit.RemoveListener (HandleUserCradChange);
            userNameInputField.onEndEdit.RemoveListener (HandleUserCradChange);

            langCodeDropDown.onValueChanged.RemoveListener (HandleLangCodeChange);
            translationToggle.onValueChanged.RemoveListener (HandleTranslationToggleChange);
        }

        private void OnConnected (bool sync)
        {
            connectButton.gameObject.SetActive (false);
            disconnectButton.gameObject.SetActive (true);

            launchButton.interactable = true;
        }

        private void OnDisconnected (FizzException ex)
        {
            connectButton.gameObject.SetActive (true);
            disconnectButton.gameObject.SetActive (false);

            launchButton.interactable = false;
        }

        private void HandleUserCradChange (string str)
        {
            PlayerPrefs.SetString (USER_ID_KEY, userIdInputField.text);
            PlayerPrefs.SetString (USER_NAME_KEY, userNameInputField.text);
            PlayerPrefs.Save ();
        }

        private void HandleLangCodeChange (int index)
        {
            PlayerPrefs.SetInt (USER_LANG_CODE_KEY, index);
            PlayerPrefs.Save ();
        }

        private void HandleTranslationToggleChange (bool isOn)
        {
            PlayerPrefs.SetInt (USER_TRANSLATION_KEY, isOn ? 1 : 0);
            PlayerPrefs.Save ();
        }

        private readonly string USER_ID_KEY = "FIZZ_USER_ID";
        private readonly string USER_NAME_KEY = "FIZZ_USER_NAME";
        private readonly string USER_LANG_CODE_KEY = "FIZZ_USER_LANG_CODE";
        private readonly string USER_TRANSLATION_KEY = "FIZZ_USER_TRANSLATION";
    }
}