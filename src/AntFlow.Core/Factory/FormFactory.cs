using AntFlow.Core.Adaptor;
using AntFlow.Core.Constant;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Vo;
using System.Reflection;
using System.Text.Json;

namespace AntFlow.Core.Factory;

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
        IFormOperationAdaptor<BusinessDataVo>? activitiService = _adaptorFactory.GetActivitiService(vo);
        if (activitiService == null)
        {
            throw new AFBizException("Form code does not have a processing bean!");
        }

        return (IFormOperationAdaptor<BusinessDataVo>)activitiService;
    }

    public BusinessDataVo DataFormConversion(string parameters, string formCode)
    {
        BusinessDataVo vo = JsonSerializer.Deserialize<BusinessDataVo>(parameters);

        if (string.IsNullOrEmpty(formCode))
        {
            formCode = vo.FormCode;
        }

        if (vo.IsOutSideAccessProc != null && vo.IsOutSideAccessProc == true)
        {
            List<OutSideBpmAccessBusiness>? bpmAccessBusinesses = _outSideBpmAccessBusinessService.baseRepo
                .Where(a => a.ProcessNumber == vo.ProcessNumber).ToList();

            if (bpmAccessBusinesses.Any())
            {
                vo.FormData = bpmAccessBusinesses.First().FormDataPc;
            }
        }

        if (vo.IsLowCodeFlow == 1)
        {
            formCode = StringConstants.LOWFLOW_FORM_CODE;
        }


        Type? formTClass = GetFormTClass(formCode);
        return (BusinessDataVo)JsonSerializer.Deserialize(parameters, formTClass);
    }

    private Type GetFormTClass(string formCode)
    {
        IFormOperationAdaptor<BusinessDataVo>? service = GetFormAdaptor(new BusinessDataVo { FormCode = formCode });
        if (service != null)
        {
            //获取泛型参数，通过wrapper获取内部的_inner字段类型，从而得到真正的业务数据类型
            Type genericArgument = service.GetType().GetRuntimeFields().First().FieldType.GetGenericArguments()[0];
            return genericArgument;
        }

        throw new AFBizException(
            "The form is not associated with a business implementation class or its generic type!");
    }
}