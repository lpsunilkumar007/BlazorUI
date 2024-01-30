using Portal.Shared.Enums;

namespace Portal.Shared.Models.Api.Request;

public record GridFilter
{
    public FilterCompositionLogicalOperator Operator { get; set; }
    public ICollection<GridFilterValue> Items { get; set; } = new List<GridFilterValue>();
}
