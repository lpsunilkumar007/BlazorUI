using Portal.Shared.Models.Entities.Base;

namespace Aura.Portal.Shared.Models.Entities.Base;

public record EnumEntity<TEnum> : BaseNameEntity
    where TEnum : struct
{
    public TEnum Value { get; init; }

    public EnumEntity()
    {
    }

    public EnumEntity(TEnum item)
    {
        Value = item;
        //Id = item.GetValue();
        //Name = item.GetDescription();
    }
}