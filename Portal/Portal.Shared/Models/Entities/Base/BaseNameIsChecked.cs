namespace Portal.Shared.Models.Entities.Base;
public record BaseNameIsChecked : BaseNameEntity
{
    public bool IsChecked { get; set; }
}