using System;

namespace Fizz.Common
{
    public interface IFizzSessionProvider
    {
        void FetchToken (string userId, string locale, Action<FizzSession, FizzException> callback);
    }
}