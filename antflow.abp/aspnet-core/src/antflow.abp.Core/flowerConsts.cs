

using antflow.abp.Debugging;

namespace antflow.abp;

public class flowerConsts
{
    public const string LocalizationSourceName = "flower";

    public const string ConnectionStringName = "Default";

    public const bool MultiTenancyEnabled = true;


    /// <summary>
    /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
    /// </summary>
    public static readonly string DefaultPassPhrase =
        DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "a6fcfe0fb59b4a9ba18dc66e80920c02";
}
