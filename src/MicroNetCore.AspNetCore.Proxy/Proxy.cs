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

                var proxyOption = _proxyOptions[key];
                var serviceUrl = CreateReplacingUrl(proxyOption, requestPath);
                var replaced = requestPath.Replace(key.ToLower(), serviceUrl.ToLower());

                return replaced;
            }

            throw new KeyNotFoundException();
        }

        private string GetServiceName(string proxyOption)
        {
            var start = proxyOption.IndexOf('[');
            var end = proxyOption.LastIndexOf(']');

            return proxyOption.Substring(start + 1, end - start - 1);
        }

        private string CreateReplacingUrl(string proxyOption, string requestPath)
        {
            var localPath = requestPath.Split("/").LastOrDefault();
            var serviceName = GetServiceName(proxyOption);
            return GetSerivcePath(serviceName) + "/" + localPath;
        }

        private string GetSerivcePath(string serviceName)
        {
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