using Microsoft.AspNetCore.Components;
using Portal.Shared.Models.Api.Request;
using Portal.Shared;
using Portal.Shared.Interfaces;
using Portal.Services.Managers;
using Portal.Shared.Extensions;
using Portal.Shared.Models.Api;
using System.Collections.ObjectModel;
namespace Portal.UI.Shared.Components.Base;
public abstract class LPBaseEntityComponent<TEntity, TApiService, TListArgs> : ComponentBase
    where TEntity : class, IEntity, new()
    where TApiService : class, IBaseApiEntityService<TEntity, TListArgs>
    where TListArgs : ListArgs, new()
{
    [Inject] public ILogger<LPBaseEntityComponent<TEntity, TApiService, TListArgs>> Logger { get; private set; } = default!;
    [Inject] public TApiService ApiService { get; private set; } = default!;
    [Inject] public ApplicationStateManager ApplicationStateManager { get; init; } = default!;

    protected int Page { get; set; } = -1;
    protected int PageSize { get; set; } = 10;

    public ObservableCollection<TEntity>? Items { get; set; }

    [Parameter] public TListArgs ListArgs { get; set; } = new();
    [Parameter] public bool IsPageable { get; set; }

    protected virtual Task OnDataRefreshedAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Refreshes the Items collection
    /// </summary>
    /// <returns>Total records on the server for the passed filter/sort/etc...</returns>
    protected virtual async Task<int> RefreshItemsAsync()
    {
        try
        {
            ApplicationStateManager.IsLoading = true;

            Items = new ObservableCollection<TEntity>();

            if (!ListArgs.IsValid())
            {
                await OnDataRefreshedAsync();
                return 0;
            }

            var paginationArgs = new ApiPaginationArgs
            {
                Page = IsPageable ? Page : -1, // If < 0, then it will be omitted from the query string.
                PageSize = PageSize
            };

            var response = await ApiService.ListAsync(paginationArgs, ListArgs);
            if (response == null)
            {
                Logger.LogWarning("{type}.{method} : Request response is null. Aborting..", GetType().Name, nameof(RefreshItemsAsync));
                return 0;
            }

            if (response.Items != null)
                Items = MapItemsToDataSource(response.Items);

            await OnDataRefreshedAsync();

            var totalRecords = response?.Pagination?.TotalRecords ?? 0;
            var itemCount = Items?.Count ?? 0;

            return totalRecords > 0 ? totalRecords : itemCount; // args.Total in OnRead MUST have a Total, otherwise it just won't display anything.
        }
        catch (Exception ex)
        {
            //await CoMessagingCentre.Exception.SendAsync(ex);
            return 0;
        }
        finally
        {
            ApplicationStateManager.IsLoading = false;
        }
    }

    protected virtual ObservableCollection<TEntity> MapItemsToDataSource(IList<TEntity> items)
    {
        return new ObservableCollection<TEntity>(items);
    }

    protected virtual Task OnListArgsChanged(TListArgs args)
    {
        return Task.CompletedTask;
    }


    private TListArgs? _previousListArgs = default;
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        // Console.WriteLine($"{typeof(TEntity).Name} 1: We have a change in args. {ListArgs}");

        if (ListArgs.IsEqual(_previousListArgs))
            return;
        _previousListArgs = ListArgs.GetCopy();

        // Console.WriteLine($"{typeof(TEntity).Name} 2: We have a change in args. {ListArgs}");
        await OnListArgsChanged(ListArgs);
    }
}
