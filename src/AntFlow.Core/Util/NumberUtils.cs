namespace AntFlow.Core.Util;

public static class NumberUtils
{
    public static decimal ToScaledDecimal(string value, int scale, MidpointRounding rounding)
    {
        if (decimal.TryParse(value, out decimal decimalValue))
        {
            return Math.Round(decimalValue, scale, rounding);
        }

        throw new FormatException($"The value '{value}' cannot be converted to a decimal.");
    }
}