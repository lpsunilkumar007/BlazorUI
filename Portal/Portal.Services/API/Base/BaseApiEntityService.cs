using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Portal.Shared.Models.Api.Request;
using Portal.Shared.Models.Api.Response;
using Portal.Shared;
using Portal.Shared.Models;
using Portal.Shared.Models.Api;
using System.Net.Http.Json;
using Portal.Shared.Extensions;
namespace Portal.Services.API.Base
{
    internal abstract class BaseApiEntityService<TEntity, TApiArgs, TCreateResponse, TListResponse, TUpdateResponse>
       where TEntity : IEntity, new()
       where TListResponse : ListResponse<TEntity>, new()
       where TCreateResponse : CreateResponse
       where TUpdateResponse : UpdateResponse
       where TApiArgs : Args, new()
    {
        public EventCallback<string> OnStatusMessage { get; set; }

        protected PortalApiService PortalApiService { get; init; }
        protected ILogger<BaseApiEntityService<TEntity, TApiArgs, TCreateResponse, TListResponse, TUpdateResponse>> Logger { get; init; }

        protected BaseApiEntityService(PortalApiService PortalApiService, ILogger<BaseApiEntityService<TEntity, TApiArgs, TCreateResponse, TListResponse, TUpdateResponse>> logger)
        {
            Logger = logger;
            PortalApiService = PortalApiService ?? throw new ArgumentNullException(nameof(PortalApiService));
        }

        public virtual TEntity InitNew(TApiArgs? args)
        {
            return new TEntity();
        }

        protected abstract AvailableCrudActions GetAvailableCrudActions();

        public bool CanCreate => GetAvailableCrudActions().CanCreate;
        public bool CanRead => GetAvailableCrudActions().CanRead;
        public bool CanUpdate => GetAvailableCrudActions().CanUpdate;
        public bool CanDelete => GetAvailableCrudActions().CanDelete;


        #region Create methods
        protected virtual string GetCreateEndpoint(TApiArgs? args, TEntity entity)
        {
            throw new NotImplementedException($"{GetType()}.{nameof(GetCreateEndpoint)} has not been implemented.");
        }

        protected virtual object GetCreateObject(TApiArgs? args, TEntity entity)
        {
            return entity;
        }

        public virtual async Task<CreateResponse> CreateAsync(TApiArgs? args, TEntity entity)
        {
            return await CreateAsync<CreateResponse>(args, entity);
        }

        protected virtual HttpContent? GetCreateHttpContent(TApiArgs? args, TEntity entity)
        {
            return null;
        }

        public virtual async Task<TResponse> CreateAsync<TResponse>(TApiArgs? args, TEntity entity)
            where TResponse : CreateResponse
        {
            var createEndpoint = GetCreateEndpoint(args, entity);
            var createObject = GetCreateObject(args, entity);

            try
            {
                var responseMessage = await PortalApiService.PostAsync(createEndpoint, createObject, GetCreateHttpContent(args, entity));
                var response = await responseMessage.Content.ReadFromJsonAsync<TResponse>();
                if (response == null)
                {
                    throw new Exception("ReadFromJsonAsync<TApiCreateResponse> returned null");
                }

                return response;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{jsonObject}", createObject);
                throw;
            }
        }
        #endregion

        #region Read methods
        protected abstract string GetListEndpoint(TApiArgs? args);
        protected virtual string GetListQueryString(ApiPaginationArgs apiPaginationArgs, TApiArgs? args)
        {
            if (apiPaginationArgs == null)
                throw new ArgumentNullException(nameof(apiPaginationArgs));

            var endpoint = GetListEndpoint(args);
            if (string.IsNullOrEmpty(endpoint))
            {
                return string.Empty;
            }

            if (apiPaginationArgs is { Page: > 0, PageSize: > 0 })
            {
                endpoint = endpoint
                    .AddQueryStringParameter("page", apiPaginationArgs.Page)
                    .AddQueryStringParameter("pageSize", apiPaginationArgs.PageSize);
            }
            else
            {
                endpoint = endpoint.AddQueryStringParameter("page", -1);
            }

            if (args is ListArgs listArgs)
            {
                endpoint = endpoint
                    .AddQueryStringParameter("sort", listArgs.SortList.ToCommaString())
                    .AddQueryStringParameter("filter", listArgs.Filters.ToJsonBase64String())
                    .AddQueryStringParameter("Search", listArgs.Search);
            }

            return endpoint;
        }
        protected virtual string GetViewEndpoint(int entityId, TApiArgs? args)
        {
            throw new NotImplementedException($"{GetType()}.{nameof(GetViewEndpoint)} has not been implemented.");
        }

        protected virtual IList<TEntity> ConfigureListItems(IList<TEntity> items, TApiArgs? args)
        {
            return items;
        }

        public virtual async Task<ListResponse<TEntity>> ListAsync(ApiPaginationArgs? apiPaginationArgs, TApiArgs? args)
        {
            apiPaginationArgs ??= new ApiPaginationArgs { Page = -1, PageSize = -1 };

            var endpoint = GetListQueryString(apiPaginationArgs, args);

            //Logger.LogInformation("{Type}: {Endpoint}", GetType().Name, endpoint);
            await OnStatusMessage.InvokeAsync($"API: {endpoint} : Reading");

            var response = await PortalApiService.GetAsync<TListResponse>(endpoint);

            //if (typeof(IBrandId).IsAssignableFrom(typeof(TEntity)) || typeof(IStoreId).IsAssignableFrom(typeof(TEntity)))
            //{
            //    if (response.Items != null)
            //    {
            //        foreach (var item in response.Items)
            //        {
            //            if (args is IBrandId argBrand && item is IBrandId tmpBrand)
            //                tmpBrand.BrandId = argBrand.BrandId;
            //            if (args is IStoreId argStore && item is IStoreId tmpStore)
            //                tmpStore.StoreId = argStore.StoreId;
            //        }
            //    }
            //}

            if (response.Items != null)
                response.Items = ConfigureListItems(response.Items, args); // Give subclasses a chances to sort, filter, etc before returning to client.

            await OnStatusMessage.InvokeAsync($"API: {endpoint} : Done");

            return response;
        }

        public virtual async Task<TEntity?> ViewAsync(int entityId, TApiArgs? args)
        {
            var endpoint = GetViewEndpoint(entityId, args);
            return await PortalApiService.GetAsync<TEntity>(endpoint);
        }
        #endregion

        #region Update methods
        protected virtual string? GetUpdateEndpoint(TApiArgs? args, TEntity entity)
        {
            return null;
        }

        protected virtual object GetUpdateObject(TApiArgs? args, TEntity entity)
        {
            return entity;
        }

        protected virtual HttpContent? GetUpdateHttpContent(TApiArgs? args, TEntity entity)
        {
            return null;
        }

        public virtual async Task UpdateAsync(TApiArgs? args, TEntity entity)
        {
            var uri = GetUpdateEndpoint(args, entity);
            if (string.IsNullOrEmpty(uri))
            {
                throw new Exception($"{GetType().Name}.UpdateAsync: It does not appear GetUpdateEndpoint has been implemented as it is returning a null or blank value");
            }

            object updateObject = GetUpdateObject(args, entity);
            try
            {
                await PortalApiService.PutAsync(uri, updateObject, GetUpdateHttpContent(args, entity));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{jsonObject}", updateObject);
                throw;
            }
        }

        #endregion

        #region Delete methods
        protected virtual string GetDeleteEndpoint(TApiArgs? args, TEntity entity)
        {
            throw new NotImplementedException($"{GetType()}.{nameof(GetDeleteEndpoint)} has not been implemented.");
        }

        public async Task DeleteAsync(TApiArgs? args, TEntity entity)
        {
            await PortalApiService.DeleteAsync(GetDeleteEndpoint(args, entity));
        }

        #endregion
    }

    internal abstract class BaseApiEntityService<TEntity, TApiArgs> :
        BaseApiEntityService<TEntity, TApiArgs, CreateResponse, ListResponse<TEntity>, UpdateResponse>
        where TEntity : IEntity, new()
        where TApiArgs : Args, new()
    {
        protected BaseApiEntityService(PortalApiService PortalApiService, ILogger<BaseApiEntityService<TEntity, TApiArgs, CreateResponse, ListResponse<TEntity>, UpdateResponse>> logger) : base(PortalApiService, logger)
        {
        }
    }

    internal abstract class BaseApiEntityService<TEntity, TApiArgs, TCreateResponse, TListResponse> :
        BaseApiEntityService<TEntity, TApiArgs, TCreateResponse, TListResponse, UpdateResponse>
        where TEntity : IEntity, new()
        where TListResponse : ListResponse<TEntity>, new()
        where TCreateResponse : CreateResponse
        where TApiArgs : Args, new()
    {
        protected BaseApiEntityService(PortalApiService PortalApiService, ILogger<BaseApiEntityService<TEntity, TApiArgs, TCreateResponse, TListResponse, UpdateResponse>> logger) : base(PortalApiService, logger)
        {
        }
    }
}
