
using Microsoft.AspNetCore.Http;

namespace ReverseProxy.Interfaces;
public interface IUriBuilder
{
    Task<Uri> PrepareRemoteUriAsync(HttpContext context);
}
