using Portal.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Services.API.Response
{
    public record CreateResponse : IId
    {
        public virtual int Id { get; set; }
        public virtual string? Message { get; set; }
    }
}
