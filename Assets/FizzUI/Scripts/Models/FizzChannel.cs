using System;
using System.Collections.Generic;
using System.Linq;
using Fizz.Chat;
using Fizz.Common;

namespace Fizz.UI.Model
{
    public class FizzChannel
    {
        public string Id { get { return _channelMeta.Id; } }

        public string Name { get { return _channelMeta.Name; } }

        public FizzChannelMeta Meta { get { return _channelMeta; } }
        private FizzChannelMeta _channelMeta = null;

        public IList<FizzChannelMessage> Messages
        {
            get
            {
                if (!cached)
                {
                    cachedMessageList = _messageList.Values;
                    cached = true;
                }
                return cachedMessageList;
            }
        }

        public FizzChannel(FizzChannelMeta channelMeta)
        {
            this._channelMeta = channelMeta;
        }

        public void AddMessage(FizzChannelMessage message, bool notify = true)
        {
            if (_messageList.ContainsKey(message.Id))
                return;

            _messageList.Add(message.Id, message);
            cached = false;
        }

        public void RemoveMessage(FizzChannelMessage message)
        {
            if (_messageList.ContainsKey(message.Id))
            {
                _messageList.Remove(message.Id);
                cached = false;
            }
        }

        public void UpdateMessage(FizzChannelMessage message)
        {
            if (_messageList.ContainsKey(message.Id))
            {
                _messageList[message.Id] = message;
                cached = false;
            }
        }

        public void AddMessages(IList<FizzChannelMessage> messages)
        {
            foreach (FizzChannelMessage message in messages)
            {
                AddMessage(message, false);
            }
        }

        public void Subscribe(Action<FizzException> cb)
        {
            FizzService.Instance.Client.Chat.Subscribe(Id, ex =>
            {
                if (ex == null)
                {
                    if (FizzService.Instance.OnChannelSubscribed != null)
                    {
                        FizzService.Instance.OnChannelSubscribed.Invoke(Id);
                    }
                }

                FizzUtils.DoCallback(ex, cb);
            });
        }

        public void Unsubscribe(Action<FizzException> cb)
        {
            try
            {
                FizzService.Instance.Client.Chat.Unsubscribe(Id, ex =>
                {
                    if (ex == null)
                    {
                        FizzLogger.D("Unsubscribed " + Id);

                        if (FizzService.Instance.OnChannelUnsubscribed != null)
                        {
                            FizzService.Instance.OnChannelUnsubscribed.Invoke(Id);
                        }
                    }

                    FizzUtils.DoCallback(ex, cb);
                });
            }
            catch
            {
                FizzLogger.E("Something went wrong while calling Unsubscribe of FizzService.");
            }
        }

        public void SubscribeAndQuery()
        {
            try
            {
                Subscribe(subEx =>
                {
                    if (subEx != null)
                    {
                        FizzLogger.E("Subscribe Error " + Id + " ex " + subEx.Message);
                    }
                    else
                    {
                        FizzLogger.D("Subscribed " + Id);

                        FizzService.Instance.Client.Chat.QueryLatest(Id, _channelMeta.InitialQueryMessageCount, (msgs, qEx) =>
                        {
                            if (qEx == null)
                            {
                                Reset();

                                if (msgs != null && msgs.Count > 0)
                                {
                                    AddMessages(msgs);
                                }

                                if (FizzService.Instance.OnChannelMessagesAvailable != null)
                                {
                                    FizzService.Instance.OnChannelMessagesAvailable.Invoke(Id);
                                }
                            }
                            else
                            {
                                FizzLogger.E("QueryLatest " + qEx.Message);
                            }
                        });
                    }
                });
            }
            catch (FizzException ex)
            {
                FizzLogger.E("SubscribeAndQuery ex " + ex.Message);
            }
        }

        public bool FetchHistory(Action complete)
        {
            long beforeId = -1;
            if (_messageList.Count > 0)
                beforeId = _messageList.First().Value.Id;

            if (beforeId == -1)
                return false;

            try
            {
                FizzService.Instance.Client.Chat.QueryLatest(Id, _channelMeta.InitialQueryMessageCount, beforeId, (msgs, qEx) =>
                {
                    if (qEx == null)
                    {
                        if (msgs != null && msgs.Count > 0)
                        {
                            AddMessages(msgs);
                        }

                        if (FizzService.Instance.OnChannelMessagesAvailable != null)
                        {
                            FizzService.Instance.OnChannelMessagesAvailable.Invoke(Id);
                        }
                    }

                    if (complete != null)
                        complete.Invoke();
                });
            }
            catch (FizzException ex)
            {
                FizzLogger.E("FetchHistory ex " + ex.Message);
            }

            return true;
        }

        public void Reset()
        {
            _messageList.Clear();
            cached = false;
        }

        private bool cached = false;
        IList<FizzChannelMessage> cachedMessageList = null;
        private SortedList<long, FizzChannelMessage> _messageList = new SortedList<long, FizzChannelMessage>(new FizzChannelMessageComparer());
    }

    class FizzChannelMessageComparer : IComparer<long>
    {
        public int Compare(long lhs, long rhs)
        {
            return (int)(lhs - rhs);
        }
    }
}