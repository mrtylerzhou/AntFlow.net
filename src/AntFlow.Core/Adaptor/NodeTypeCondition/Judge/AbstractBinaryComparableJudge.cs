using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Vo;
using System.Reflection;
using System.Text.Json;

namespace AntFlow.Core.Adaptor.NodeTypeCondition.Judge;

/// <summary>
///     Ϊ֧�� 1 < a < 2 �������͵ıȽ��� Ƶģ��������ͨ� ĵ�ֵ� Ƚϣ��� ʹ�� AbstractComparableJudge��� ڶ�������ֵ Ϊ null ����
/// </summary>
public abstract class AbstractBinaryComparableJudge : AbstractComparableJudge
{
    private static readonly ILogger<AbstractBinaryComparableJudge> _log =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AbstractBinaryComparableJudge>();

    public override bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf,
        BpmnStartConditionsVo bpmnStartConditionsVo, int group)
    {
        string fieldNameInDb = FieldNameInDb();
        string fieldNameActual = FieldNameInStartConditions();

        string fieldValueInDb = null;
        string fieldValueActual = null;
        decimal fieldValueActualDecimal = 0;
        string fieldValue1InDb = null;
        string fieldValue2InDb = null;
        decimal? fieldValue1InDbDecimal = null;
        decimal? fieldValue2InDbDecimal = null;

        int? theOperatorType = conditionsConf.NumberOperator;

        try
        {
            PropertyInfo? dbField = conditionsConf.GetType().GetProperty(fieldNameInDb,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            PropertyInfo? actualField = bpmnStartConditionsVo.GetType().GetProperty(fieldNameActual,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (dbField == null || actualField == null)
            {
                throw new MissingFieldException($"�ֶ�δ�ҵ�: {fieldNameInDb} �� {fieldNameActual}");
            }

            fieldValueInDb = dbField.GetValue(conditionsConf)?.ToString();
            fieldValueActual = actualField.GetValue(bpmnStartConditionsVo)?.ToString() ?? "0";
            fieldValueActualDecimal = decimal.Parse(fieldValueActual);

            if (JudgeOperatorEnum.BinaryOperator().Contains(theOperatorType.Value))
            {
                string[]? split = fieldValueInDb.Split(',');
                fieldValue1InDb = split[0];
                fieldValue2InDb = split[1];
                fieldValue1InDbDecimal = decimal.Parse(fieldValue1InDb);
                fieldValue2InDbDecimal = decimal.Parse(fieldValue2InDb);
            }
            else
            {
                fieldValue1InDb = fieldValueInDb;
                fieldValue1InDbDecimal = decimal.Parse(fieldValue1InDb);
            }
        }
        catch (System.Exception e)
        {
            _log.LogError(e, "��ȡ���������ֶ���ʧ��! ����ֵ: {0}, ʵ������ֵ: {1}", JsonSerializer.Serialize(conditionsConf),
                JsonSerializer.Serialize(bpmnStartConditionsVo));
            throw;
        }

        return CompareJudge(fieldValue1InDbDecimal, fieldValue2InDbDecimal, fieldValueActualDecimal, theOperatorType);
    }

    /// <summary>
    ///     ���ݿ������õ���������
    /// </summary>
    protected abstract string FieldNameInDb();

    /// <summary>
    ///     ʵ�������ֶ�����
    /// </summary>
    protected abstract string FieldNameInStartConditions();
}