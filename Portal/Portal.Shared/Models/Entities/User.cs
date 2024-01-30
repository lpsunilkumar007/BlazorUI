using Portal.Shared.Interfaces;
using Portal.Shared.Models.Entities.Base;
using System.Text.Json.Serialization;

namespace Portal.Shared.Models.Entities
{
    public record User : BaseNameEntity, IIdName<string>
    {
        [JsonIgnore] public override int Id { get; set; }
        public override string Name { get; set; } = string.Empty;
        string IIdName<string>.Id => string.Empty;
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePictureDataUrl { get; set; }
    }
}
