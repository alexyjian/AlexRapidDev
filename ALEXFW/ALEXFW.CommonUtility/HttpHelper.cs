using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ALEXFW.CommonUtility
{
    /// <summary>
    ///     http协议处理方法  GET、POST方式Http请求方法
    /// </summary>
    public static class HttpHelper
    {
        public static async Task<string> GetHttp(Uri uri, Encoding encoding, int timeout)
        {
            var request = WebRequest.CreateHttp(uri);
            request.Timeout = timeout;
            request.Method = "GET";
            request.AllowAutoRedirect = true;
            try
            {
                var response = await request.GetResponseAsync();
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream, encoding);
                var result = await reader.ReadToEndAsync();
                return result;
            }
            catch
            {
                return null;
            }
        }

        public static Task<string> GetHttp(Uri uri, Encoding encoding)
        {
            return GetHttp(uri, encoding, 5000);
        }

        public static Task<string> GetHttp(Uri uri, int timeout)
        {
            return GetHttp(uri, Encoding.UTF8, timeout);
        }

        public static Task<string> GetHttp(Uri uri)
        {
            return GetHttp(uri, Encoding.UTF8, 5000);
        }

        public static Task<string> GetHttp(string url)
        {
            return GetHttp(new Uri(url));
        }

        public static Task<string> GetHttp(string url, object querystring)
        {
            var type = querystring.GetType();
            var data = new Dictionary<string, string>();
            foreach (var property in type.GetProperties())
                data.Add(property.Name, property.GetValue(querystring).ToString());
            url += "?" + GetQueryString(data);
            return GetHttp(url);
        }

        public static async Task<string> PostHttp(Uri uri, byte[] rawData, Encoding encoding, int timeout)
        {
            var request = WebRequest.CreateHttp(uri);
            request.Timeout = timeout;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = rawData.Length;
            request.AllowAutoRedirect = true;
            if (rawData != null)
                try
                {
                    var stream = await request.GetRequestStreamAsync();
                    await stream.WriteAsync(rawData, 0, rawData.Length);
                    stream.Close();
                }
                catch
                {
                    return null;
                }
            try
            {
                var response = await request.GetResponseAsync();
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream, encoding);
                var result = await reader.ReadToEndAsync();
                return result;
            }
            catch (WebException ex)
            {
                var stream = ex.Response.GetResponseStream();
                var reader = new StreamReader(stream, encoding);
                var result = reader.ReadToEnd();
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static Task<string> PostHttp(Uri uri, byte[] rawData, Encoding encoding)
        {
            return PostHttp(uri, rawData, encoding, 5000);
        }

        public static Task<string> PostHttp(Uri uri, byte[] rawData, int timeout)
        {
            return PostHttp(uri, rawData, Encoding.UTF8, timeout);
        }

        public static Task<string> PostHttp(Uri uri, byte[] rawData)
        {
            return PostHttp(uri, rawData, Encoding.UTF8, 5000);
        }

        public static Task<string> PostHttp(Uri uri, object formData, Encoding encoding)
        {
            var type = formData.GetType();
            var data = new Dictionary<string, string>();
            foreach (var property in type.GetProperties())
                data.Add(property.Name, property.GetValue(formData).ToString());
            return PostHttp(uri, encoding.GetBytes(GetFormString(data)));
        }

        public static Task<string> PostHttp(Uri uri, object formData)
        {
            return PostHttp(uri, formData, Encoding.UTF8);
        }

        public static Task<string> PostHttp(Uri uri, IDictionary<string, string> formData, Encoding encoding)
        {
            return PostHttp(uri, encoding.GetBytes(GetFormString(formData)), encoding);
        }

        public static Task<string> PostHttp(Uri uri, IDictionary<string, string> formData)
        {
            return PostHttp(uri, formData, Encoding.UTF8);
        }

        public static Task<string> PostHttp(string url, object formData)
        {
            return PostHttp(new Uri(url), formData);
        }

        public static Task<string> PostHttp(string url, IDictionary<string, string> formData)
        {
            return PostHttp(new Uri(url), formData);
        }

        public static string GetQueryString(IDictionary<string, string> dictionary)
        {
            return string.Join("&", dictionary.Select(t => t.Key + "=" + Uri.EscapeUriString(t.Value)));
        }

        public static string GetFormString(IDictionary<string, string> dictionary)
        {
            return string.Join("&", dictionary.Select(t => t.Key + "=" + Uri.EscapeDataString(t.Value)));
        }

        /// <summary>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postStream"></param>
        /// <param name="encoding"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static async Task<string> HttpPost(string url, Stream postStream = null,
            Encoding encoding = null,
            int timeOut = 10000)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = timeOut;

            request.ContentLength = postStream != null ? postStream.Length : 0;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.KeepAlive = true;

            request.UserAgent =
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";

            #region 输入二进制流

            if (postStream != null)
            {
                postStream.Position = 0;

                //直接写入流
                var requestStream = await request.GetRequestStreamAsync();

                var buffer = new byte[1024];
                var bytesRead = 0;
                while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                    requestStream.Write(buffer, 0, bytesRead);

                postStream.Close(); //关闭文件访问
            }

            #endregion 输入二进制流

            var response = (HttpWebResponse)await request.GetResponseAsync();

            using (var responseStream = response.GetResponseStream())
            {
                using (
                    var myStreamReader = new StreamReader(responseStream,
                        encoding ?? Encoding.GetEncoding("utf-8")))
                {
                    var retString = myStreamReader.ReadToEnd();
                    return retString;
                }
            }
        }
    }
}