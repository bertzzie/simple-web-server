using System.Collections.Generic;

namespace SimpleWebServer.HTTP
{
    public enum HttpStatusCode
    {
        OK = 200,
        NotFound = 404,
        InternalServerError = 500
    }

    internal static class HttpStatusDescription
    {
        private readonly static Dictionary<int, string> descriptions = new Dictionary<int, string>()
        {
            {200, "200 OK"},
            {404, "404 File Not Found"},
            {500, "500 Internal Server Error"},
        };

        internal static string Get(HttpStatusCode status)
        {
            return Get((int)status);
        }

        internal static string Get(int status)
        {
            return descriptions[status];
        }
    }
}