using Portal.Services.API.Base;
using Portal.Shared.Interfaces;
using Portal.Shared.Models.Api.Response;
using Portal.Shared.Models.Api.Request;
using Portal.Shared.Models.Entities;
using Portal.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Portal.Services.API
{
    internal class UserApiService : BaseApiEntityService<User, ListArgs, CreateResponse, ListResponse<User>>, IUserApiService
    {
        public UserApiService(PortalApiService PortalApiService, ILogger<BaseApiEntityService<User, ListArgs, CreateResponse, ListResponse<User>, UpdateResponse>> logger) : base(PortalApiService, logger)
        {
        }

       

        protected override AvailableCrudActions GetAvailableCrudActions()
        {
            return new AvailableCrudActions(true, true, true);
        }

        protected override string GetListEndpoint(ListArgs? args)
        {
            return "/users";
        }

       
    }
}
