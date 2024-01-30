using Portal.Shared.Enums;

namespace Portal.Services.API.Request
{
    public record GridFilter
    {
        public FilterCompositionLogicalOperator Operator { get; set; }
        public ICollection<GridFilterValue> Items { get; set; } = new List<GridFilterValue>();
    }
}
