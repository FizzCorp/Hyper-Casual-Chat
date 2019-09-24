using System;
using System.Collections.Generic;

using Fizz.Common;

namespace Fizz.Ingestion.Impl
{
    class FizzInMemoryEventComparer : IComparer<long>
    {
        public int Compare (long lhs, long rhs)
        {
            int result = (int)(lhs - rhs);
            if (result == 0)
                return 1;   // Handle equality as beeing greater
            else
                return result;
        }
    }

    public class FizzInMemoryEventLog : IFizzEventLog
    {
        private SortedList<long, FizzEvent> log = new SortedList<long, FizzEvent> (new FizzInMemoryEventComparer ());

        public void Put(FizzEvent item)
        {
            if (item == null)
            {
                FizzLogger.W("Empty item put in log");
                return;
            }

            log.Add(item.Id, item);
        }

        public void Read(int count, Action<List<FizzEvent>> callback)
        {
            if (callback == null)
            {
                return;
            }

            List<FizzEvent> events = new List<FizzEvent>();

            foreach (FizzEvent item in log.Values) 
            {
                events.Add(item);
                if (events.Count >= count) {
                    break;
                }
            }

            callback.Invoke(events);
        }

        public void RollTo(FizzEvent item)
        {
            try
            {
                int index = log.IndexOfValue (item);
                for (int i = index; i >= 0; i--)
                {
                    log.RemoveAt (i);
                }
            }
            catch (Exception e)
            {
                FizzLogger.E ("Fizz Event Log RollTo " + e);
            }
        }
    }
}
