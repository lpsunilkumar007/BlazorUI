using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Shared.Routes
{
    public static class TokenEndpoints
    {
        public static string Get = "api/identity/token";
        public static string Refresh = "api/identity/token/refresh";
    }
}
