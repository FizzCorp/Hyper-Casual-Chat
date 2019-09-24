using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using Fizz.Common.Json;

namespace Fizz.Common
{
    public class FizzAuthRestClient: IFizzAuthRestClient
    {
        private IFizzRestClient _restClient;
        private FizzSessionRepository _sessionRepository;
        private Queue<Action<FizzException>> _requestQueue = new Queue<Action<FizzException>>();

        public FizzAuthRestClient(IFizzRestClient restClient)
        {
            _restClient = restClient;
        }

        public void Open(FizzSessionRepository sessionRepo, Action<FizzException> callback)
        {
            if (sessionRepo == null)
            {
                throw FizzException.ERROR_INVALID_SESSION_REPOSITORY;
            }

            _sessionRepository = sessionRepo;

            FetchSessionToken(callback);
        }

        public void Close()
        {
            _sessionRepository = null;
        }

        public void Post(string host, 
                         string path, 
                         string json,
                         Action<string, FizzException> callback)
        {
            _restClient.Post(host, path, json, AuthHeaders(), (response, ex) =>
            {
                ProcessResponse(response, ex, callback, () => 
                {
                    _restClient.Post(host, path, json, AuthHeaders(), callback);
                });
            });
        }

        public void Delete(string host, 
                           string path, 
                           string json, 
                           Action<string, FizzException> callback)
        {
            _restClient.Delete(host, path, json, AuthHeaders(), (response, ex) => 
            {
                ProcessResponse(
                    response, 
                    ex, 
                    callback, 
                    () => _restClient.Delete(host, path, json, AuthHeaders(), callback)
                );
            });    
        }

        public void Get(string host, 
                        string path, 
                        Action<string, FizzException> callback)
        {
            _restClient.Get(host, path, AuthHeaders(), (response, ex) => 
            {
                ProcessResponse(
                    response, 
                    ex, 
                    callback, 
                    () => _restClient.Get(host, path, AuthHeaders(), callback)
                );
            });
        }

        public void FetchSessionToken(Action<FizzException> onFetch)
        {
            _requestQueue.Enqueue(onFetch);
            if (_requestQueue.Count > 1)
            {
                return;
            }

            _sessionRepository.FetchToken ((session, ex) => {
                if (ex != null)
                {
                    while (_requestQueue.Count > 0)
                    {
                        FizzUtils.DoCallback(ex, _requestQueue.Dequeue());
                    }
                }
                else 
                {
                    while (_requestQueue.Count > 0)
                    {
                        FizzUtils.DoCallback(null, _requestQueue.Dequeue());
                    }
                }
            });
        }
       
        private IDictionary<string,string> AuthHeaders()
        {
            if (_sessionRepository == null || string.IsNullOrEmpty(_sessionRepository.Session._token))
            {
                return null;
            }
            else
            {
                return FizzUtils.headers(_sessionRepository.Session._token);
            }
        }


        private void ProcessResponse(string response, 
                                     FizzException ex, 
                                     Action<string,FizzException> onResult,
                                     Action onRetry)
        {
            if (ex != null)
            {
                if (ex.Code == FizzError.ERROR_AUTH_FAILED)
                {
                    FetchSessionToken(authEx =>
                    {
                        if (authEx != null)
                        {
                            FizzUtils.DoCallback<string>(null, ex, onResult);
                        }
                        else
                        {
                            if (onRetry != null)
                            {
                                onRetry();
                            }
                        }
                    });
                }
                else
                {
                    FizzUtils.DoCallback<string>(null, ex, onResult);
                }
            }
            else
            {
                FizzUtils.DoCallback<string>(response, ex, onResult);
            }
        }
    }
}
