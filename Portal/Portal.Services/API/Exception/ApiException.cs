using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Services.API
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; init; }

        public ApiException((HttpStatusCode statusCode, string message) data) : this(data.statusCode, data.message) { }
        public ApiException(HttpStatusCode statusCode) : base(statusCode.ToString()) => StatusCode = statusCode;
        public ApiException(HttpStatusCode statusCode, string errorMessage) : base(errorMessage) => StatusCode = statusCode;
        public ApiException(HttpStatusCode statusCode, string errorMessage, Exception ex) : base(errorMessage, ex) => StatusCode = statusCode;
    }
}
