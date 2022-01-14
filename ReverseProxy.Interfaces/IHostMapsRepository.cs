
namespace ReverseProxy.Interfaces;

public interface IHostMapsRepository
{
    Task<IDictionary<string, string>> GetInboundOutboundMapAsync();
}
