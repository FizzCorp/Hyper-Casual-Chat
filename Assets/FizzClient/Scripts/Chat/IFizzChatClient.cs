using System;
using System.Collections.Generic;

namespace Fizz.Chat
{
    public interface IFizzChatClient
    {
        bool IsConnected { get; }

        IFizzChannelMessageListener Listener { get; }

        void Subscribe(string channel, Action<FizzException> callback);

        void Unsubscribe(string channel, Action<FizzException> callback);

        void QueryLatest(
            string channelId, 
            int count, 
            Action<IList<FizzChannelMessage>, FizzException> callback);

        void QueryLatest(
            string channelId, 
            int count, 
            long beforeId, 
            Action<IList<FizzChannelMessage>, FizzException> callback);

        void PublishMessage(
            string channelId, 
            string nick, 
            string body, 
            Dictionary<string, string> data, 
            bool translate, 
            bool filter, 
            bool persist, 
            Action<FizzException> callback);

        void UpdateMessage(
            string channelId,
            long messageId, 
            string nick, 
            string body, 
            Dictionary<string, string> data, 
            bool translate, 
            bool filter, 
            bool persist, 
            Action<FizzException> callback);

        void DeleteMessage(
            string channelId,
            long messageId, 
            Action<FizzException> callback);

        void Ban(
            string channel, 
            string userId, 
            Action<FizzException> callback);

        void Unban(
            string channel, 
            string userId, 
            Action<FizzException> callback);

        void Mute(
            string channel, 
            string userId, 
            Action<FizzException> callback);

        void Unmute(
            string channel, 
            string userId, 
            Action<FizzException> callback);
    }
}