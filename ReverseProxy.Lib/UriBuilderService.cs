
using Microsoft.AspNetCore.Http;
using ReverseProxy.Interfaces;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.Extensions;

namespace ReverseProxy.Lib;
/// <summary>
/// Subdomain based UriBuilder
/// </summary>

public class UriBuilderService : IUriBuilder
{
    private readonly IHostMapsRepository _repository;
    public UriBuilderService(IHostMapsRepository repository)
    {
        _repository = repository;
    }
    public async Task<Uri> PrepareRemoteUriAsync(HttpContext context)
    {
        var maps = await _repository.GetInboundOutboundMapAsync();
        var path = context.Request.GetEncodedUrl();
        var host = context.Request.Host.Value;

        var key = maps.Keys.First(k => Regex.IsMatch(host, k));

        var regex = new Regex(host, RegexOptions.IgnoreCase);

        path = regex.Replace(path, maps[key], 1);

        return new Uri(path);
    }
}
