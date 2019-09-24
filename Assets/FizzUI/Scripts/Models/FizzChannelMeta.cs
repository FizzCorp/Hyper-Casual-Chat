namespace Fizz.UI.Model
{
    public class FizzChannelMeta
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Group { get; set; }

        public bool PersistMessages { get; set; }

        public bool FilterContent { get; set; }

        public bool Readonly { get; set; }

        public int InitialQueryMessageCount { get; set; }

        public int HistoryQueryMessageCount { get; set; }

        public FizzChannelMeta () { }

        public FizzChannelMeta(string id, string name)
        {
            Id = id;
            Name = name;
            PersistMessages = true;
            FilterContent = true;
            Readonly = false;
            InitialQueryMessageCount = 50;
            HistoryQueryMessageCount = 50;
        }

        public FizzChannelMeta (string id, string name, string group)
        {
            Id = id;
            Name = name;
            Group = group;

            PersistMessages = true;
            FilterContent = true;
            Readonly = false;
            InitialQueryMessageCount = 50;
            HistoryQueryMessageCount = 50;
        }

        public FizzChannelMeta (string id, string name, bool persist, int initialMessages, int historyMessages)
        {
            Id = id;
            Name = name;
            PersistMessages = persist;
            InitialQueryMessageCount = initialMessages;
            HistoryQueryMessageCount = historyMessages;

            FilterContent = true;
            Readonly = false;
        }
    }
}