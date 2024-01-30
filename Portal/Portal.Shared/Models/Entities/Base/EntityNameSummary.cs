using Portal.Shared.Interfaces;

namespace Portal.Shared.Models.Entities.Base;
public class EntityNameSummary : EntitySummary, IName
{
    public virtual string Name { get; set; } = "";
}