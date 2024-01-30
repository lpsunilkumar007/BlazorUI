namespace Portal.Shared.Models.Api.Response;

public record ListResponse<TEntitySummary>
     where TEntitySummary : IEntity
{
    public Pagination Pagination { get; set; } = new();
    public virtual IList<TEntitySummary>? Items { get; set; } = new List<TEntitySummary>();
}
