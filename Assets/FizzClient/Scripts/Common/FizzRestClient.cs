using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using Fizz.Common.Json;

namespace Fizz.Common
{
    public interface IFizzRestClient
    {
        void Post(string host, string path, string json, IDictionary<string, string> headers, Action<string, FizzException> callback);
        void Delete(string host, string path, string json, IDictionary<string, string> headers, Action<string, FizzException> callback);
        void Get(string host, string path, IDictionary<string, string> headers, Action<string, FizzException> callback);
    }

    public class FizzRestClient: IFizzRestClient
    {
        private static readonly FizzException ERROR_INVALID_DISPATCHER = new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_dispatcher");
        private static readonly FizzException ERROR_INVALID_HOST = new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_host");
        private static readonly FizzException ERROR_INVALID_PATH = new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_path");
        private static readonly FizzException ERROR_INVALID_CONTENT = new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_request_content");

        private static readonly string CONTENT_TYPE = "application/json";
        private static readonly int REQ_TIMEOUT = 15*1000;

        private readonly IFizzActionDispatcher _dispatcher;

        public FizzRestClient(IFizzActionDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw ERROR_INVALID_DISPATCHER;
            }

            _dispatcher = dispatcher;
        }

        public virtual void Post(string host, 
                                 string path, 
                                 string json, 
                                 IDictionary<string,string> headers,
                                 Action<string, FizzException> callback)
        {
            Action<string, FizzException> onDone = FizzUtils.SafeCallback(callback);

            if (json == null)
            {
                onDone.Invoke(null, ERROR_INVALID_CONTENT);
                return;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            SendRequestAsync(host, path, "POST", headers, CONTENT_TYPE, buffer.Length, buffer, (status, response, ex) =>
            {
                if (ex != null)
                {
                    onDone.Invoke(null, ex);
                }
                else
                {
                    FormatResponse(status, response, onDone);
                }
            });
        }

        public void Delete(string host, 
                           string path, 
                           string json, 
                           IDictionary<string, string> headers,
                           Action<string, FizzException> callback)
        {
            Action<string, FizzException> onDone = FizzUtils.SafeCallback(callback);

            if (json == null)
            {
                onDone.Invoke(null, ERROR_INVALID_CONTENT);
                return;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            int bufferCount = buffer.Length;

            if (string.IsNullOrEmpty (json))
            {
                buffer = null;
                bufferCount = 0;
            }

            SendRequestAsync(host, path, "DELETE", headers, CONTENT_TYPE, bufferCount, buffer, (status, response, ex) =>
            {
                if (ex != null)
                {
                    onDone.Invoke(null, ex);
                }
                else
                {
                    FormatResponse(status, response, onDone);
                }
            });
        }

        public void Get(string host, 
                        string path, 
                        IDictionary<string, string> headers,
                        Action<string, FizzException> callback)
        {
            Action<string, FizzException> onDone = FizzUtils.SafeCallback(callback);

            SendRequestAsync(host, path, "GET", headers, CONTENT_TYPE, 0, null, (status, response, ex) => 
            {
                if (ex != null)
                {
                    FizzUtils.DoCallback(null, ex, onDone);
                }
                else
                {
                    FormatResponse(status, response, onDone);
                }
            });   
        }

        private void SendRequestAsync(string host, 
                                      string path, 
                                      string verb,
                                      IDictionary<string,string> headers,
                                      string contentType,
                                      int contentLength, 
                                      byte[] content, 
                                      Action<HttpStatusCode, byte[], FizzException> callback)
        {
            if (host == null)
            {
                callback(HttpStatusCode.NotFound, null, ERROR_INVALID_HOST);
                return;
            }
            if (path == null)
            {
                callback(HttpStatusCode.NotFound, null, ERROR_INVALID_PATH);
                return;
            }

            try
            {
                HttpWebRequest request = BuildRequest(host + path, verb, contentType, contentLength);

                if (headers != null) 
                {
                    foreach (string key in headers.Keys)
                    {
                        request.Headers.Add(key, headers[key]);
                    }   
                }

                if (content != null)
                {
                    SendRequestAsync(request, contentLength, content, (status, response, ex) =>
                    {
                        _dispatcher.Post(() => callback(status, response, ex));
                    });
                }
                else
                {
                    GetResponseAsync(request, (status, response, ex) => 
                    {
                        _dispatcher.Post(() => callback(status, response, ex));
                    });
                }
            }
            catch (Exception ex)
            {
                callback(HttpStatusCode.NotFound, null, new FizzException(FizzError.ERROR_REQUEST_FAILED, ex.Message));
            }
        }

        private void SendRequestAsync(WebRequest request, 
                                      int contentLength, 
                                      byte[] content, 
                                      Action<HttpStatusCode, byte[], FizzException> callback)
        {
            try
            {
                FizzTimer timeout = new FizzTimer(REQ_TIMEOUT, _dispatcher, () => 
                {
                    request.Abort();
                    callback(HttpStatusCode.NotFound, null, new FizzException(FizzError.ERROR_REQUEST_FAILED, "request_timeout"));
                });

                request.BeginGetRequestStream(result =>
                {
                    try
                    {
                        request.EndGetRequestStream(result).Write(content, 0, contentLength);
                        GetResponseAsync(request, (status, res, ex) =>
                        {
                            if (timeout.TryAbort())
                            {
                                callback(status, res, ex);
                            }
                        });
                    }
                    catch(Exception ex)
                    {
                        if (timeout.TryAbort())
                        {
                            callback(HttpStatusCode.NotFound, null, new FizzException(FizzError.ERROR_REQUEST_FAILED, ex.Message));
                        }
                    }
                }, null);
            }
            catch (Exception ex)
            {
                callback(HttpStatusCode.NotFound, null, new FizzException(FizzError.ERROR_REQUEST_FAILED, ex.Message));
            }
        }

        private static void GetResponseAsync(WebRequest request, Action<HttpStatusCode, byte[], FizzException> callback)
        {
            try
            {
                request.BeginGetResponse(result =>
                {
                    try
                    {
                        HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
                        ReadResponseAsync(response, callback);
                    }
                    catch(WebException ex)
                    {
                        if (ex.Status == WebExceptionStatus.ProtocolError)
                        {
                            HttpWebResponse response = (HttpWebResponse)ex.Response;
                            ReadResponseAsync(response, callback);
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    catch(Exception ex)
                    {
                        callback(HttpStatusCode.NotFound, null, new FizzException(FizzError.ERROR_REQUEST_FAILED, ex.Message));
                    }
                }, null);
            }
            catch (Exception ex)
            {
                callback(HttpStatusCode.NotFound, null, new FizzException(FizzError.ERROR_REQUEST_FAILED, ex.Message));
            }
        }

        private static void ReadResponseAsync(HttpWebResponse response, Action<HttpStatusCode, byte[], FizzException> callback)
        {
            const int chunkSize = 8192;
            byte[] chunkBuffer = new byte[chunkSize];
            List<byte> responseBuffer = new List<byte>();

            ReadResponseChunksAsync(response, chunkBuffer, chunkSize, (readSize, ex) =>
            {
                if (ex != null)
                {
                    callback(response.StatusCode, null, ex);
                }
                else
                {
                    if (readSize > 0)
                    {
                        byte[] buffer = new byte[readSize];
                        Array.Copy(chunkBuffer, buffer, readSize);
                        responseBuffer.AddRange(buffer);
                    }
                    else
                    {
                        callback(response.StatusCode, responseBuffer.ToArray(), null);
                    }
                }
            });
        }

        private static void ReadResponseChunksAsync(WebResponse response, byte[] chunkBuffer, int chunkSize, Action<int, FizzException> callback)
        {
            try
            {
                Stream stream = response.GetResponseStream();
                stream.BeginRead(chunkBuffer, 0, chunkSize, result =>
                {
                    int size = stream.EndRead(result);
                    callback(size, null);
                    if (size > 0)
                    {
                        ReadResponseChunksAsync(response, chunkBuffer, chunkSize, callback);
                    }
                }, null);
            }
            catch (Exception ex)
            {
                callback(-1, new FizzException(FizzError.ERROR_REQUEST_FAILED, ex.Message));
            }
        }

        private static void FormatResponse(HttpStatusCode status, byte[] body, Action<string, FizzException> callback)
        {
            try
            {
                string buffer = Encoding.UTF8.GetString(body);
                switch (status)
                {
                    case HttpStatusCode.OK:
                        FizzUtils.DoCallback(buffer, null, callback);
                        break;
                    case HttpStatusCode.BadRequest:
                        FizzUtils.DoCallback(null, new FizzException(FizzError.ERROR_BAD_ARGUMENT, JSONNode.Parse(buffer)["reason"]), callback);
                        break;
                    case HttpStatusCode.Unauthorized:
                        FizzUtils.DoCallback(null, new FizzException(FizzError.ERROR_AUTH_FAILED, JSONNode.Parse(buffer)["reason"]), callback);
                        break;
                    default:
                        FizzUtils.DoCallback(null, new FizzException(FizzError.ERROR_REQUEST_FAILED, buffer), callback);
                        break;
                }
            }
            catch (Exception responseEx)
            {
                FizzUtils.DoCallback<string>(
                    null,
                    new FizzException(FizzError.ERROR_REQUEST_FAILED, responseEx.Message),
                    callback
                );
            }
        }

        HttpWebRequest BuildRequest(string url, string verb, string contentType, int contentLength)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = verb;
            request.ContentType = contentType;
            request.ContentLength = contentLength;
            request.Timeout = REQ_TIMEOUT;

            return request;
        }
    }
}
