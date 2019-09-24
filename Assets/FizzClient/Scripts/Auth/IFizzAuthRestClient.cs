using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fizz.Common
{	
	public interface IFizzAuthRestClient
    {
        void Open(FizzSessionRepository sessionRepo, Action<FizzException> action);
        void Close();
        void FetchSessionToken(Action<FizzException> callback);
        void Post(string host, string path, string json, Action<string, FizzException> callback);
        void Delete(string host, string path, string json, Action<string, FizzException> callback);
        void Get(string host, string path, Action<string, FizzException> callback);
    }
}