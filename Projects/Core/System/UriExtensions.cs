using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace System
{
    /// <summary>
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Извлекает QUERY-параметры из адреса <paramref name="uriWithQueryParams"/> и добавляет к исходному адресу <paramref name="uri"/>.
        /// </summary>
        /// <returns>Новый адрес на основе <paramref name="uri"/> и QUERY-параметров из <paramref name="uriWithQueryParams"/>.</returns>
        public static Uri AddQueryFrom(this Uri uri, Uri uriWithQueryParams)
        {
            var uriBuilder = new UriBuilder(uri);
            var querySource = HttpUtility.ParseQueryString(uriBuilder.Query);
            var queryWithQueryParams = HttpUtility.ParseQueryString(uriWithQueryParams.Query);
            foreach (var key in queryWithQueryParams.AllKeys) querySource[key] = queryWithQueryParams[key];
            uriBuilder.Query = querySource.ToString();
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Возвращает новый экземпляр адреса, добавляя к нему query-параметр <paramref name="paramName"/> со значением <paramref name="paramValue"/>.
        /// </summary>
        public static Uri AddParameter(this Uri url, string paramName, string paramValue)
        {
            var baseUrl = new Uri("http://localhost");
            var urlString = url.ToString();
            var uriBuilder = url.IsAbsoluteUri ? new UriBuilder(url) : new UriBuilder(new Uri(baseUrl, urlString));
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[paramName] = paramValue;
            uriBuilder.Query = query.ToString();

            if (url.IsAbsoluteUri)
            {
                return uriBuilder.Uri;
            }
            else
            {
                var prefix = "";
                if (urlString.First() == '/') prefix = "/";
                if (urlString.First() == '\\') prefix = "\\";

                var d = baseUrl.MakeRelativeUri(uriBuilder.Uri);
                return new Uri(prefix +  d.ToString(), UriKind.Relative);
            }
        }

        public static string MakeRelativeFromUrl(string url)
        {
            Uri uri = null;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                return uri.PathAndQuery;
            }
            else
            {
                return "/" + url.TrimStart('/');
            }
        }

    }
}
