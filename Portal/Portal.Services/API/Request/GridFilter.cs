using Portal.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Services.API.Request
{
    public record GridFilter
    {
        public FilterCompositionLogicalOperator Operator { get; set; }
        public ICollection<GridFilterValue> Items { get; set; } = new List<GridFilterValue>();
    }
}
