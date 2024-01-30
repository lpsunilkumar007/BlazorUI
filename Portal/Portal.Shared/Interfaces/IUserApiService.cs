using Portal.Shared.Models.Api.Request;
using Portal.Shared.Models.Entities;

namespace Portal.Shared.Interfaces
{
    public interface IUserApiService : IBaseApiEntityService<User, ListArgs>
    {
    }
}
