using Microsoft.Extensions.Logging;

namespace AntFlowCore.Base.util;

public class AfStaticLogUtil
{
    public static ILogger Logger { get; set; }

    public AfStaticLogUtil(ILogger<AfStaticLogUtil> logger)
    {
        Logger = logger;
    }
}