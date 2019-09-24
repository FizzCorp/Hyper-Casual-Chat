using System.Collections.Generic;

namespace Fizz.UI.Core
{
    public class LocalizationService : IServiceLocalization
    {
        private string m_language = string.Empty;
        private Dictionary<string, Dictionary<string, string>> m_resources = null;

        public LocalizationService ()
        {
            InitLangugageResources ();
        }

        private void InitLangugageResources ()
        {
            m_language = "en";
            DefaultLanguageResources ();
        }

        public override string GetText (string id)
        {
            if (m_resources[m_language].ContainsKey (id))
            {
                return m_resources[m_language][id];
            }
            return id;
        }

        public override string Language
        {
            get
            {
                return m_language;
            }
            set
            {
                m_language = value;
            }
        }

        public override string this[string id]
        {
            get
            {
                return GetText (id);
            }
        }

        private void DefaultLanguageResources ()
        {
            m_resources = new Dictionary<string, Dictionary<string, string>>
            {
                { "en", new Dictionary<string, string>
                    {
                        { "Message_PlaceHolderTypeMsg","Write a message..." },
                        { "DateFormat_Yesterday", "Yesterday" },
                        { "DateFormat_Today", "Today" },
                        { "Channels_Group", "Group" },
                        { "Channels_Title", "Channels" },
                        { "General_SocketConnecting","Connecting..." },
                        { "General_SocketConnected", "Connected" }
                    }
                }
            };
        }
    }
}