using System.Collections.Generic;
using UnityEngine;

namespace Fizz.UI.Extentions
{
    public class FizzStaticHypercasualInputDataProvider : IFizzHypercasualInputDataProvider
    {
        public List<string> GetAllPhrases (string tag)
        {
            List<string> phrases = new List<string> ();

            if (FizzStaticHypercasualInputData.Instance == null) return phrases;

            foreach (FizzHypercasualDataItem dataItem in FizzStaticHypercasualInputData.Instance.Phrases)
            {
                if (dataItem.Tag.Equals (tag))
                    phrases.Add (dataItem.Id);
            }
            return phrases;
        }

        public List<string> GetAllStickers (string tag)
        {
            List<string> stickers = new List<string> ();

            if (FizzStaticHypercasualInputData.Instance == null) return stickers;

            foreach (FizzHypercasualDataItem dataItem in FizzStaticHypercasualInputData.Instance.Stickers)
            {
                if (dataItem.Tag.Equals (tag))
                    stickers.Add (dataItem.Id);
            }
            return stickers;
        }

        public List<string> GetAllTags ()
        {
            List<string> tags = new List<string> ();

            if (FizzStaticHypercasualInputData.Instance == null) return tags;

            foreach (FizzHypercasualDataItem dataItem in FizzStaticHypercasualInputData.Instance.Phrases)
            {
                if (tags.Contains (dataItem.Tag)) continue;
                tags.Add (dataItem.Tag);
            }
            foreach (FizzHypercasualDataItem dataItem in FizzStaticHypercasualInputData.Instance.Stickers)
            {
                if (tags.Contains (dataItem.Tag)) continue;
                tags.Add (dataItem.Tag);
            }
            return tags;
        }

        public FizzHypercasualPhraseDataItem GetPhrase (string id)
        {
            if (FizzStaticHypercasualInputData.Instance == null) return null;
            
            foreach (FizzHypercasualPhraseDataItem dataItem in FizzStaticHypercasualInputData.Instance.Phrases)
            {
                if (dataItem.Id.Equals (id))
                    return dataItem;
            }
            return null;
        }

        public FizzHypercasualStickerDataItem GetSticker (string id)
        {
            if (FizzStaticHypercasualInputData.Instance == null) return null;

            foreach (FizzHypercasualStickerDataItem dataItem in FizzStaticHypercasualInputData.Instance.Stickers)
            {
                if (dataItem.Id.Equals (id))
                    return dataItem;
            }
            return null;
        }

        public List<string> GetRecentPhrases ()
        {
#if UNITY_EDITOR
            if (recentPhrases == null) recentPhrases = new List<string> ();
            return recentPhrases;
#else
            string ids = PlayerPrefs.GetString ("fizz_cached_recent_phrases", string.Empty);
            return new List<string> (ids.Split (';'));
#endif
        }

        public List<string> GetRecentStickers ()
        {
#if UNITY_EDITOR
            if (recentStickers == null) recentStickers = new List<string> ();
            return recentStickers;
#else
            string ids = PlayerPrefs.GetString ("fizz_cached_recent_stickers", string.Empty);
            return new List<string> (ids.Split (';'));
#endif
        }

        public void AddPhraseToRecent (string id)
        {
#if UNITY_EDITOR
            if (recentPhrases == null) recentPhrases = new List<string> (9);
            if (recentPhrases.Contains (id)) return;
            if (recentPhrases.Count >= 9) recentPhrases.RemoveAt (recentPhrases.Count - 1);
            recentPhrases.Insert (0, id);
#else
            string ids = PlayerPrefs.GetString ("fizz_cached_recent_phrases", string.Empty);
            if (ids.Contains (id)) return;
            if (ids.Split (';').Length >= 9) ids = ids.Substring (0, ids.LastIndexOf (';') + 1);

            ids = id + ";" + ids;
            
            PlayerPrefs.SetString ("fizz_cached_recent_phrases", ids);
            PlayerPrefs.Save ();
#endif
        }

        public void AddStickerToRecent (string id)
        {
#if UNITY_EDITOR
            if (recentStickers == null) recentStickers = new List<string> (5);
            if (recentStickers.Contains (id)) return;
            if (recentStickers.Count >= 5) recentStickers.RemoveAt (recentStickers.Count - 1);
            recentStickers.Insert (0, id);
#else
            string ids = PlayerPrefs.GetString ("fizz_cached_recent_stickers", string.Empty);
            if (ids.Contains (id)) return;
            if (ids.Split (';').Length >= 5) ids = ids.Substring (0, ids.LastIndexOf (';') + 1);

            ids = id + ";" + ids;
            
            PlayerPrefs.SetString ("fizz_cached_recent_stickers", ids);
            PlayerPrefs.Save ();
#endif
        }

#if UNITY_EDITOR
        private List<string> recentPhrases;
        private List<string> recentStickers;
#endif
    }
}