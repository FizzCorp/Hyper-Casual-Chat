using System;
using System.Collections.Generic;
using Fizz.Ingestion.Impl;

namespace Fizz.Ingestion
{
	public interface IFizzEventLog 
    {
        void Put(FizzEvent item);
        void Read(int count, Action<List<FizzEvent>> callback);
        void RollTo(FizzEvent item);
    }
}