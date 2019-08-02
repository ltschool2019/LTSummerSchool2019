using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LTRegistrator.BLL.Contracts.Exceptions
{
    public class ResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public ResponseException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
