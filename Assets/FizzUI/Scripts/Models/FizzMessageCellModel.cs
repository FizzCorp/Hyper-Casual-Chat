using System;
using System.Collections.Generic;
using Fizz.Chat;
using Fizz.Common;

namespace Fizz.UI.Model
{
    public class FizzMessageCellModel : FizzChannelMessage
    {
        public FizzChatCellType Type { get; set; }

        public static readonly string KEY_CLIENT_ID = "clientId";

        public FizzMessageCellModel() { }

        public FizzMessageCellModel(long id,
                                  string from,
                                  string nick,
                                  string to,
                                  string body,
                                  Dictionary<string, string> data,
                                  IDictionary<string, string> translations,
                                  long created)
            : base(id, from, nick, to, body, data, translations, created)
        {
            TranslationState = FizzChatCellTranslationState.Translated;
            DeliveryState = FizzChatCellDeliveryState.Pending;

            if (data != null && data.ContainsKey(KEY_CLIENT_ID))
            {
                AlternateId = long.Parse(data[KEY_CLIENT_ID]);
            }
        }

        public long AlternateId { get; protected set; }
        public FizzChatCellDeliveryState DeliveryState { get; set; }
        public FizzChatCellTranslationState TranslationState { get; protected set; }

        public void ToggleTranslationState()
        {
            if (TranslationState == FizzChatCellTranslationState.Original)
                TranslationState = FizzChatCellTranslationState.Translated;
            else
                TranslationState = FizzChatCellTranslationState.Original;
        }

        public string GetActiveMessage()
        {
            if (TranslationState == FizzChatCellTranslationState.Original)
            {
                return Body;
            }
            else
            {
                string langCode = GetLanguageCode();
                if (Translations != null && Translations.ContainsKey(langCode))
                {
                    return Translations[langCode];
                }
                else
                {
                    return Body;
                }
            }
        }

        public void Update(FizzMessageCellModel model)
        {
            Id = model.Id;
            From = model.From;
            To = model.To;
            Nick = model.Nick;
            Body = model.Body;
            Topic = model.Topic;
            Data = model.Data;
            Translations = model.Translations;
            Created = model.Created;

            AlternateId = model.AlternateId;
            DeliveryState = model.DeliveryState;
            TranslationState = model.TranslationState;
        }

        private string GetLanguageCode()
        {
            string langCode = FizzLanguageCodes.English.Code;
            try
            {
                langCode = FizzService.Instance.Language.Code;
            }
            catch (Exception)
            {
                FizzLogger.E("Unable to get LanguageCode");
            }

            return langCode;
        }
    }

    public enum FizzChatCellType
    {
        ChatCell,
        DateCell
    }

    public enum FizzChatCellTranslationState
    {
        Original,
        Translated
    }

    public enum FizzChatCellDeliveryState
    {
        Pending = 1,
        Sent = 2,
        Published = 3
    }
}