namespace Portal.Shared.Models.Api.Request;

public record UserListArgs : ListArgs
{
    public virtual bool IsValid()
    {
        return true;
    }
}

