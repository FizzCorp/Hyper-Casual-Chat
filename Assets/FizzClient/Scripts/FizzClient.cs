using System;

namespace Fizz
{
    using Chat;
    using Chat.Impl;
    using Common;
    using Ingestion;
    using Ingestion.Impl;
    using Moderation;
    using Moderation.Impl;
    using Threading;

    public enum FizzClientState
    {
        Closed,
        Opened
    }

    [Flags]
    public enum FizzServices
    {
        Chat = 1 << 0,
        Analytics = 1 << 1,
        Moderation = 1 << 2,
        All = Chat | Analytics | Moderation
    }

    public interface IFizzClient
    { 
        IFizzChatClient Chat { get; }

        IFizzIngestionClient Ingestion { get; }

        IFizzModerationClient Moderation { get; }

        FizzClientState State { get; }

        string Version { get; }

        void Open(string userId, IFizzLanguageCode language, FizzServices services, Action<FizzException> callback);

        void Close(Action<FizzException> callback);

        void Update();
    }

    public class FizzClient : IFizzClient
    {
        private readonly FizzChatClient _chat;
        private readonly FizzIngestionClient _ingestionClient;
        private readonly FizzModerationClient _moderationClient;
        private readonly FizzActionDispatcher _dispatcher;
        private readonly IFizzRestClient _restClient;
        private readonly IFizzAuthRestClient _authClient;
        private readonly IFizzSessionProvider _sessionClient;

        public FizzClient(string appId, string appSecret)
        {
            if (string.IsNullOrEmpty(appId))
            {
                throw FizzException.ERROR_INVALID_APP_ID;
            }

            if (string.IsNullOrEmpty(appSecret))
            {
                throw FizzException.ERROR_INVALID_APP_SECRET;
            }

            _dispatcher = new FizzActionDispatcher();
            _chat = new FizzChatClient(appId, _dispatcher);
            _restClient = new FizzRestClient(_dispatcher);
            _sessionClient = new FizzIdSecretSessionProvider(appId, appSecret, _restClient);
            _authClient = new FizzAuthRestClient(_restClient);
            _ingestionClient = new FizzIngestionClient(new FizzInMemoryEventLog(), _dispatcher);
            _moderationClient = new FizzModerationClient();
        }

        public FizzClient(string appId, IFizzSessionProvider sessionClient)
        {
            if (string.IsNullOrEmpty(appId))
            {
                throw FizzException.ERROR_INVALID_APP_ID;
            }

            _dispatcher = new FizzActionDispatcher();
            _sessionClient = sessionClient;
            _chat = new FizzChatClient(appId, _dispatcher);
            _restClient = new FizzRestClient(_dispatcher);
            _authClient = new FizzAuthRestClient(_restClient);
            _ingestionClient = new FizzIngestionClient(new FizzInMemoryEventLog(), _dispatcher);
            _moderationClient = new FizzModerationClient();
        }

        public IFizzChatClient Chat
        {
            get
            {
                return _chat;
            }
        }

        public IFizzIngestionClient Ingestion
        {
            get
            {
                return _ingestionClient;
            }
        }

        public IFizzModerationClient Moderation
        {
            get
            {
                return _moderationClient;
            }
        }

        public FizzClientState State { get; private set; }

        public string Version { get { return "v1.5.4"; } }

        public void Close(Action<FizzException> callback)
        {
            try
            {
                if (State == FizzClientState.Closed)
                {
                    FizzUtils.DoCallback(null, callback);
                    return;
                }

                Close(() => { FizzUtils.DoCallback(null, callback); });
            }
            catch (FizzException ex)
            {
                FizzUtils.DoCallback(ex, callback);
            }
        }

        public void Open(string userId, IFizzLanguageCode locale, FizzServices services, Action<FizzException> callback)
        {
            try
            {
                if (State == FizzClientState.Opened)
                    return;

                FizzSessionRepository sessionRepo = new FizzSessionRepository(userId, locale.Code, _sessionClient);
                _authClient.Open(sessionRepo, ex =>
                {
                    if (ex == null)
                    {
                        if (services.HasFlag(FizzServices.Chat))
                        {
                            _chat.Open(userId, _authClient, sessionRepo);
                        }
                        if (services.HasFlag(FizzServices.Analytics))
                        {
                            _ingestionClient.Open(userId, sessionRepo.Session._serverTS, _authClient);
                        }
                        if (services.HasFlag(FizzServices.Moderation))
                        {
                            _moderationClient.Open(_authClient);
                        }

                        State = FizzClientState.Opened;
                        FizzUtils.DoCallback(null, callback);
                    }
                    else
                    {
                        FizzUtils.DoCallback(ex, callback);
                    }
                });
            }
            catch (FizzException ex)
            {
                FizzUtils.DoCallback(ex, callback);
            }
        }
        public void Update()
        {
            _dispatcher.Process();
        }
        private void Close(Action callback)
        {
            _ingestionClient.Close(() =>
            {
                _authClient.Close();
                _chat.Close(callback);
                _moderationClient.Close();
                State = FizzClientState.Closed;
            });
        }
    }
}