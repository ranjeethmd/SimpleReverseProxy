using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ReverseProxy.Interfaces
{
    public interface IClientBuilder
    {
        HttpClient GetClient(HttpContext context, Uri uri);

        void SetResponseCookies(HttpContext context, Uri uri);
    }
}
