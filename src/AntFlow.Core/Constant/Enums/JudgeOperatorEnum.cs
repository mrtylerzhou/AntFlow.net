using AntFlow.Core.Exception;

namespace AntFlow.Core.Constant.Enums;

public class JudgeOperatorEnum
{
    public static readonly JudgeOperatorEnum GTE = new(1, ">=");
    public static readonly JudgeOperatorEnum GT = new(2, ">");
    public static readonly JudgeOperatorEnum LTE = new(3, "<=");
    public static readonly JudgeOperatorEnum LT = new(4, "<");
    public static readonly JudgeOperatorEnum EQ = new(5, "=");
    public static readonly JudgeOperatorEnum GT1LT2 = new(6, "first<a<second");
    public static readonly JudgeOperatorEnum GTE1LT2 = new(7, "first<=a<second");
    public static readonly JudgeOperatorEnum GET1LE2 = new(8, "first<a<=second");
    public static readonly JudgeOperatorEnum GTE1LTE2 = new(9, "first<=a<=second");

    private JudgeOperatorEnum(int code, string symbol)
    {
        Code = code;
        Symbol = symbol;
    }


    public int Code { get; }
    public string Symbol { get; }

    public static JudgeOperatorEnum GetBySymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            return null;
        }

        return AllValues()
            .FirstOrDefault(value => value.Symbol.Equals(symbol.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public static JudgeOperatorEnum GetByOpType(int opType)
    {
        JudgeOperatorEnum? value = AllValues().FirstOrDefault(v => v.Code == opType);
        if (value == null)
        {
            throw new AFBizException("??????????¦Ä????");
        }

        return value;
    }

    private static JudgeOperatorEnum[] AllValues()
    {
        return new[] { GTE, GT, LTE, LT, EQ };
    }

    public static List<int> BinaryOperator()
    {
        return new List<int> { 6, 7, 8, 9 };
    }
}