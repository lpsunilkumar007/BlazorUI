using Portal.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Services.API.Request
{
    public record GridFilterValue
    {
        public  string Member { get; set; } = string.Empty;
        public  FilterOperator Operator { get; set; }
        public string? Value { get; set; }
    }
}
