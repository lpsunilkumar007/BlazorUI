using Portal.Shared.Models.Api;
using Portal.Shared.Models.Api.Request;
using Portal.Shared.Models.Api.Response;
//using Microsoft.AspNetCore.Components;

namespace Portal.Shared.Interfaces;

public interface IBaseApiEntityService<TEntity, in TApiRequestArgs>
     where TEntity : IEntity
     where TApiRequestArgs : ListArgs
{
    Task<CreateResponse> CreateAsync(TApiRequestArgs args, TEntity entity);

    Task<ListResponse<TEntity>> ListAsync(ApiPaginationArgs? apiPaginationArgs, TApiRequestArgs? args);
    Task<TEntity?> ViewAsync(int entityId, TApiRequestArgs? args);
    Task UpdateAsync(TApiRequestArgs? args, TEntity entity);
    Task DeleteAsync(TApiRequestArgs? args, TEntity entity);

    //EventCallback<string> OnStatusMessage { get; set; } //commented for now 
    TEntity InitNew(TApiRequestArgs? apiRequestArgs);

    public bool CanRead { get; }
    public bool CanCreate { get; }
    public bool CanUpdate { get; }
    public bool CanDelete { get; }
}
