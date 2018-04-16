using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using Utilities.Extension;

namespace Utilities
{
    public class WebHelper
    {
        private readonly static string CONTENTTYPE_DEFAULT = "application/x-www-form-urlencoded";
        public class DownloadEntity
        {
            public long TotalSize { get; set; }
            public long DownloadedSize { get; set; }
            public double Progress { get; set; }
            public int ReceivedBytes { get; set; }
        }

        public static string Read(string url, bool isUseCache = true, int timeout = 10000)
        {
            string strRel = string.Empty;
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Timeout = timeout;
                if (!isUseCache)
                {
                    HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                    request.CachePolicy = noCachePolicy;
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                WebHeaderCollection header = response.Headers;

                var encoding = ASCIIEncoding.ASCII;
                using (var reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    strRel = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("WebHelper:Read exception:" + e.Message);
            }

            return strRel;
        }

        public static bool DownLoad(string url, string fileName, Dictionary<string, string> headers = null, bool isUseCache = true, int timeout = 10000)
        {
            System.GC.Collect();
            bool bRel = false;
            WebResponse response = null;
            Stream stream = null;
            HttpWebRequest request = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        request.Headers.Add(key, headers[key]);
                    }
                }
                //request.Proxy = null;
                request.Timeout = timeout;
                request.ReadWriteTimeout = timeout;
                if (!isUseCache)
                {
                    HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                    request.CachePolicy = noCachePolicy;
                }
                response = request.GetResponse();
                stream = response.GetResponseStream();
                bRel = SaveBinaryFile(response, fileName);
            }
            catch (Exception e)
            {
                Debug.WriteLine("WebHelper:Read exception:" + e.Message);
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }

                if (response != null)
                {
                    response.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
                stream = null;
                request = null;
                response = null;
            }

            return bRel;
        }
        /// <summary>
        /// Save a binary file to disk.
        /// </summary>
        /// <param name="response">The response used to save the file</param>
        private static bool SaveBinaryFile(WebResponse response, string fileName)
        {
            bool bRel = true;
            byte[] buffer = new byte[1024];
            try
            {
                string folder = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                FileUtility.DeleteFile(fileName);
                Stream outStream = System.IO.File.Create(fileName);
                Stream inStream = response.GetResponseStream();
                long totalSize = response.ContentLength;
               NLogger.LogHelper.UILogger.DebugFormat("DownLoad: ContentLength: {0} ", totalSize);

                if (totalSize == -1)
                {
                    NLogger.LogHelper.UILogger.DebugFormat("bad file?");
                    outStream.Close();
                    inStream.Close();
                    return false;
                } 
                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        outStream.Write(buffer, 0, l);
                }
                while (l > 0);
                outStream.Close();
                inStream.Close();
            }
            catch(Exception e)
            {
                bRel = false;
            }
            return bRel;
        }

        public static bool DownloadAsync(string url, string fileName, ref bool isCancel, Action<DownloadEntity> callback, Dictionary<string, string> headers = null, bool isUseCache = true, int timeout = 10000)
        {
            System.GC.Collect();
            bool isSucceed = false;
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(fileName))
            {
                return isSucceed;
            }

            string folder = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            FileStream ostream = null;
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;
            Stream responseStream = null;
            long totalSize = 0;
            try
            {
                httpRequest = (HttpWebRequest)WebRequest.Create(url);
                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        httpRequest.Headers.Add(key, headers[key]);
                    }
                }
                if (httpRequest != null)
                {
                    //httpRequest.Proxy = null;
                    httpRequest.Timeout = timeout;
                    httpRequest.ReadWriteTimeout = timeout;
                    if (!isUseCache)
                    {
                        HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                        httpRequest.CachePolicy = noCachePolicy;
                    }
                    int nTryCount = 0;
                    bool bOK = false;
                    do
                    {
                        try
                        {
                            httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                            bOK = true;
                        }
                        catch (WebException e)
                        {
                            nTryCount++;
                        }

                        if (isCancel)
                        {
                            break;
                        }
                    } while (!bOK && nTryCount > 2);

                    if (httpResponse != null && httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        responseStream = httpResponse.GetResponseStream();

                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);
                        }

                        int bufferSize = 8 * 1024;
                        byte[] buffer = new byte[bufferSize];

                        long totalReadedSize = 0;
                        ostream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
                        totalSize = httpResponse.ContentLength;
                        while (true)
                        {
                            if (isCancel)
                            {
                                break;
                            }

                            int retSize = responseStream.Read(buffer, 0, bufferSize);
                            if (retSize <= 0)
                            {
                                break;
                            }

                            ostream.Write(buffer, 0, retSize);
                            totalReadedSize += retSize;
                            if (callback != null && totalSize > 0)
                            {
                                double dbPgs = (double)totalReadedSize * 100 / totalSize;
                                callback(new DownloadEntity()
                                {
                                    TotalSize = totalSize,
                                    DownloadedSize = totalReadedSize,
                                    Progress = dbPgs,
                                    ReceivedBytes = retSize
                                });
                            }
                        }
                        ostream.Flush();
                        ostream.Close();
                        ostream = null;

                        if (isCancel)
                        {
                            if (File.Exists(fileName))
                            {
                                File.Delete(fileName);
                            }
                        }
                        else
                        {
                            isSucceed = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ostream != null)
                {
                    ostream.Close();
                    ostream = null;
                }
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (httpResponse != null)
                {
                    httpResponse.Close();
                }

                if (httpRequest != null)
                {
                    httpRequest.Abort();
                }

                responseStream = null;
                httpRequest = null;
                httpResponse = null;
            }

            return isSucceed;
        }

        public static void AppendHTTPParameter(string key, string value, ref string content, string separator = "&")
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;
            if (!string.IsNullOrEmpty(content))
                content += separator;

            content += string.Format("{0}={1}", HttpUtility.UrlEncode(key, Encoding.UTF8), HttpUtility.UrlEncode(value, Encoding.UTF8));
        }

        public static void AppendHTTPParameter(string key, string value, ref StringBuilder content, string separator = "&")
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;
            if (content.Length > 0)
                content.AppendFormat(@"{0}", separator);

            content.AppendFormat("{0}={1}", HttpUtility.UrlEncode(key, Encoding.UTF8), HttpUtility.UrlEncode(value, Encoding.UTF8));
        }

        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, Encoding requestEncoding, Dictionary<string, string> headers = null, int? timeout = 100000)
        {
            byte[] data = null;
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                data = requestEncoding.GetBytes(buffer.ToString());
            }

            return CreatePostHttpResponse(url, data, requestEncoding, headers, timeout);
        }

        public static HttpWebResponse CreatePostHttpResponse(string url, string postData, Encoding requestEncoding, Dictionary<string, string> headers = null, int? timeout = 100000)
        {
            byte[] data = requestEncoding.GetBytes(postData);
            return CreatePostHttpResponse(url, data, requestEncoding, headers, timeout);
        }

        public static HttpWebResponse CreatePostHttpResponse(string url, byte[] postData, Encoding requestEncoding, Dictionary<string, string> headers = null, int? timeout = 100000)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            if (postData.IsNull())
            {
                throw new ArgumentNullException("postData");
            }

            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    request.Headers.Add(key, headers[key]);
                }
            }

            request.Method = "POST";
            request.ContentType = CONTENTTYPE_DEFAULT;

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }

            if (!postData.IsNull())
            {
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}
