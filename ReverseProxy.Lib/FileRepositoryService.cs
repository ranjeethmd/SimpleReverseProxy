using ReverseProxy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseProxy.Lib
{
    public class FileRepositoryService : IHostMapsRepository
    {
        public async Task<IDictionary<string, string>> GetInboundOutboundMapAsync()
        {
            return await Task.FromResult(new Dictionary<string, string>{ { "intel", "www.intel.com" }, { "google", "www.google.com" } } );
        }
    }
}
