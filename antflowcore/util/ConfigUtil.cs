namespace antflowcore.util;

public class ConfigUtil
{
    public static bool IsFullSassMode()
    {
      return  ServiceProviderUtils.GetService<IConfiguration>().GetValue<bool?>("antflow.sass.full-sass-mode") ?? false;
    }
}