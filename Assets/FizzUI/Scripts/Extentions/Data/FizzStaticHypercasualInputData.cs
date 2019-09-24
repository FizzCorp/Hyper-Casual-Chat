using Fizz.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fizz.UI.Extentions
{
    [CreateAssetMenu (menuName = "Fizz/HypercasualInputData")]
    public class FizzStaticHypercasualInputData : ScriptableObject
    {
        public List<FizzHypercasualPhraseDataItem> Phrases;
        public List<FizzHypercasualStickerDataItem> Stickers;

        static FizzStaticHypercasualInputData _instance = null;
        public static FizzStaticHypercasualInputData Instance
        {
            get
            {
                if (!_instance)
                {
                    FizzStaticHypercasualInputData[] list = Resources.FindObjectsOfTypeAll<FizzStaticHypercasualInputData> ();
                    if (list.Length > 0)
                    {
                        _instance = list.FirstOrDefault ();
                    }
                    else
                    {
                        _instance = Resources.Load<FizzStaticHypercasualInputData> ("FizzHypercasualInputData");
                    }

                    if (_instance == null) FizzLogger.E ("Unable to find FizzStaticHypercasualInputData. It should be placed under Resource directory.");
                }
                return _instance;
            }
        }
    }

    [Serializable]
    public class FizzHypercasualDataItem
    {
        public string Id;
        public string Tag;
    }

    [Serializable]
    public class FizzHypercasualPhraseDataItem : FizzHypercasualDataItem
    {
        public List<FizzHypercasualLocalizePhrase> LocalizedPhrases;

        public string GetLocalizedContent (SystemLanguage lang)
        {
            foreach (FizzHypercasualLocalizePhrase locPh in LocalizedPhrases)
            {
                if (locPh.Language.Equals (lang)) return locPh.Content;
            }
            return (LocalizedPhrases.Count > 0) ? LocalizedPhrases[0].Content : string.Empty;
        }
    }

    [Serializable]
    public class FizzHypercasualLocalizePhrase
    {
        public string Content;
        public SystemLanguage Language;
    }

    [Serializable]
    public class FizzHypercasualStickerDataItem : FizzHypercasualDataItem
    {
        public Sprite Content;
    }
}