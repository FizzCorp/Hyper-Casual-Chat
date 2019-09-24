using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

using Fizz.Common;
using Fizz.Common.Json;

namespace Fizz.Ingestion.Impl
{
    public class FizzIngestionClient : IFizzIngestionClient
    {
        static readonly int LOG_FLUSH_INTERVAL = 5 * 1000;
        static readonly int EVENT_VER = 1;

#if UNITY_ANDROID
        static readonly string PLATFORM = "android";
#elif UNITY_IOS
        static readonly string PLATFORM = "ios";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        static readonly string PLATFORM = "mac_osx";
#else
        static readonly string PLATFORM = "windows";
#endif
        static readonly FizzException ERROR_INVALID_CLIENT = new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_client");

        IFizzAuthRestClient _client;
        string _userId;
        long _timeOffset;
        long _startTime;
        string _sessionId;
        IFizzEventLog _eventLog;
        IFizzActionDispatcher _dispatcher;
        Action _onLogEmpty;
        bool _flushInProgress = false;
        readonly FizzInterval _interval = null;

        public string BuildVer { get; set; }
        public string CustomDimesion01 { get; set; }
        public string CustomDimesion02 { get; set; }
        public string CustomDimesion03 { get; set; }

        private const string KEY_SESSION = "fizz_session";
        private const string KEY_SESSION_START_TS = "fizz_session_start_ts";
        private const string KEY_SESSION_UPDATE_TS = "fizz_session_update_ts";

        public FizzIngestionClient(IFizzEventLog eventLog, IFizzActionDispatcher dispatcher)
        {
            if (eventLog == null)
            {
                throw new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_event_log");
            }
            if (dispatcher == null)
            {
                throw new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_dispatcher");
            }

            _eventLog = eventLog;
            _dispatcher = dispatcher;
            _interval = new FizzInterval(_dispatcher, Flush, LOG_FLUSH_INTERVAL);
        }

        public void Open(string userId, long curServerTS, IFizzAuthRestClient client)
        {
            IfClosed(() =>
           {
               if (_userId != null)
               {
                   FizzLogger.W("Please close instance before re-opening");
                   return;
               }

               if (userId == null)
               {
                   throw FizzException.ERROR_INVALID_USER_ID;
               }
               if (client == null)
               {
                   throw ERROR_INVALID_CLIENT;
               }

               _client = client;
               _userId = userId;
               _timeOffset = FizzUtils.Now() - curServerTS;
               _startTime = FizzUtils.Now() + _timeOffset;
               _sessionId = Guid.NewGuid().ToString();

               ClosePreviousSession();
               SessionStarted();

               _interval.Enable();
               _onLogEmpty = () => { };
           });
        }

        public void Close(Action callback)
        {
            IfOpened(() =>
            {
                _interval.Disable();
                UpdateSessionTimestamp ();
                //SessionEnded(_startTime, FizzUtils.Now() + _timeOffset);

                _onLogEmpty = () =>
                {
                    _userId = null;
                    _client = null;
                    _sessionId = null;
                    _onLogEmpty = null;
                    callback();
                };
                
                Flush();
            });
        }

        public void ProductPurchased(string productId, double amount, string currency, string receipt)
        {
            IfOpened(() => 
            {
                FizzLogger.D("Product Purchased => id:" + productId + " amount:" + amount + " currency:" + currency + " receipt:" + receipt);

                JSONClass fields = new JSONClass();

                if (productId != null)
                {
                    fields["product_id"] = productId;
                }
                if (currency != null)
                {
                    fields["currency"] = currency;
                }
                if (receipt != null) 
                {
                    fields["receipt"] = receipt;
                }
                fields["amount"].AsDouble = amount;

                _eventLog.Put(BuildEvent(_sessionId, FizzEventType.product_purchased, FizzUtils.Now () + _timeOffset, fields)); 
            });
        }

        public void TextMessageSent(string channelId, string content, string senderNick)
        {
            IfOpened(() =>
            {
                FizzLogger.D("Text Message Sent => channel:" + channelId + " content:" + content + " nick:" + senderNick);

                JSONClass fields = new JSONClass();

                if (channelId != null)
                {
                    fields["channel_id"] = channelId;
                }
                if (content != null)
                {
                    fields["content"] = content;
                }
                if (senderNick != null)
                {
                    fields["nick"] = senderNick;
                }

                _eventLog.Put(BuildEvent(_sessionId, FizzEventType.text_msg_sent, FizzUtils.Now () + _timeOffset, fields)); 
            });
        }

        private void ClosePreviousSession ()
        {
            string lastSessionId = UnityEngine.PlayerPrefs.GetString (KEY_SESSION, string.Empty);
            if (!string.IsNullOrEmpty (lastSessionId)) 
            {
                long sessionStartTs = FizzUtils.Now () + _timeOffset;
                long sessionUpdateTs = FizzUtils.Now () + _timeOffset;
                long.TryParse (UnityEngine.PlayerPrefs.GetString (KEY_SESSION_START_TS), out sessionStartTs);
                long.TryParse (UnityEngine.PlayerPrefs.GetString (KEY_SESSION_UPDATE_TS), out sessionUpdateTs);

                SessionEnded (lastSessionId, sessionStartTs, sessionUpdateTs);

                UnityEngine.PlayerPrefs.DeleteKey (KEY_SESSION);
                UnityEngine.PlayerPrefs.DeleteKey (KEY_SESSION_START_TS);
                UnityEngine.PlayerPrefs.DeleteKey (KEY_SESSION_UPDATE_TS);
                UnityEngine.PlayerPrefs.Save ();
            }
        }

        private void SessionStarted()
        {
            FizzLogger.D("Session Started");
            
            //Persist Session Start
            UnityEngine.PlayerPrefs.SetString (KEY_SESSION, _sessionId);
            UnityEngine.PlayerPrefs.SetString (KEY_SESSION_START_TS, (_startTime).ToString ());
            UnityEngine.PlayerPrefs.Save ();

            _eventLog.Put(BuildEvent(_sessionId, FizzEventType.session_started, _startTime, null));   
        }

        private void SessionEnded(string sessionId, long sessionStartTime, long sessionEndTime)
        {
            FizzLogger.D("Session Ended");

            JSONClass fields = new JSONClass();

            long duration = (sessionEndTime - sessionStartTime) / 1000;

            fields["duration"].AsDouble = duration;
            
            _eventLog.Put(BuildEvent(sessionId, FizzEventType.session_ended, sessionEndTime, fields));
        }

        private void UpdateSessionTimestamp ()
        {
            UnityEngine.PlayerPrefs.SetString (KEY_SESSION_UPDATE_TS, (FizzUtils.Now () + _timeOffset).ToString ());
            UnityEngine.PlayerPrefs.Save ();
        }

        private FizzEvent BuildEvent(string sessionId, FizzEventType type, long timestamp, JSONNode fields)
        {
            try
            {
                UnityEngine.PlayerPrefs.SetString (KEY_SESSION_UPDATE_TS, (FizzUtils.Now () + _timeOffset).ToString ());
                UnityEngine.PlayerPrefs.Save ();

                return new FizzEvent(
                    _userId,
                    type,
                    EVENT_VER,
                    sessionId,
                    timestamp,
                    PLATFORM,
                    BuildVer,
                    CustomDimesion01, CustomDimesion02, CustomDimesion03,
                    fields
                );
            }
            catch (Exception ex)
            {
                FizzLogger.E("Invalid event encountered: " + ex.Message);
                return null;
            }
        }

        private void IfOpened(Action onInit)
        {
            if (string.IsNullOrEmpty (_userId))
            {
                FizzLogger.W("Ingestion must be opened before use.");
            }
            else
            {
                onInit.Invoke();
            }
        }

        private void IfClosed(Action onClose)
        {
            if (string.IsNullOrEmpty (_userId))
            {
                onClose.Invoke();
            }
            else
            {
                FizzLogger.W("Ingestion is already opened");
            }
        }

        public void Flush()
        {
            if (_flushInProgress)
            {
                FizzUtils.DoCallback(_onLogEmpty);
                return;
            }

            _flushInProgress = true;
            _eventLog.Read(128, items =>
            {
                if (items.Count <= 0)
                {
                    _flushInProgress = false;
                    FizzUtils.DoCallback(_onLogEmpty);
                    return;
                }

                PostEvents(items, (response, ex) => 
                {
                    bool rollLog = true;

                    if (ex != null)
                    {
                        if (ex.Code == FizzError.ERROR_REQUEST_FAILED)
                        {
                            FizzLogger.W("Failed to submit events to service");
                            rollLog = false;
                        }
                        else
                        if (ex.Code == FizzError.ERROR_INVALID_REQUEST)
                        {
                            FizzLogger.E("Submission of some events failed: " + ex.Message);
                        }
                    }

                    if (rollLog)
                    {
                        _eventLog.RollTo(items[items.Count - 1]);
                    }

                    FizzUtils.DoCallback(_onLogEmpty);
                    _flushInProgress = false;
                });
            });
        }

        private void PostEvents(List<FizzEvent> events,
                                Action<string, FizzException> callback)
        {
            _client.Post(
                FizzConfig.API_BASE_URL,
                FizzConfig.API_PATH_EVENTS,
                ParseEvents(events),
                callback
            );
        }

        private string ParseEvents(List<FizzEvent> items)
        {
            JSONArray events = new JSONArray();

            foreach (FizzEvent item in items)
            {
                events.Add(ParseEvent(item));
            }
            
            return events.ToString();
        }

        private static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private JSONNode ParseEvent(FizzEvent item)
        {
            JSONClass json = new JSONClass();

            json["user_id"] = item.UserId;
            json["type"] = Enum.GetName(typeof(FizzEventType), item.Type);
            //json["ver"].AsInt = EVENT_VER;
            json.Add("ver", new JSONData(EVENT_VER));
            json["session_id"] = item.SessionId;
            json["time"].AsDouble = item.Time;

            if (item.Platform != null)
            {
                json["platform"] = item.Platform;
            }
            if (item.Build != null)
            {
                json["build"] = item.Build;
            }
            if (item.Custom01 != null)
            {
                json["custom_01"] = item.Custom01;
            }
            if (item.Custom02 != null)
            {
                json["custom_02"] = item.Custom02;
            }
            if (item.Custom03 != null)
            {
                json["custom_03"] = item.Custom03;
            }

            if (item.Fields != null) 
            {
                foreach (string key in item.Fields.Keys)
                {
                    json[key] = item.Fields[key];
                }   
            }

            return json;
        }
    }
}
