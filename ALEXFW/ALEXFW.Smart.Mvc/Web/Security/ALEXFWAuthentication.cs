﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Routing;

namespace System.Web.Security
{
    /// <summary>
    /// ALEXFW authentication.
    /// </summary>
    public sealed class ALEXFWAuthentication
    {
        private static System.Configuration.Configuration _Config;
        private static byte[] _Key;

        static ALEXFWAuthentication()
        {
            _Config = WebConfigurationManager.OpenWebConfiguration("~");
            SystemWebSectionGroup system = (SystemWebSectionGroup)_Config.GetSectionGroup("system.web");
            IsEnabled = system.Authentication.Mode == AuthenticationMode.Forms;
            if (!IsEnabled)
                return;
            if (!_Config.AppSettings.Settings.AllKeys.Contains("ALEXFWAuthenticationKey"))
            {
                _Key = Guid.NewGuid().ToByteArray();
                _Config.AppSettings.Settings.Add("ALEXFWAuthenticationKey", Convert.ToBase64String(_Key));
                _Config.Save();
            }
            else
            {
                _Key = Convert.FromBase64String(_Config.AppSettings.Settings["ALEXFWAuthenticationKey"].Value);
            }
            CookieDomain = system.Authentication.Forms.Domain;
            CookieName = system.Authentication.Forms.Name ?? "ALEXFWauth";
            CookiePath = system.Authentication.Forms.Path;
            LoginUrl = system.Authentication.Forms.LoginUrl;
            Timeout = system.Authentication.Forms.Timeout;
        }

        /// <summary>
        /// Get the login url.
        /// </summary>
        public static string LoginUrl { get; private set; }

        /// <summary>
        /// Get the cookie domain.
        /// </summary>
        public static string CookieDomain { get; private set; }

        /// <summary>
        /// Get the cookie name.
        /// </summary>
        public static string CookieName { get; private set; }

        /// <summary>
        /// Get the cookie path.
        /// </summary>
        public static string CookiePath { get; private set; }

        /// <summary>
        /// Get the ALEXFW authentication enabled.
        /// </summary>
        public static bool IsEnabled { get; private set; }

        /// <summary>
        /// Get the cookie enable timespan.
        /// </summary>
        public static TimeSpan Timeout { get; private set; }

        /// <summary>
        /// Refresh authentication security key. 
        /// </summary>
        public static void RefreshSecurityKey()
        {
            _Key = Guid.NewGuid().ToByteArray();
            _Config.AppSettings.Settings.Add("ALEXFWAuthenticationKey", Convert.ToBase64String(_Key));
            _Config.Save();
        }

        /// <summary>
        /// Create cookies.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="authArea">Authenticate area.</param>
        /// <returns></returns>
        public static string CreateCookies(string username, string authArea)
        {
            return CreateCookies(username, authArea, Timeout);
        }

        /// <summary>
        /// Create cookies.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="authArea">Authenticate area.</param>
        /// <param name="timeout">Enable timespan.</param>
        /// <returns></returns>
        public static string CreateCookies(string username, string authArea, TimeSpan timeout)
        {
            ALEXFWCookiesToken token = new ALEXFWCookiesToken();
            token.Username = username;
            token.ExpiredDate = DateTime.Now.Add(timeout);
            byte[] data;
            if (authArea == null)
                data = Encoding.UTF8.GetBytes(token.Username).Concat(BitConverter.GetBytes(token.ExpiredDate.ToBinary())).ToArray();
            else
                data = Encoding.UTF8.GetBytes(token.Username).Concat(BitConverter.GetBytes(token.ExpiredDate.ToBinary())).Concat(Encoding.UTF8.GetBytes(authArea)).ToArray();
            token.NewSalt();
            token.Signature = GetTokenSignature(data, token.Salt);
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, token);
            data = stream.ToArray();
            stream.Dispose();
            return HttpServerUtility.UrlTokenEncode(data);

        }

        /// <summary>
        /// Get token signature data.
        /// </summary>
        /// <param name="data">Token data.</param>
        /// <param name="salt">Salt data.</param>
        /// <returns></returns>
        public static byte[] GetTokenSignature(byte[] data, byte[] salt)
        {
            data = data.Concat(_Key).ToArray();
            byte[] signature;
            using (SHA1 sha1 = SHA1.Create())
            {
                signature = sha1.ComputeHash(data);
                signature = sha1.ComputeHash(signature.Concat(salt).ToArray());
            }
            return signature;
        }

        /// <summary>
        /// Write token signature data.
        /// </summary>
        /// <param name="token">Token.</param>
        public static void WriteTokenSignature(ALEXFWToken token)
        {
            byte[] data = token.GetTokenData();
            token.NewSalt();
            token.Signature = GetTokenSignature(data, token.Salt);
        }

        /// <summary>
        /// Verify a token.
        /// </summary>
        /// <param name="data">Token data.</param>
        /// <param name="salt">Salt data.</param>
        /// <param name="signature">Token signature.</param>
        /// <returns></returns>
        public static bool VerifyToken(byte[] data, byte[] salt, byte[] signature)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (signature == null)
                throw new ArgumentNullException("signature");
            data = data.Concat(_Key).ToArray();
            using (SHA1 sha1 = SHA1.Create())
            {
                data = sha1.ComputeHash(data);
                data = sha1.ComputeHash(data.Concat(salt).ToArray());
            }
            for (int i = 0; i < 20; i++)
                if (data[i] != signature[i])
                    return false;
            return true;
        }

        /// <summary>
        /// Verify a token.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <returns></returns>
        public static bool VerifyToken(ALEXFWToken token)
        {
            if (token == null)
                throw new ArgumentNullException("token");
            return VerifyToken(token.GetTokenData(), token.Salt, token.Signature);
        }

        /// <summary>
        /// Verify cookie.
        /// </summary>
        /// <param name="cookieValue">Cookie value.</param>
        /// <param name="authArea">Authenticate area.</param>
        /// <param name="username">Username.</param>
        /// <param name="expiredDate">Expired date.</param>
        /// <returns></returns>
        public static bool VerifyCookie(string cookieValue, string authArea, out string username, out DateTime expiredDate)
        {
            username = null;
            expiredDate = DateTime.MinValue;
            byte[] data;
            try
            {
                data = HttpServerUtility.UrlTokenDecode(cookieValue);
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream(data);
                ALEXFWCookiesToken token = (ALEXFWCookiesToken)formatter.Deserialize(stream);
                stream.Dispose();

                if (token.Signature.Length != 20)
                    return false;
                if (token.ExpiredDate < DateTime.Now)
                    return false;
                if (token.Username == null)
                    return false;
                if (authArea == null)
                    data = token.GetTokenData();
                else
                    data = token.GetTokenData().Concat(Encoding.UTF8.GetBytes(authArea)).ToArray();

                if (!VerifyToken(data, token.Salt, token.Signature))
                    return false;

                username = token.Username;
                expiredDate = token.ExpiredDate;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sign in.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="forever">Sign in forever that will not timeout.</param>
        public static void SignIn(string username, bool forever)
        {
            if (forever)
                SignIn(username, TimeSpan.FromDays(360));
            else
                SignIn(username, TimeSpan.Zero);
        }

        /// <summary>
        /// Sign in.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="timeout">Timeout.</param>
        public static void SignIn(string username, TimeSpan timeout)
        {
            RouteData route = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
            string authArea = null;
            if (route.DataTokens.ContainsKey("authArea"))
                authArea = route.DataTokens["authArea"].ToString();
            string cookieName;
            if (authArea == null)
                cookieName = CookieName;
            else
                cookieName = CookieName + "_" + authArea;
            if (timeout == TimeSpan.Zero)
            {
                HttpContext.Current.Session[cookieName] = DateTime.Now.Add(Timeout);
                HttpContext.Current.Session[cookieName + "_Username"] = username;
            }
            else
            {
                string cookieValue;
                cookieValue = CreateCookies(username, authArea, timeout);
                HttpCookie cookie;
                if (HttpContext.Current.Response.Cookies.AllKeys.Contains(cookieName))
                    cookie = HttpContext.Current.Response.Cookies[cookieName];
                else
                    cookie = new HttpCookie(cookieName);
                cookie.Value = cookieValue;
                cookie.Domain = CookieDomain;
                cookie.Expires = DateTime.Now.Add(timeout);
                cookie.HttpOnly = true;
                cookie.Path = CookiePath;
                HttpContext.Current.Response.Cookies.Remove(cookieName);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// Sign out.
        /// </summary>
        public static void SignOut()
        {
            RouteData route = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
            string authArea = null;
            if (route.DataTokens.ContainsKey("authArea"))
                authArea = route.DataTokens["authArea"].ToString();
            string cookieName = authArea == null ? CookieName : CookieName + "_" + authArea;
            if (HttpContext.Current.Request.Cookies.AllKeys.Contains(cookieName))
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Set(cookie);
            }
            HttpContext.Current.Session.Remove(cookieName);
            HttpContext.Current.Session.Remove(cookieName + "_Username");
        }
    }
}
