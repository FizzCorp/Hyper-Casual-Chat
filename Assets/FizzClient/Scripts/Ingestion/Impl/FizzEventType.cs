using System;

namespace Fizz.Ingestion.Impl
{
    public enum FizzEventType
    {
        session_started,
        session_ended,
        text_msg_sent,
        product_purchased
    }
}
