using Fizz.Common;
using System;
using System.Collections.Generic;

namespace Fizz.Moderation
{
    public interface IFizzModerationClient
    {
        void SanitizeText (IList<string> texts, Action<IList<string>, FizzException> callback);
        void ReportMessage (string channelId, string message, string messageId, IFizzLanguageCode lang, string userId, string offense, string description, Action<string, FizzException> callback);
    }
}
