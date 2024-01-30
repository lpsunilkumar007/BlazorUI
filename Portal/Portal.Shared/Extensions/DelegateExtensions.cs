using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Shared.Extensions
{
    public static class DelegateExtensions
    {
        public static Task InvokeAsync<TArgs>(this Func<TArgs, Task> func, TArgs e)
        {
            return Task.WhenAll(
                func.GetInvocationList()
                    .Cast<Func<TArgs, Task>>()
                    .Select(f => f(e)));
        }
    }
}
