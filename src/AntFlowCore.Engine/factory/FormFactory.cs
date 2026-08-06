using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using AntFlowCore.Base.adaptor.formoperation;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.adaptor;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Engine.factory;


public class FormFactory : IFormFactory
    {

        private readonly IAdaptorFactory _adaptorFactory;
        private readonly IOutSideBpmAccessBusinessService _outSideBpmAccessBusinessService;
        private readonly IServiceProvider _serviceProvider;

        public FormFactory(
            IAdaptorFactory adaptorFactory,
            IOutSideBpmAccessBusinessService outSideBpmAccessBusinessService,
            IServiceProvider serviceProvider)
        {
            _adaptorFactory = adaptorFactory;
            _outSideBpmAccessBusinessService = outSideBpmAccessBusinessService;
            _serviceProvider = serviceProvider;
        }

        public IFormOperationAdaptor<BusinessDataVo> GetFormAdaptor(string formCode)
        {

            return GetFormAdaptor(new BusinessDataVo { FormCode = formCode });
        }

        public IFormOperationAdaptor<BusinessDataVo> GetFormAdaptor(BusinessDataVo vo)
        {

            if (vo == null)
            {
                return null;
            }
            //todo
            var activitiService = _adaptorFactory.GetActivitiService(vo);
            if (activitiService == null)
            {
                throw new AFBizException("Form code does not have a processing bean!");
            }

            return (IFormOperationAdaptor<BusinessDataVo>)activitiService;
        }

        /// <summary>
        /// UDLFApplyVo(含父类 BusinessDataVo)的 JSON 属性名集合,用于识别"扁平业务字段"。
        /// 顶层 JSON key 命中此集合视为引擎控制/结构字段(不折进 lfFields),其余视为自定义表单业务字段。
        /// 双性 special-case: remark 被显式排除——它既是 UDLFApplyVo 属性(被 OnSubmitData 用作 ProcessDigest),
        /// 又是"备注"表单字段的最自然命名。排除后 remark 既折进 lfFields(存表+往返),又因仍是 UDLFApplyVo 真实属性
        /// 被 target 反序列化映射到 vo.Remark(喂 digest),两全。
        /// </summary>
        private static readonly HashSet<string> ReservedPropsUdLf = BuildReservedProps();

        private static HashSet<string> BuildReservedProps()
        {
            var set = new HashSet<string>(StringComparer.Ordinal);
            foreach (var prop in typeof(UDLFApplyVo).GetProperties())
            {
                var attr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
                if (attr != null && !string.IsNullOrEmpty(attr.Name))
                {
                    set.Add(attr.Name);
                }
                else
                {
                    set.Add(prop.Name);
                }
            }
            set.Remove("remark"); // 双性: remark 既要喂 ProcessDigest 又要存表
            return set;
        }

        /// <summary>
        /// page-added DIY 辅助: 把顶层"扁平业务字段"(非 UDLFApplyVo 保留属性)折进 lfFields,
        /// 使自定义 Vue 表单(直接发扁平顶层字段)的数据能被 LowFlowApprovalService 按 lfFields 存储。
        /// 仅 IsLowCodeFlow==1 时调用。纯 LF 无扁平字段 → extra 为空 → 原样返回(no-op)。扁平覆盖嵌套。
        /// </summary>
        /// <summary>返回 JsonObject(不再序列化回 String),no-op 返回 null 表示无需折叠</summary>
        private JsonObject? FoldFlatFieldsIntoLfFields(string parameters)
        {
            JsonObject jsonObj = JsonNode.Parse(parameters)?.AsObject()
                ?? throw new AFBizException("invalid form json");
            var extra = new Dictionary<string, JsonNode>();
            foreach (var kvp in jsonObj)
            {
                if (!ReservedPropsUdLf.Contains(kvp.Key))
                {
                    extra[kvp.Key] = kvp.Value;
                }
            }
            if (extra.Count == 0)
            {
                return null; // 纯 LF: 无扁平业务字段
            }
            var merged = new JsonObject();
            if (jsonObj.TryGetPropertyValue("lfFields", out var existingLf) && existingLf is JsonObject existingObj)
            {
                foreach (var e in existingObj)
                {
                    merged[e.Key] = e.Value?.DeepClone();
                }
            }
            foreach (var e in extra)
            {
                merged[e.Key] = e.Value?.DeepClone(); // 扁平覆盖嵌套
            }
            jsonObj["lfFields"] = merged;
            return jsonObj;
        }

        public BusinessDataVo DataFormConversion(string parameters, string formCode)
        {
            BusinessDataVo vo = JsonSerializer.Deserialize<BusinessDataVo>(parameters);

            if (string.IsNullOrEmpty(formCode))
            {
                formCode = vo.FormCode;
            }

            if (vo.IsOutSideAccessProc!=null&&vo.IsOutSideAccessProc==true)
            {
                var bpmAccessBusinesses = _outSideBpmAccessBusinessService._repository
                    .Find(a => a.ProcessNumber == vo.ProcessNumber);

                if (bpmAccessBusinesses.Any())
                {
                    vo.FormData = bpmAccessBusinesses.First().FormDataPc;
                }

            }

            JsonObject? foldedObj = null;
            if (vo.IsLowCodeFlow == 1)
            {
                formCode = StringConstants.LOWFLOW_FORM_CODE;
                // 方案C: 纯 LF 前端必有嵌套 lfFields → 跳过 JsonObject 解析(零额外成本);
                //        page-added DIY 前端发扁平顶层(无 lfFields) → 才折叠(方案A: 返回 JsonObject,由 Deserialize 直转目标,不再序列化回 String)。
                if (vo.LfFields == null)
                {
                    foldedObj = FoldFlatFieldsIntoLfFields(parameters);
                }
            }


            var formTClass = GetFormTClass(formCode);
            if (foldedObj != null)
            {
                return (BusinessDataVo)foldedObj.Deserialize(formTClass);
            }
            return (BusinessDataVo)JsonSerializer.Deserialize(parameters, formTClass);
        }

        private Type GetFormTClass(string formCode)
        {
            var service = GetFormAdaptor(new BusinessDataVo { FormCode = formCode });
            if (service != null)
            {
                //跟设计有关,获取wrapper里面的_inner的泛型类型,如果后期更换了设计记得更改
                Type genericArgument = service.GetType().GetRuntimeFields().First().FieldType.GetGenericArguments()[0];
                return genericArgument;
            }

            throw new AFBizException("The form is not associated with a business implementation class or its generic type!");
        }
    }
