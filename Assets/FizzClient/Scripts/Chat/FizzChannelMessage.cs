using System.Collections.Generic;

namespace Fizz.Chat
{
    public class FizzChannelMessage
    {
        public FizzChannelMessage(long id,
                                  string from,
                                  string nick,
                                  string to,
                                  string body,
                                  Dictionary<string, string> data,
                                  IDictionary<string,string> translations,
                                  long created) {
            if (from == null)
            {
                throw new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_message_from");
            }
            if (to == null)
            {
                throw new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_message_to");
            }

            Id = id;
            From = from;
            Nick = nick;
            To = to;
            Body = body == null ? "" : body;
            Data = data;
            Translations = translations;
            Created = created;
        }

        public long Id { get; protected set; }

        public string From { get; protected set; }

        public string Nick { get; protected set; }

        public string To { get; protected set; }

        public string Body { get; protected set; }

        public string Topic { get; protected set; }

        public Dictionary<string, string> Data { get; protected set; }

        public IDictionary<string, string> Translations { get; protected set; }

        public long Created { get; protected set; }

        protected FizzChannelMessage() {}
    }
}
