
using Microsoft.AspNetCore.Http;
using ReverseProxy.Interfaces;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.Extensions;


namespace ReverseProxy.Lib
{
    public class PathBasedUriBuilderService : IUriBuilder
    {
        private readonly IHostMapsRepository _repository;
        public PathBasedUriBuilderService(IHostMapsRepository repository)
        {
            _repository = repository;
        }
        public async Task<Uri> PrepareRemoteUriAsync(HttpContext context)
        {
            var maps = await _repository.GetInboundOutboundMapAsync();
            var path = context.Request.Path;

            foreach (string key in maps.Keys)
            {
                if(path.StartsWithSegments(key, out var remainingPath))
                {
                    return new Uri($"{maps[key]}{remainingPath}");
                }
            }

            throw new InvalidOperationException($"Unable to locate configuration for the path {path.Value}");

            
        }
    }
}
