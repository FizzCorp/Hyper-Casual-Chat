using System;

namespace Fizz.Common
{
    public class FizzSessionRepository
    {
        private string _userId;
        private string _locale;
        private IFizzSessionProvider _sessionProvider;

        public FizzSession Session { get; private set; }

        public Action OnSessionUpdate;

        public FizzSessionRepository (string userId, string locale, IFizzSessionProvider sessionProvider)
        {
            _userId = userId;
            _locale = locale;
            _sessionProvider = sessionProvider;

            Session = new FizzSession (null, null, 0);
        }

        public void FetchToken (Action<FizzSession, FizzException> callback)
        {
            _sessionProvider.FetchToken(_userId, _locale, (session, exception) => {
                if (exception == null)
                {
                    Session = session;

                    FizzUtils.DoCallback(OnSessionUpdate);
                }

                FizzUtils.DoCallback<FizzSession>(session, exception, callback);
            });
        }
    }
}
