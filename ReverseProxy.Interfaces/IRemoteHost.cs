using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseProxy.Interfaces
{
    public  interface IRemoteHost
    {
        HttpRequestMessage CreateRequest(HttpContext context, Uri remoteUri);

        void PrepareResponse(HttpContext context, HttpResponseMessage remoteResponse);
    }
}
