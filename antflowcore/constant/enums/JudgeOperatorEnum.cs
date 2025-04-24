using antflowcore.exception;

namespace antflowcore.constant.enus;

public class JudgeOperatorEnum
{
    public static readonly JudgeOperatorEnum GTE = new JudgeOperatorEnum(1, ">=");
    public static readonly JudgeOperatorEnum GT = new JudgeOperatorEnum(2, ">");
    public static readonly JudgeOperatorEnum LTE = new JudgeOperatorEnum(3, "<=");
    public static readonly JudgeOperatorEnum LT = new JudgeOperatorEnum(4, "<");
    public static readonly JudgeOperatorEnum EQ = new JudgeOperatorEnum(5, "=");
    public static readonly JudgeOperatorEnum GT1LT2=new JudgeOperatorEnum(6,"first<a<second");
    public static readonly JudgeOperatorEnum GTE1LT2=new JudgeOperatorEnum(7,"first<=a<second");
    public static readonly JudgeOperatorEnum GET1LE2=new JudgeOperatorEnum(8,"first<a<=second");
    public static readonly JudgeOperatorEnum GTE1LTE2=new JudgeOperatorEnum(9,"first<=a<=second");
  
 
    
    public int Code { get; }
    public string Symbol { get; }

    private JudgeOperatorEnum(int code, string symbol)
    {
        Code = code;
        Symbol = symbol;
    }

    public static JudgeOperatorEnum GetBySymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            return null;
        }

        return AllValues().FirstOrDefault(value => value.Symbol.Equals(symbol.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public static JudgeOperatorEnum GetByOpType(int opType)
    {
        var value = AllValues().FirstOrDefault(v => v.Code == opType);
        if (value == null)
        {
            throw new AFBizException("操作符类型未定义");
        }
        return value;
    }

    private static JudgeOperatorEnum[] AllValues()
    {
        return new[] { GTE, GT, LTE, LT, EQ };
    }
    public static List<int> BinaryOperator()
    {
        return new List<int>() { 6, 7, 8, 9 };
    }
}