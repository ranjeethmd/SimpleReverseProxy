

using ReverseProxy.Interfaces;

namespace ReverseProxy;
public class ReverseProxyMiddleware
{   
  
    private readonly ILogger<ReverseProxyMiddleware> _logger;
    private readonly IUriBuilder _uriBuilder;
    private readonly IRemoteHost _remoteHost;
    private readonly IClientBuilder _clientBuilder;

    public ReverseProxyMiddleware(
        RequestDelegate _,
        ILogger<ReverseProxyMiddleware> logger,
        IUriBuilder uriBuilder,
        IRemoteHost remoteHost,
        IClientBuilder clientBuilder)
    {       
        _logger = logger;
        _uriBuilder = uriBuilder;
        _remoteHost = remoteHost;
        _clientBuilder = clientBuilder;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var remoteUri = await _uriBuilder.PrepareRemoteUriAsync(context);
            var remoteRequest = _remoteHost.CreateRequest(context, remoteUri);

            remoteRequest.Headers.Host = remoteUri.Host;

            var httpClient = _clientBuilder.GetClient(context, remoteUri);

            using (var remoteResponse = await httpClient.SendAsync(remoteRequest, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
            {
                _remoteHost.PrepareResponse(context, remoteResponse);

                _clientBuilder.SetResponseCookies(context, remoteUri);

                await remoteResponse.Content.CopyToAsync(context.Response.Body);
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed processing request");
            context.Response.StatusCode = 503;
        }

        return;        
    }
}

internal static class ReverseProxyMiddlewareExtensions
{
    public static  void EnableReverseProxy(this IApplicationBuilder app)
    {
        app.UseMiddleware<ReverseProxyMiddleware>();
    }
}