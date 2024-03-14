namespace YourBrand.Catalog;

using Microsoft.Extensions.Logging;

public static partial class Log
{
    [LoggerMessage(
        EventId = 0,
        Message = "Could not open socket to `{HostName}`")]
    public static partial void CouldNotOpenSocket(
        ILogger logger,
        LogLevel level, /* Dynamic log level as parameter, rather than defined in attribute. */
        string hostName);
}