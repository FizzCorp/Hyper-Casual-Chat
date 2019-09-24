using System;

namespace Fizz.Chat
{
    public interface IFizzChannelMessageListener
    {
        Action<bool> OnConnected { get; set; }
		Action<FizzException> OnDisconnected { get; set; }

        Action<FizzChannelMessage> OnMessagePublished { get; set; }
        Action<FizzChannelMessage> OnMessageUpdated { get; set; }
        Action<FizzChannelMessage> OnMessageDeleted { get; set; }
    }
}
