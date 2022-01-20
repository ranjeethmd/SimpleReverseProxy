using Microsoft.AspNetCore.Http;
using ReverseProxy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ReverseProxy.Lib
{
    public class CookieClientService : IClientBuilder
    {
        private CookieContainer _cookieContainer = new CookieContainer();
        public HttpClient GetClient(HttpContext context,Uri uri)
        {
            foreach (var cookie in context.Request.Cookies)
            {
                _cookieContainer.Add(uri, new Cookie(cookie.Key, HttpUtility.UrlEncode(cookie.Value)));
            }
            
            var handler = new HttpClientHandler() { CookieContainer = _cookieContainer };
            return new HttpClient(handler);            
        }

        public void SetResponseCookies(HttpContext context, Uri uri)
        {
            var cookies = _cookieContainer.GetCookies(uri).Cast<Cookie>(); 

            var responseCookies = context.Response.Cookies;

            foreach (var cookie in cookies)
            {
                if(cookie.Name.Equals("Domain", StringComparison.OrdinalIgnoreCase))
                {
                    responseCookies.Append(cookie.Name, HttpUtility.UrlEncode(cookie.Value.Replace(uri.Host,context.Request.Host.Value)));
                }
                else
                {
                    responseCookies.Append(cookie.Name, HttpUtility.UrlEncode(cookie.Value));
                }                
            }
        }
    }
}
