using System.Collections.Generic;

namespace Fizz.UI
{
    using Extentions;

    public interface IFizzHypercasualInputDataProvider
    {
        List<string> GetAllTags ();
        List<string> GetAllPhrases (string tag);
        List<string> GetAllStickers (string tag);

        FizzHypercasualPhraseDataItem GetPhrase (string id);
        FizzHypercasualStickerDataItem GetSticker (string id);

        List<string> GetRecentPhrases ();
        List<string> GetRecentStickers ();

        void AddPhraseToRecent (string id);
        void AddStickerToRecent (string id);
    }
}