using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Shared.Models
{
    public record AvailableCrudActions(bool CanCreate, bool CanRead, bool CanUpdate, bool CanDelete = false);
}
