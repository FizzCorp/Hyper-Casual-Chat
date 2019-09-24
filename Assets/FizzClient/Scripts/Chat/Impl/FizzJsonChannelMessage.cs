using System.Collections.Generic;
using Fizz.Common.Json;

namespace Fizz.Chat.Impl
{
    public class FizzJsonChannelMessage: FizzChannelMessage
    {
        private static readonly string KEY_ID = "id";
        public static readonly string KEY_FROM = "from";
        public static readonly string KEY_NICK = "nick";
        public static readonly string KEY_TO = "to";
        public static readonly string KEY_BODY = "body";
        public static readonly string KEY_TOPIC = "topic";
        public static readonly string KEY_DATA = "data";
        public static readonly string KEY_TRANSLATE = "translate";
        public static readonly string KEY_PERSIST = "persist";
        public static readonly string KEY_FILTER = "filter";
        public static readonly string KEY_TRANSLATIONS = "translations";
        public static readonly string KEY_CREATED = "created";

        public FizzJsonChannelMessage(string json): this(JSONNode.Parse(json))
        {
        }

        public FizzJsonChannelMessage(JSONNode json)
        {
            Id = (long)json[KEY_ID].AsDouble;
            From = json[KEY_FROM];
            Nick = json[KEY_NICK];
            To = json[KEY_TO];
            Body = json[KEY_BODY];
            Topic = json[KEY_TOPIC];
            Created = (long)json[KEY_CREATED].AsDouble;
            
            if (json[KEY_DATA] != null)
            {
                JSONNode messageData = JSONNode.Parse(json[KEY_DATA]);
                if (messageData != null)
                {
                    Data = new Dictionary<string, string>();
                    foreach (string key in messageData.Keys)
                    {
                        Data.Add(key, messageData[key]);
                    }
                }
            }

            if (json[KEY_TRANSLATIONS] != null)
            {
                JSONClass translationData = json[KEY_TRANSLATIONS].AsObject;
                Translations = new Dictionary<string, string>();
                foreach (string key in translationData.Keys)
                {
                    Translations.Add(key, translationData[key]);
                }
            }
        }
    }
}
