using System.Collections.Generic;

namespace MicroNetCore.AspNetCore.Proxy
{
    /// <summary>
    ///     Represents a collection of keys and values, where a key is service name,
    ///     and a value is its corresponding URL
    /// </summary>
    public sealed class ServiceOptions : Dictionary<string, string>
    {
    }
}