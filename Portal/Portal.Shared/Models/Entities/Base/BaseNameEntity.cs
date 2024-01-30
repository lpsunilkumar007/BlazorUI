using Portal.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Portal.Shared.Models.Entities.Base;

public record BaseNameEntity : BaseEntity, IIdName
{
    [Required] [StringLength(50)] public virtual string Name { get; set; } = string.Empty;
}