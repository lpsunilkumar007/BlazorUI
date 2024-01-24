using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Shared.Enums
{
    /// <summary>
    /// Maps to  FilterOperator.
    /// </summary>
    public enum FilterOperator
    {
        IsLessThan,
        IsLessThanOrEqualTo,
        IsEqualTo,
        IsNotEqualTo,
        IsGreaterThanOrEqualTo,
        IsGreaterThan,
        StartsWith,
        EndsWith,
        Contains,
        IsContainedIn,
        DoesNotContain,
        IsNull,
        IsNotNull,
        IsEmpty,
        IsNotEmpty,
        IsNullOrEmpty,
        IsNotNullOrEmpty,
    }
}
