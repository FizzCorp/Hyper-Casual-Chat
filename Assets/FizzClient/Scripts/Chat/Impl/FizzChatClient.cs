using System;
using System.Collections.Generic;

using Fizz.Common;
using Fizz.Common.Json;

namespace Fizz.Chat.Impl
{
    public class FizzChatClient : IFizzChatClient
    {
        private static readonly FizzException ERROR_INVALID_DISPATCHER = new FizzException (FizzError.ERROR_BAD_ARGUMENT, "invalid_dispatcher");
        private static readonly FizzException ERROR_INVALID_REST_CLIENT = new FizzException (FizzError.ERROR_BAD_ARGUMENT, "invalid_rest_client");
        private static readonly FizzException ERROR_INVALID_CHANNEL = new FizzException (FizzError.ERROR_BAD_ARGUMENT, "invalid_channel_id");
        private static readonly FizzException ERROR_INVALID_USER = new FizzException (FizzError.ERROR_BAD_ARGUMENT, "invalid_user_id");
        private static readonly FizzException ERROR_INVALID_MESSAGE_ID = new FizzException (FizzError.ERROR_BAD_ARGUMENT, "invalid_message_id");
        private static readonly FizzException ERROR_INVALID_RESPONSE_FORMAT = new FizzException (FizzError.ERROR_REQUEST_FAILED, "invalid_response_format");
        private static readonly FizzException ERROR_INVALID_MESSAGE_QUERY_COUNT = new FizzException (FizzError.ERROR_BAD_ARGUMENT, "invalid_query_count");

        private readonly IFizzActionDispatcher _dispatcher;
        private readonly FizzMQTTChannelMessageListener _messageListener;

        private string _userId;
        private IFizzAuthRestClient _restClient;
        private readonly FizzSessionRepository sessionRepository;

        public IFizzChannelMessageListener Listener
        {
            get
            {
                return _messageListener;
            }
        }

		public bool IsConnected 
        {
            get 
            {
                if (_messageListener == null) return false;
                return _messageListener.IsConnected;
            }
        }
        public FizzChatClient (string appId, IFizzActionDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw ERROR_INVALID_DISPATCHER;
            }

            _dispatcher = dispatcher;

            _messageListener = CreateListener (appId, _dispatcher);
        }

        public void Open (string userId, IFizzAuthRestClient client, FizzSessionRepository sessionRepository)
        {
            IfClosed (() =>
            {
                if (string.IsNullOrEmpty (userId))
                {
                    throw FizzException.ERROR_INVALID_USER_ID;
                }
                if (client == null)
                {
                    throw ERROR_INVALID_REST_CLIENT;
                }
                if (sessionRepository == null)
                {
                    throw FizzException.ERROR_INVALID_SESSION_REPOSITORY;
                }

                if (_userId == userId)
                {
                    return;
                }

                _userId = userId;
                _restClient = client;

                _messageListener.Open (userId, sessionRepository);
            });
        }

        public void Close (Action cb)
        {
            IfOpened (() =>
            {
                _userId = null;
                _restClient = null;

                _messageListener.Close (cb);
            });
        }

        public void PublishMessage (
            string channelId,
            string nick,
            string body,
            Dictionary<string, string> data,
            bool translate,
            bool filter,
            bool persist,
            Action<FizzException> callback)
        {
            IfOpened (() =>
            {
                if (string.IsNullOrEmpty (channelId))
                {
                    FizzUtils.DoCallback (ERROR_INVALID_CHANNEL, callback);
                    return;
                }

                try
                {
                    string path = string.Format (FizzConfig.API_PATH_MESSAGES, channelId);
                    JSONClass json = new JSONClass ();
                    json[FizzJsonChannelMessage.KEY_NICK] = nick;
                    json[FizzJsonChannelMessage.KEY_BODY] = body;
                    json[FizzJsonChannelMessage.KEY_PERSIST].AsBool = persist;
                    json[FizzJsonChannelMessage.KEY_FILTER].AsBool = filter;
                    json[FizzJsonChannelMessage.KEY_TRANSLATE].AsBool = translate;

                    string dataStr = string.Empty;
                    if (data != null)
                    {
                        JSONClass dataJson = new JSONClass();
                        foreach (KeyValuePair<string, string> pair in data)
                        {
                            dataJson.Add(pair.Key, new JSONData(pair.Value));
                        }

                        dataStr = dataJson.ToString();
                    }

                    json[FizzJsonChannelMessage.KEY_DATA] = dataStr;

                    _restClient.Post (FizzConfig.API_BASE_URL, path, json.ToString (), (response, ex) =>
                    {
                        FizzUtils.DoCallback (ex, callback);
                    });
                }
                catch (FizzException ex)
                {
                    FizzUtils.DoCallback (ex, callback);
                }
            });
        }

        public void UpdateMessage (
            string channelId,
            long messageId,
            string nick,
            string body,
            Dictionary<string, string> data,
            bool translate,
            bool filter,
            bool persist,
            Action<FizzException> callback)
        {
            IfOpened (() =>
            {
                if (string.IsNullOrEmpty (channelId))
                {
                    FizzUtils.DoCallback (ERROR_INVALID_CHANNEL, callback);
                    return;
                }

                if (messageId < 1)
                {
                    FizzUtils.DoCallback (ERROR_INVALID_MESSAGE_ID, callback);
                    return;
                }

                try
                {
                    string path = string.Format (FizzConfig.API_PATH_MESSAGE_ACTION, channelId, messageId);
                    JSONClass json = new JSONClass ();
                    json[FizzJsonChannelMessage.KEY_NICK] = nick;
                    json[FizzJsonChannelMessage.KEY_BODY] = body;
                    json[FizzJsonChannelMessage.KEY_PERSIST].AsBool = persist;
                    json[FizzJsonChannelMessage.KEY_FILTER].AsBool = filter;
                    json[FizzJsonChannelMessage.KEY_TRANSLATE].AsBool = translate;

                    string dataStr = string.Empty;
                    if (data != null)
                    {
                        JSONClass dataJson = new JSONClass();
                        foreach (KeyValuePair<string, string> pair in data)
                        {
                            dataJson.Add(pair.Key, new JSONData(pair.Value));
                        }

                        dataStr = dataJson.ToString();
                    }

                    json[FizzJsonChannelMessage.KEY_DATA] = dataStr;

                    _restClient.Post (FizzConfig.API_BASE_URL, path, json.ToString (), (response, ex) =>
                    {
                        FizzUtils.DoCallback (ex, callback);
                    });
                }
                catch (FizzException ex)
                {
                    FizzUtils.DoCallback (ex, callback);
                }
            });
        }

        public void DeleteMessage (string channelId, long messageId, Action<FizzException> callback)
        {
            IfOpened (() => 
            {
                if (string.IsNullOrEmpty (channelId))
                {
                    FizzUtils.DoCallback (ERROR_INVALID_CHANNEL, callback);
                    return;
                }

                if (messageId < 1)
                {
                    FizzUtils.DoCallback (ERROR_INVALID_MESSAGE_ID, callback);
                    return;
                }

                try 
                {
                    string path = string.Format (FizzConfig.API_PATH_MESSAGE_ACTION, channelId, messageId);
                    _restClient.Delete (FizzConfig.API_BASE_URL, path, string.Empty, (response, ex) => {
                        FizzUtils.DoCallback (ex, callback);
                    });
                } 
                catch (FizzException ex)
                {
                    FizzUtils.DoCallback (ex, callback);
                }
            });
        }

        public void Subscribe (string channel, Action<FizzException> callback)
        {
            IfOpened (() =>
            {
                if (channel == null || string.IsNullOrEmpty (channel))
                {
                    FizzUtils.DoCallback (ERROR_INVALID_CHANNEL, callback);
                    return;
                }

                string path = string.Format (FizzConfig.API_PATH_SUBSCRIBERS, channel);

                _restClient.Post (FizzConfig.API_BASE_URL, path, "", (resp, ex) =>
                {
                    FizzUtils.DoCallback (ex, callback);
                });
            });
        }

        public void Unsubscribe (string channel, Action<FizzException> callback)
        {
            IfOpened (() =>
            {
                if (channel == null || string.IsNullOrEmpty (channel))
                {
                    FizzUtils.DoCallback (ERROR_INVALID_CHANNEL, callback);
                    return;
                }

                string path = string.Format (FizzConfig.API_PATH_SUBSCRIBERS, channel);

                _restClient.Delete (FizzConfig.API_BASE_URL, path, "", (resp, ex) =>
                {
                    FizzUtils.DoCallback (ex, callback);
                });
            });
        }

        public void QueryLatest (string channel, int count, Action<IList<FizzChannelMessage>, FizzException> callback)
        {
            QueryLatest (channel, count, -1, callback);
        }

        public void QueryLatest (string channel, int count, long beforeId, Action<IList<FizzChannelMessage>, FizzException> callback)
        {
            IfOpened (() =>
            {
                if (string.IsNullOrEmpty (channel))
                {
                    FizzUtils.DoCallback<IList<FizzChannelMessage>> (null, ERROR_INVALID_CHANNEL, callback);
                    return;
                }
                if (count < 0)
                {
                    FizzUtils.DoCallback<IList<FizzChannelMessage>> (null, ERROR_INVALID_MESSAGE_QUERY_COUNT, callback);
                    return;
                }
                if (count == 0)
                {
                    FizzUtils.DoCallback<IList<FizzChannelMessage>> (new List<FizzChannelMessage> (), null, callback);
                    return;
                }

                string path = string.Format (FizzConfig.API_PATH_MESSAGES, channel) + "?count=" + count;
                if (beforeId > 0)
                {
                    path += "&before_id=" + beforeId;
                }
                _restClient.Get (FizzConfig.API_BASE_URL, path, (response, ex) =>
                {
                    if (ex != null)
                    {
                        FizzUtils.DoCallback<IList<FizzChannelMessage>> (null, ex, callback);
                    }
                    else
                    {
                        try
                        {
                            JSONArray messagesArr = JSONNode.Parse (response).AsArray;
                            IList<FizzChannelMessage> messages = new List<FizzChannelMessage> ();
                            foreach (JSONNode message in messagesArr.Childs)
                            {
                                messages.Add (new FizzJsonChannelMessage (message));
                            }
                            FizzUtils.DoCallback<IList<FizzChannelMessage>> (messages, null, callback);
                        }
                        catch
                        {
                            FizzUtils.DoCallback<IList<FizzChannelMessage>> (null, ERROR_INVALID_RESPONSE_FORMAT, callback);
                        }
                    }
                });
            });
        }

        public void Ban(string channel, string userId, Action<FizzException> callback)
        {
            IfOpened (() => {
                if (string.IsNullOrEmpty (channel))
                {
                    FizzUtils.DoCallback (ERROR_INVALID_CHANNEL, callback);
                    return;
                }
                if (string.IsNullOrEmpty (userId))
                {
                    FizzUtils.DoCallback (ERROR_INVALID_USER, callback);
                    return;
                }

                try
                {
                    string path = string.Format (FizzConfig.API_PATH_BAN, channel);
                    JSONClass json = new JSONClass ();
                    json["user_id"] = userId;

                    _restClient.Post (FizzConfig.API_BASE_URL, path, json.ToString (), (response, ex) => {
                        FizzUtils.DoCallback (ex, callback);
                    });
                }
                catch (FizzException ex)
                {
                    FizzUtils.DoCallback (ex, callback);
                }
            });
        }

        public void Unban(string channel, string userId, Action<FizzException> callback)
        {
            IfOpened (() => {
                if (string.IsNullOrEmpty (channel))
                {
                    FizzUtils.DoCallback (ERROR_INVALID_CHANNEL, callback);
                    return;
                }
                if (string.IsNullOrEmpty (userId))
                {
                    FizzUtils.DoCallback (ERROR_INVALID_USER, callback);
                    return;
                }

                try
                {
                    string path = string.Format (FizzConfig.API_PATH_BAN, channel);
                    JSONClass json = new JSONClass ();
                    json["user_id"] = userId;

                    _restClient.Delete (FizzConfig.API_BASE_URL, path, json.ToString (), (response, ex) => {
                        FizzUtils.DoCallback (ex, callback);
                    });
                }
                catch (FizzException ex)
                {
                    FizzUtils.DoCallback (ex, callback);
                }
            });
        }

        public void Mute(string channel, string userId, Action<FizzException> callback)
        {
            IfOpened (() => {
                if (string.IsNullOrEmpty (channel))
                {
                    FizzUtils.DoCallback (ERROR_INVALID_CHANNEL, callback);
                    return;
                }
                if (string.IsNullOrEmpty (userId))
                {
                    FizzUtils.DoCallback (ERROR_INVALID_USER, callback);
                    return;
                }

                try
                {
                    string path = string.Format (FizzConfig.API_PATH_MUTE, channel);
                    JSONClass json = new JSONClass ();
                    json["user_id"] = userId;

                    _restClient.Post (FizzConfig.API_BASE_URL, path, json.ToString (), (response, ex) => {
                        FizzUtils.DoCallback (ex, callback);
                    });
                }
                catch (FizzException ex)
                {
                    FizzUtils.DoCallback (ex, callback);
                }
            });
        }

        public void Unmute(string channel, string userId, Action<FizzException> callback)
        {
            IfOpened (() => {
                if (string.IsNullOrEmpty (channel))
                {
                    FizzUtils.DoCallback (ERROR_INVALID_CHANNEL, callback);
                    return;
                }
                if (string.IsNullOrEmpty (userId))
                {
                    FizzUtils.DoCallback (ERROR_INVALID_USER, callback);
                    return;
                }

                try
                {
                    string path = string.Format (FizzConfig.API_PATH_MUTE, channel);
                    JSONClass json = new JSONClass ();
                    json["user_id"] = userId;

                    _restClient.Delete (FizzConfig.API_BASE_URL, path, json.ToString (), (response, ex) => {
                        FizzUtils.DoCallback (ex, callback);
                    });
                }
                catch (FizzException ex)
                {
                    FizzUtils.DoCallback (ex, callback);
                }
            });
        }

        protected FizzMQTTChannelMessageListener CreateListener (string appId, IFizzActionDispatcher dispatcher)
        {
            return new FizzMQTTChannelMessageListener (appId, dispatcher);
        }

        private void IfOpened (Action callback)
        {
            if (!string.IsNullOrEmpty (_userId))
            {
                FizzUtils.DoCallback (callback);
            }
            else
            {
                FizzLogger.W ("Chat client should be opened before usage.");
            }
        }

        private void IfClosed (Action callback)
        {
            if (string.IsNullOrEmpty (_userId))
            {
                FizzUtils.DoCallback (callback);
            }
            else
            {
                FizzLogger.W ("Chat client should be closed before opening.");
            }
        }
    }
}