

using ReverseProxy.Interfaces;

namespace ReverseProxy;
public class ReverseProxyMiddleware
{
    private static readonly HttpClient _httpClient = new HttpClient();
  
    private readonly ILogger<ReverseProxyMiddleware> _logger;
    private readonly IUriBuilder _uriBuilder;
    private readonly IRemoteHost _remoteHost;

    public ReverseProxyMiddleware(
        RequestDelegate _,
        ILogger<ReverseProxyMiddleware> logger,
        IUriBuilder uriBuilder,
        IRemoteHost remoteHost)
    {       
        _logger = logger;
        _uriBuilder = uriBuilder;
        _remoteHost = remoteHost;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var remoteUri = await _uriBuilder.PrepareRemoteUriAsync(context);
            var remoteRequest = _remoteHost.CreateRequest(context, remoteUri);

            remoteRequest.Headers.Host = remoteUri.Host;

            using (var remoteResponse = await _httpClient.SendAsync(remoteRequest, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
            {
                _remoteHost.PrepareResponse(context, remoteResponse);

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