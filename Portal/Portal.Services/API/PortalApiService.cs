using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using Portal.Services.API.Base;
using Portal.Shared.Interfaces;

namespace Portal.Services.API
{
    public class PortalApiService : BaseApiService
    {
        public PortalApiService(IPortal portal, IHttpClientFactory httpClientFactory,
            ILocalStorageService localStorageService,
            ILogger<PortalApiService> logger) :
            base(portal, httpClientFactory, "PortalApiHttpClient", localStorageService, logger) { }
    }
}
