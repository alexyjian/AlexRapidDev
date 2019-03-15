using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Security
{
    /// <summary>
    /// ALEXFW identity.
    /// </summary>
    public class ALEXFWIdentity : IIdentity
    {
        internal ALEXFWIdentity(ALEXFWPrincipal principal)
        {
            _Principal = principal;
        }

        private ALEXFWPrincipal _Principal;

        /// <summary>
        /// Gets the type of authentication used.
        /// </summary>
        public string AuthenticationType
        {
            get { return "Forms"; }
        }

        private bool? _IsAuthenticated;
        /// <summary>
        /// Gets a value that indicates whether the user has been authenticated.
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                if (_IsAuthenticated == null)
                {
                    HttpContext context = HttpContext.Current;
                    string name;
                    string authArea = null;
                    if (_Principal.CurrentRoute == null || !_Principal.CurrentRoute.DataTokens.ContainsKey("authArea"))
                        name = ALEXFWAuthentication.CookieName;
                    else
                    {
                        authArea = _Principal.CurrentRoute.DataTokens["authArea"].ToString();
                        name = ALEXFWAuthentication.CookieName + "_" + authArea;
                    }

                    object state = context.Items[name];
                    if (state == null)
                    {
                        if (context.Request.Cookies.AllKeys.Contains(name))
                        {
                            string cookies = context.Request.Cookies[name].Value;
                            DateTime expiredDate;
                            _IsAuthenticated = ALEXFWAuthentication.VerifyCookie(cookies, authArea, out _Name, out expiredDate);
                            if (_IsAuthenticated.Value && _Principal.RoleEntity != null)
                                if (expiredDate < DateTime.Now.Add(ALEXFWAuthentication.Timeout))
                                    ALEXFWAuthentication.SignIn(_Name, ALEXFWAuthentication.Timeout);
                        }
                        else if (context.Session != null && context.Session[name] != null)
                        {
                            DateTime expiredDate = (DateTime)context.Session[name];
                            if (expiredDate < DateTime.Now)
                                _IsAuthenticated = false;
                            else
                            {
                                context.Session[name] = DateTime.Now.Add(ALEXFWAuthentication.Timeout);
                                _Name = (string)context.Session[name + "_Username"];
                                _IsAuthenticated = true;
                            }
                        }
                        else
                        {
                            _IsAuthenticated = false;
                        }
                    }
                    else
                    {
                        _IsAuthenticated = (bool)state;
                    }
                    context.Items[name] = _IsAuthenticated.Value;
                }
                return _IsAuthenticated.Value;
            }
        }

        private string _Name;
        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        public string Name
        {
            get
            {
                if (!IsAuthenticated)
                    return null;
                return _Name;
            }
        }
    }
}
