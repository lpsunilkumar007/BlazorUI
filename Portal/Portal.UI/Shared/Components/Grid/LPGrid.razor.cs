using Microsoft.AspNetCore.Components;
using MudBlazor;
using Portal.Shared.Models.Api.Request;
using Portal.Shared;
using Portal.Shared.Interfaces;
using Portal.UI.Shared.Components.Base;
using static MudBlazor.CategoryTypes;
using System.Net.Http;

namespace Portal.UI.Shared.Components.Grid
{
    public partial class LPGrid<TEntity, TApiService, TListArgs> : LPBaseEntityComponent<TEntity, TApiService, TListArgs>
       where TEntity : class, IEntity, new()
       where TApiService : class, IBaseApiEntityService<TEntity, TListArgs>
       where TListArgs : ListArgs, new()
    {
        #region Parameters
        [Parameter] public bool ShowAddButton { get; set; } = true;
        [Parameter] public bool ShowEditButton { get; set; } = true;
        [Parameter] public bool ShowDeleteButton { get; set; } = false;
        [Parameter] public bool ShowExcelButton { get; set; } = true;
        [Parameter] public bool ShowCsvButton { get; set; } = true;
        [Parameter] public RenderFragment? GridToolBar { get; set; }
        [Parameter] public RenderFragment? LPColumns { get; set; }
        [Parameter] public RenderFragment<TEntity>? RowTemplate { get; set; }
        [Parameter] public RenderFragment<TEntity>? DetailTemplate { get; set; }
        //[Parameter] public RenderFragment<FormItemsTemplateContext>? FormItemsTemplate { get; set; }
        [Parameter] public IEnumerable<TEntity>? SelectedItems { get; set; } = new List<TEntity>();
        [Parameter] public EventCallback<IEnumerable<TEntity>> SelectedItemsChanged { get; set; }

        //[Parameter] public string Width { get; set; }
        //[Parameter] public Action<GridRowRenderEventArgs>? OnRowRender { get; set; }
        //[Parameter] public EventCallback<GridCommandEventArgs> OnEdit { get; set; }
        [Parameter] public string PopupUpWindowWidth { get; set; } = "32rem";
        /// <summary>
        /// Optional setting. If not set, then the namespace + typename is used as a key, but is not unique. Set the StorageKey to have uniqueness.
        /// </summary>
        //[Parameter] public string? StorageKey { get; set; }

        /// <summary>
        /// Saves the current state to local storage each time a change is detected.
        /// </summary>
        //[Parameter] public bool PersistState { get; set; } = false;
        //[Inject] public ILocalStorageService LocalStorageService { get; set; }
        //[Parameter] public GridEditMode EditMode { get; set; } = GridEditMode.Incell;
        //[Parameter] public bool IsEditable { get; set; } = true;
        //[Parameter] public GridSelectionMode SelectionMode { get; set; } = GridSelectionMode.Single;
        //[Parameter] public GridFilterMode FilterMode { get; set; } = GridFilterMode.FilterMenu;
        //[Parameter] public bool ShowColumnMenu { get; set; } = true;
        //[Parameter] public bool Sortable { get; set; } = false;
        //[Parameter] public SortMode SortMode { get; set; } = SortMode.Multiple;
        //[Parameter] public EventCallback<CreateResponse> OnCreated { get; set; }

        [Parameter] public RenderFragment<TEntity>? InsertFormItems { get; set; }
        [Parameter] public string FormTitle { get; set; } = "New Item";
        [Parameter] public EventCallback<TEntity> OnBeforeCreate { get; set; }
        [Parameter] public EventCallback<TEntity> OnBeforeUpdate { get; set; }
        [Parameter] public bool AutoShowName { get; set; } = true;
        [Parameter] public bool IsNameEditable { get; set; } = true;
        [Parameter] public bool AutoShowNameForForm { get; set; } = true;
        [Parameter] public bool RefreshDataOnEditFormCancel { get; set; }

        //[Parameter] public RenderFragment? ChildContent { get; set; }
        //[Parameter] public TelerikTextBox? FocusElement { get; set; }
        [Parameter] public bool AutoGenerateColumns { get; set; }
        [Parameter] public bool ConfirmOnClose { get; set; } = false;
        #endregion

        private string _manualShowWindowFormTitle = string.Empty;
        private bool _isDataWindowVisible = false;
        private MudDataGrid<TEntity>? _grid;
        private bool _isInEditMode = false;
        //private TelerikTextBox? _internalFocusElement;
        //private EditContext? _formDataContext;

        private bool _isConfirmWindowVisible = false;
        private object _dataBeforeUpdate { get; set; }
        public async Task RefreshGridAsync()
        {
            _grid?.ReloadServerData();
            await Task.CompletedTask;
        }

        public async Task<MudDataGrid<TEntity>> GetCurrentGridAsync()
        {
            return _grid;

        }

        public LPGrid()
        {
            IsPageable = true;
            Page = 1;
        }

        private async Task OnSelectedItemsChanged(IEnumerable<TEntity> items)
        {
            var noChange = items.Equals(SelectedItems);
            if (noChange)
                return;

            SelectedItems = items;
            await SelectedItemsChanged.InvokeAsync(SelectedItems);
        }

        protected bool CanShowAddButton()
        {
            return ApiService.CanCreate && ShowAddButton && ListArgs.IsValid();
            //&& IsEditable;
        }

        protected bool CanShowEditButton()
        {
            return ApiService.CanUpdate && ShowEditButton;
            //&& EditMode == GridEditMode.Inline && IsEditable;
        }
        protected bool CanShowDeleteButton()
        {
            return ApiService.CanDelete && ShowDeleteButton;
            //&& IsEditable;
        }

        protected override async Task OnListArgsChanged(TListArgs args)
        {
            await base.OnListArgsChanged(args);
            Page = 1;
            await RefreshGridAsync();
        }

        
        private async Task<GridData<TEntity>> LoadGridData(GridState<TEntity> state)
        {
            var totalItems = await RefreshItemsAsync();
            GridData<TEntity> data = new()
            {
                Items = Items,
                TotalItems = totalItems
            };

            return data;
        }

        //private async Task OnRead(GridState<TEntity> state, GridData<TEntity> data)
        //{
        //    _isInEditMode = false;

        //    if (!ListArgs.IsValid())
        //    {
        //        data.Items = new List<TEntity>();
        //        return;
        //    }

        //    ListArgs.SortList.Clear();
        //    //if (args is { Request.Sorts.Count: > 0 })
        //    //{
        //    //    foreach (var sort in args.Request.Sorts)
        //    //    {
        //    //        ListArgs.SortList.Add(new SortArg(sort.Member, sort.SortDirection == ListSortDirection.Ascending));
        //    //    }
        //    //}

        //    ListArgs.Filters.Clear();
        //    //if (args is { Request.Filters.Count: > 0 })
        //    {
        //        //foreach (var compositeFilter in args.Request.Filters
        //        //             .Where(x => x.GetType() == typeof(CompositeFilterDescriptor))
        //        //             .Cast<CompositeFilterDescriptor>())
        //        //{
        //        //    var gridFilter = new GridFilter
        //        //    {
        //        //        Operator = (FilterCompositionLogicalOperator)(int)compositeFilter.LogicalOperator
        //        //    };

        //        //    foreach (var filterDescriptor in compositeFilter.FilterDescriptors.Cast<FilterDescriptor>())
        //        //    {
        //        //        gridFilter.Items.Add(new GridFilterValue
        //        //        {
        //        //            Member = filterDescriptor.Member,
        //        //            Operator = (FilterOperator)(int)filterDescriptor.Operator,
        //        //            Value = filterDescriptor.Value.ToString()
        //        //        });
        //        //    }

        //        //    ListArgs.Filters.Add(gridFilter);
        //        //}
        //    }

        //    data.TotalItems = await RefreshItemsAsync();
        //    data.Items = Items;

        //    // Assign data else getting null 
        //    if (_grid != null)
        //    {
        //        _grid.Items = Items;
        //    }

        //    await InvokeAsync(StateHasChanged);
        //}


        //private async Task OnCreate(GridCommandEventArgs args)
        //{
        //    try
        //    {
        //        if (EditMode == GridEditMode.Incell)
        //        {
        //            Console.WriteLine("OnCreate called when Incell editing is true. Exiting..");
        //            return;
        //        }

        //        if (args.Item is not TEntity entity)
        //            throw new ArgumentNullException(nameof(args));

        //        var response = await ApiService.CreateAsync(ListArgs, entity);

        //        await OnCreated.InvokeAsync(response);
        //    }
        //    catch (ApiException ex)
        //    {
        //        await CoMessagingCentre.ApiException.SendAsync(new ApiException(ex.StatusCode, ex.StatusCode == HttpStatusCode.Conflict ? "Record exists already!" : ex.Message));
        //    }
        //    catch (Exception ex)
        //    {
        //        await CoMessagingCentre.Exception.SendAsync(ex);
        //    }
        //}

        //private TEntity? _onEditBackup;
        //private async Task OnEditHandler(GridCommandEventArgs args)
        //{
        //    if (!IsEditable)
        //    {
        //        Logger.LogWarning("Editing has been disabled");
        //        args.IsCancelled = true;
        //        return;
        //    }

        //    if (!ApiService.CanUpdate)
        //    {
        //        await CoMessagingCentre.WarningMessage.SendAsync("Edit permission denied");
        //        Logger.LogWarning("Edit permission denied");
        //        args.IsCancelled = true;
        //        return;
        //    }

        //    if (OnEdit.HasDelegate)
        //    {
        //        await OnEdit.InvokeAsync(args);
        //        if (args.IsCancelled)
        //            return;
        //    }

        //    // If here, then we can edit no problem. Create a backup for if the user cancels.

        //    _onEditBackup = args.Item as TEntity;
        //    _focusFlag = true;
        //    _isInEditMode = true;
        //}

        //private bool _focusFlag = false;
        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    var elementToFocus = FocusElement ?? _internalFocusElement;
        //    if (elementToFocus == null)
        //    {
        //        return;
        //    }

        //    if (_isDataWindowVisible && _focusFlag)
        //    {
        //        _focusFlag = false;

        //        await Task.Delay(200); // Need a little time for the DOM to render, then we can set focus.
        //        await elementToFocus.FocusAsync();
        //    }
        //}

        //private async Task OnUpdate(GridCommandEventArgs args)
        //{
        //    try
        //    {
        //        if (args.Item is not TEntity entity)
        //        {
        //            throw new ArgumentNullException(nameof(args));
        //        }

        //        if (_onEditBackup != null && _onEditBackup.Equals(entity)) // No changes made. Just exit.
        //        {
        //            return;
        //        }

        //        await ApiService.UpdateAsync(ListArgs, entity);
        //        _onEditBackup = entity;
        //        _isInEditMode = false;
        //    }
        //    catch (ApiException ex)
        //    {
        //        await CoMessagingCentre.ApiException.SendAsync(ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        await CoMessagingCentre.Exception.SendAsync(ex);
        //    }
        //}

        //private async Task OnDelete(GridCommandEventArgs args)
        //{
        //    try
        //    {
        //        if (args.Item is not TEntity entity)
        //            throw new ArgumentNullException(nameof(args));

        //        await ApiService.DeleteAsync(ListArgs, entity);

        //        if (Items != null)
        //        {
        //            Items.Remove(entity);
        //            Items = new ObservableCollection<TEntity>(Items);
        //        }

        //        _isInEditMode = false;
        //        Logger.LogInformation("OnDelete: {entity}", entity);
        //    }
        //    catch (Exception ex)
        //    {
        //        await CoMessagingCentre.Exception.SendAsync(ex);
        //    }
        //}

        //private void OnCancel(GridCommandEventArgs obj)
        //{
        //    _isInEditMode = false;
        //}

        //private string GetStateStorageKey()
        //{
        //    return !string.IsNullOrEmpty(StorageKey) ? StorageKey : $"{GetType().Namespace}.{GetType().Name}";
        //}

        //private Task OnGridStateInit(GridStateEventArgs<TEntity> args)
        //{
        //    //if (!PersistState)
        //    //    return;

        //    //try
        //    //{
        //    //    var state = await LocalStorageService.GetItemAsync<GridState<TEntity>>(GetStateStorageKey(), CancellationToken.None);
        //    //    if (state != null)
        //    //        args.GridState = state;
        //    //}
        //    //catch (InvalidOperationException ex)
        //    //{
        //    //    Logger.LogError(ex, "Failed to read grid state");
        //    //}

        //    return Task.CompletedTask;
        //}

        //private string GetFormTitle()
        //{
        //    return !string.IsNullOrEmpty(_manualShowWindowFormTitle) ? _manualShowWindowFormTitle : FormTitle;
        //}

        //private async Task OnAddViaFormClick(GridCommandEventArgs obj)
        //{
        //    await InvokeAddNewWindowAsync();
        //}


        //public async Task InvokeAddNewWindowAsync(string windowTitle = "")
        //{
        //    _manualShowWindowFormTitle = windowTitle;

        //    var newEntity = ApiService.InitNew(ListArgs);

        //    await OnBeforeCreate.InvokeAsync(newEntity);
        //    _formDataContext = new EditContext(newEntity);
        //    _dataBeforeUpdate = _formDataContext.Model.GetCopy();
        //    _isDataWindowVisible = true;
        //    _focusFlag = true;
        //}

        //public async Task InvokeEditWindowAsync(TEntity selectedEntity, string windowTitle = "")
        //{
        //    _manualShowWindowFormTitle = windowTitle;
        //    await OnBeforeUpdate.InvokeAsync(selectedEntity);
        //    _formDataContext = new EditContext(selectedEntity);
        //    _dataBeforeUpdate = _formDataContext.Model.GetCopy();
        //    _isDataWindowVisible = true;
        //    _focusFlag = true;
        //}

        //private async Task OnFormSubmitAsync()
        //{
        //    _isDataWindowVisible = false;

        //    if (_formDataContext?.Model is not TEntity entity)
        //        throw new Exception("_formDataContext.Model is not a supported type");

        //    try
        //    {
        //        _appStateManager.IsLoading = true;

        //        if (entity.Id <= 0)
        //        {
        //            var response = await ApiService.CreateAsync(ListArgs, entity);
        //            await OnCreated.InvokeAsync(response);
        //        }
        //        else
        //            await ApiService.UpdateAsync(ListArgs, entity);

        //        _manualShowWindowFormTitle = string.Empty;
        //        await RefreshGridAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        await CoMessagingCentre.Exception.SendAsync(ex);
        //    }
        //    finally
        //    {
        //        _appStateManager.IsLoading = false;
        //    }
        //}

        //async Task OnFormCancelAsync(MouseEventArgs obj)
        //{
        //    if (RefreshDataOnEditFormCancel)
        //    {
        //        await RefreshGridAsync();
        //    }

        //    _isDataWindowVisible = false;
        //}

        //public RenderFragment<TEntity>? GetRowTemplate()
        //{
        //    return RowTemplate;
        //}

        //public RenderFragment<TEntity>? GetDetailTemplate()
        //{
        //    return DetailTemplate;
        //}

        //public RenderFragment<FormItemsTemplateContext>? GetFormItemsTemplate()
        //{
        //    return FormItemsTemplate;
        //}


        //async Task ResetState()
        //{
        //    await LocalStorageService.RemoveItemAsync(GetStateStorageKey());
        //    if (_grid != null)
        //        await _grid.SetState(null); // pass null to reset the state
        //}

        //private async Task SavePageSizeAsync()
        //{
        //    await LocalStorageService.SetItemAsync("LPGrid_PageSize", PageSize, CancellationToken.None);
        //}

        // This was being call in OnInitializedAsync (in case we need it again). 
        //private async Task LoadPageSizeAsync()
        //{
        //    var pageSize = await LocalStorageService.GetItemAsync<int>("LPGrid_PageSize", CancellationToken.None);
        //    PageSize = pageSize > 0 ? pageSize : Defaults.GridPageSize;
        //}

        //public GridState<TEntity> GetState()
        //{
        //    return _grid.GetState();
        //}

        //public async Task SetState(GridState<TEntity> state)
        //{
        //    await _grid.SetState(state);
        //}

        //#region Confirm Window
        //private async Task CloseEditAndConfirmWindow(MouseEventArgs obj)
        //{
        //    _isConfirmWindowVisible = false;
        //    await OnFormCancelAsync(obj);
        //}
        //private async Task CloseConfirmWidnow(MouseEventArgs obj)
        //{
        //    _isConfirmWindowVisible = false;
        //}

        //private async Task CanCloseEditWindow(MouseEventArgs obj)
        //{
        //    if (ConfirmOnClose)
        //    {
        //        if (_formDataContext.IsModified() || !_dataBeforeUpdate.IsEqual(_formDataContext.Model))
        //        {
        //            _isConfirmWindowVisible = true;
        //        }
        //        else
        //        {
        //            _isDataWindowVisible = false;
        //        }
        //    }
        //    else
        //    {
        //        await OnFormCancelAsync(obj);
        //    }
        //}

        //#endregion
    }
}
