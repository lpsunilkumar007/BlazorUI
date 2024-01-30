using Portal.Shared.Interfaces;

namespace Portal.Shared.Models.Api.Response;
public record CreateResponse : IId
{
    public virtual int Id { get; set; }
    public virtual string? Message { get; set; }
}

