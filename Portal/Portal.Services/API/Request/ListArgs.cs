using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Services.API.Request
{
    public record ListArgs : Args
    {
        public IList<SortArg> SortList { get; set; } = new List<SortArg>();
        public IList<GridFilter> Filters { get; set; } = new List<GridFilter>();

        public string? Search { get; set; }

        public virtual bool IsValid()
        {
            return true;
        }
    }
}
