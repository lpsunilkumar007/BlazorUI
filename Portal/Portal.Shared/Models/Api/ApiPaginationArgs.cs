using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Shared.Models.Api
{
    public record ApiPaginationArgs
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }
}
