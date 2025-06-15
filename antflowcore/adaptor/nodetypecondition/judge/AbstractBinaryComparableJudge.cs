using System.Reflection;
using System.Text.Json;
using antflowcore.constant.enus;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.nodetypecondition.judge;

 /// <summary>
    /// 为支持 1 < a < 2 这种类型的比较设计的，如果是普通的单值比较，请使用 AbstractComparableJudge，第二个参数值为 null 即可
    /// </summary>
    public abstract class AbstractBinaryComparableJudge : AbstractComparableJudge
    {
        private static readonly ILogger<AbstractBinaryComparableJudge> _log = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AbstractBinaryComparableJudge>();

        public override bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo, int index,int group)
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
                var dbField = conditionsConf.GetType().GetProperty(fieldNameInDb, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var actualField = bpmnStartConditionsVo.GetType().GetProperty(fieldNameActual, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (dbField == null || actualField == null)
                {
                    throw new MissingFieldException($"字段未找到: {fieldNameInDb} 或 {fieldNameActual}");
                }

                fieldValueInDb = dbField.GetValue(conditionsConf)?.ToString();
                fieldValueActual = actualField.GetValue(bpmnStartConditionsVo)?.ToString()??"0";
                fieldValueActualDecimal = decimal.Parse(fieldValueActual);

                if (JudgeOperatorEnum.BinaryOperator().Contains(theOperatorType.Value))
                {
                    var split = fieldValueInDb.Split(',');
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
            catch (Exception e)
            {
                _log.LogError(e, "获取配置条件字段名失败! 配置值: {0}, 实际条件值: {1}", JsonSerializer.Serialize(conditionsConf), JsonSerializer.Serialize(bpmnStartConditionsVo));
                throw;
            }

            return CompareJudge(fieldValue1InDbDecimal, fieldValue2InDbDecimal, fieldValueActualDecimal, theOperatorType);
        }

        /// <summary>
        /// 数据库中配置的条件名称
        /// </summary>
        protected abstract string FieldNameInDb();

        /// <summary>
        /// 实际条件字段名称
        /// </summary>
        protected abstract string FieldNameInStartConditions();
    }