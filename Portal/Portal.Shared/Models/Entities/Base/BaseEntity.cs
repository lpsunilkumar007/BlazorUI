using System.ComponentModel.DataAnnotations;

namespace Portal.Shared.Models.Entities.Base;

public abstract record BaseEntity : IEntity
{
    [Key, Editable(false)]
    public virtual int Id { get; set; }
}