using Portal.Shared.Enums;

namespace Portal.Shared.Models.Api.Request;
public record GridFilterValue
{
    public string Member { get; set; } = string.Empty;
    public FilterOperator Operator { get; set; }
    public string? Value { get; set; }
}

