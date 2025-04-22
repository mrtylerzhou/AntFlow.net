using antflowcore.adaptor.formoperation;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;
using System.Reflection;
using System.Text.Json;

namespace antflowcore.factory;

public class FormFactory
{
    private readonly IAdaptorFactory _adaptorFactory;
    private readonly OutSideBpmAccessBusinessService _outSideBpmAccessBusinessService;
    private readonly IServiceProvider _serviceProvider;

    public FormFactory(
        IAdaptorFactory adaptorFactory,
        OutSideBpmAccessBusinessService outSideBpmAccessBusinessService,
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

    public BusinessDataVo DataFormConversion(string parameters, string formCode)
    {
        var vo = JsonSerializer.Deserialize<BusinessDataVo>(parameters);

        if (string.IsNullOrEmpty(formCode))
        {
            formCode = vo.FormCode;
        }

        if (vo.IsOutSideAccessProc != null && vo.IsOutSideAccessProc == true)
        {
            var bpmAccessBusinesses = _outSideBpmAccessBusinessService.baseRepo
                .Where(a => a.ProcessNumber == vo.ProcessNumber).ToList();

            if (bpmAccessBusinesses.Any())
            {
                vo.FormData = bpmAccessBusinesses.First().FormDataPc;
            }

            return vo;
        }

        if (vo.IsLowCodeFlow == 1)
        {
            formCode = StringConstants.LOWFLOW_FORM_CODE;
        }

        var formTClass = GetFormTClass(formCode);
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