using Fizz.Chat;
using Fizz.UI.Core;
using Fizz.UI.Extentions;
using UnityEngine;

namespace Fizz.UI
{
    public class FizzHypercasualCustomViewProvider : MonoBehaviour, IFizzCustomMessageCellViewDataSource
    {
        [SerializeField] FizzChatView ChatView;
        [SerializeField] FizzCustomPhraseView PhrasePrefab;
        [SerializeField] FizzCustomStickerView StickerPrefab;

        IFizzHypercasualInputDataProvider dataProvider;

        private void Awake ()
        {
            dataProvider = Registry.PredefinedInputDataProvider;

            ChatView.SetCustomDataViewSource (this);
        }

        public RectTransform GetCustomMessageCellViewNode (FizzChannelMessage message)
        {
            if (message.Data != null)
            {
                if (message.Data.ContainsKey ("type") && message.Data["type"].Equals ("fizz_predefine_phrase"))
                {
                    string id = message.Data["phrase_id"];
                    FizzHypercasualPhraseDataItem phraseData = dataProvider.GetPhrase (id);
                    if (phraseData != null)
                    {
                        FizzCustomPhraseView phraseView = Instantiate (PhrasePrefab);
                        phraseView.gameObject.SetActive (true);
                        phraseView.SetPhrase (phraseData.GetLocalizedContent (Application.systemLanguage));

                        // Theme color change
                        phraseView.GetComponent<ThemeLabelColor>().SetColor((message.From.Equals(FizzService.Instance.UserId)? ThemeColor.Base_2 : ThemeColor.Base_1));

                        return phraseView.GetComponent<RectTransform> ();
                    }

                }
                else if (message.Data.ContainsKey ("type") && message.Data["type"].Equals ("fizz_predefine_sticker"))
                {
                    string id = message.Data["sticker_id"];
                    FizzHypercasualStickerDataItem stickerData = dataProvider.GetSticker (id);
                    if (stickerData != null)
                    {
                        FizzCustomStickerView sticker = Instantiate (StickerPrefab);
                        sticker.gameObject.SetActive (true);
                        sticker.SetSticker (stickerData.Content);
                        return sticker.GetComponent<RectTransform> ();
                    }

                }
            }
            return null;
        }
    }
}