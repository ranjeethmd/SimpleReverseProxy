using Microsoft.AspNetCore.Http;
using ReverseProxy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseProxy.Lib
{
    public class RemoteHostService : IRemoteHost
    {
        public HttpRequestMessage CreateRequest(HttpContext context, Uri remoteUri)
        {
            var request = context.Request;

            var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), remoteUri);

            request.EnableBuffering();
            request.Body.Position = 0;
            requestMessage.Content = new StreamContent(request.Body);

            foreach (var header in request.Headers)
            {
                requestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }

            return requestMessage;
        }

        public void PrepareResponse(HttpContext context, HttpResponseMessage remoteResponse)
        {
            var response = context.Response;

            context.Response.StatusCode = (int)remoteResponse.StatusCode;

            foreach (var header in remoteResponse.Headers)
            {
                response.Headers.Add(header.Key, header.Value.ToArray());
            }

            foreach (var header in remoteResponse.Content.Headers)
            {
                response.Headers.Add(header.Key, header.Value.ToArray());
            }

            response.Headers.Remove("transfer-encoding");
        }
    }
}
