using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroNetCore.AspNetCore.Proxy
{
    /// <summary>
    ///     Default implementation of <see cref="T:MicroNetCore.AspNetCore.Proxy.IProxy" />.
    /// </summary>
    public sealed class Proxy : IProxy
    {
        private readonly ProxyOptions _proxyOptions;
        private readonly ServiceOptions _serviceOptions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:MicroNetCore.AspNetCore.Proxy.Proxy" /> class.
        /// </summary>
        /// <param name="proxyOptions">
        ///     <see cref="T:MicroNetCore.AspNetCore.Proxy.ProxyOptions" /> for resolving target
        ///     destinations.
        /// </param>
        /// <param name="serviceOptions">
        ///     <see cref="T:MicroNetCore.AspNetCore.Proxy.ServiceOptions" /> for resolving target
        ///     destinations.
        /// </param>
        public Proxy(ProxyOptions proxyOptions, ServiceOptions serviceOptions)
        {
            _proxyOptions = proxyOptions;
            _serviceOptions = serviceOptions;
        }

        /// <summary>
        ///     Maps request URI to target URI.
        /// </summary>
        /// <param name="requestPath">Request path.</param>
        /// <returns>Target URI string.</returns>
        public string Map(string requestPath)
        {
            // Remove first '/'
            requestPath = requestPath.Remove(0, 1);

            var path = requestPath.Split('?').First();

            foreach (var key in _proxyOptions.Keys)
            {
                if (!Match(path, key)) continue;

                var proxyUrl = _proxyOptions[key];
                var serviceUrl = ReplaceServiceNameByUrl(proxyUrl, requestPath);
                var replaced = requestPath.Replace(key, serviceUrl);

                return replaced.ToLower();
            }

            throw new KeyNotFoundException();
        }

        private string ReplaceServiceNameByUrl(string proxyUrl, string requestPath)
        {
            var localPath = requestPath.Split("/").LastOrDefault();

            return GetServicePath(proxyUrl) + "/" + localPath;
        }

        private string GetServicePath(string proxyUrl)
        {
            var start = proxyUrl.IndexOf('[');
            var end = proxyUrl.IndexOf(']');
            var serviceName = proxyUrl.Substring(start + 1, end - start - 1);

            return _serviceOptions[serviceName];
        }

        private static bool Match(string uri, string pattern)
        {
            if (string.Equals(uri, pattern, StringComparison.OrdinalIgnoreCase))
                return true;

            if (uri.StartsWith(pattern, StringComparison.CurrentCultureIgnoreCase) && uri[pattern.Length] == '/')
                return true;

            return false;
        }
    }
}