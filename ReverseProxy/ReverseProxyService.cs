namespace ReverseProxy;
public class ReverseProxyService
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private readonly RequestDelegate _nextAsync;
    private readonly ILogger<ReverseProxyService> _logger;

    public ReverseProxyService(RequestDelegate nextAsync, ILogger<ReverseProxyService> logger)
    {
        _nextAsync = nextAsync;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var remoteUri = PrepareRemoteUri(context);
        var remoteRequest = CreateRemoteRequest(context, remoteUri);

        using (var remoteResponse = await _httpClient.SendAsync(remoteRequest, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
        {
            await CreateClientResponseAsyc(context, remoteResponse);            
        }

        return;        
    }

    private Uri PrepareRemoteUri(HttpContext context)
    {        
        var uri = new Uri($"https://www.intel.com/{context.Request.Path}");

        _logger.LogInformation(uri.AbsoluteUri);

        return uri;
    }

    private HttpRequestMessage CreateRemoteRequest(HttpContext context, Uri remoteUri)
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
        
        
        request.Headers.Host = remoteUri.Host;

        return requestMessage;
       
    }

    private async Task CreateClientResponseAsyc(HttpContext context, HttpResponseMessage remoteResponse)
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

        await remoteResponse.Content.CopyToAsync(context.Response.Body);
    }
}
