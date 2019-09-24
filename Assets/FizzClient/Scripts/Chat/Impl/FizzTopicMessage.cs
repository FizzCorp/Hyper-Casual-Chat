using System;

using Fizz.Common.Json;

namespace Fizz.Chat.Impl
{
    public class FizzTopicMessage
    {
        public static readonly string KEY_ID = "id";
        public static readonly string KEY_TYPE = "type";
        public static readonly string KEY_FROM = "from";
        public static readonly string KEY_DATA = "data";
        public static readonly string KEY_CREATED = "created";

        public FizzTopicMessage(string json)
        {
            JSONNode data = JSONNode.Parse(json);
            Id = (long)data[KEY_ID].AsDouble;
            Type = data[KEY_TYPE];
            From = data[KEY_FROM];
            Data = data[KEY_DATA];
            Created = (long)data[KEY_CREATED].AsDouble;
        }

        public long Id { get; private set; }
        public string Type { get; private set; }
        public string From { get; private set; }
        public string Data { get; private set; }
        public long Created { get; private set; }
    }
}
