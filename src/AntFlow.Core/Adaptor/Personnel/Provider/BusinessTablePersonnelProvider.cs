using AntFlow.Core.Adaptor.Personnel.BusinessTableAdp;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Exception;
using AntFlow.Core.Factory;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel.Provider;

public class BusinessTablePersonnelProvider : IBpmnPersonnelProviderService
{
    private readonly IAdaptorFactory _adaptorFactory;

    public BusinessTablePersonnelProvider(IAdaptorFactory adaptorFactory)
    {
        _adaptorFactory = adaptorFactory;
    }

    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo,
        BpmnStartConditionsVo startConditionsVo)
    {
        BpmnNodePropertysVo property = bpmnNodeVo.Property;
        if (property == null)
        {
            throw new AFBizException("property can not be null");
        }

        int? configurationTableType = property.ConfigurationTableType;
        int? tableFieldType = property.TableFieldType;
        if (configurationTableType == null)
        {
            throw new AFBizException("configuration table type can not be null");
        }

        if (tableFieldType == null)
        {
            throw new AFBizException("table field type can not be null!");
        }

        BusinessConfTableFieldEnum? tableFieldEnumByCode =
            BusinessConfTableFieldEnumExtensions.GetTableFieldEnumByCode(tableFieldType.Value);
        if (tableFieldEnumByCode == null)
        {
            throw new AFBizException("can not find BusinessConfTableFieldEnum by given fieldType");
        }

        ConfigurationTableAdapterEnum? byTableFieldEnum =
            ConfigurationTableAdapterEnumExtensions.GetByTableFieldEnum(tableFieldEnumByCode.Value);
        if (byTableFieldEnum == null)
        {
            throw new AFBizException("can not find ConfigurationTableAdapterEnum by given fieldType");
        }

        AbstractBusinessConfigurationAdaptor businessConfigurationAdaptor =
            _adaptorFactory.GetBusinessConfigurationAdaptor(byTableFieldEnum.Value);
        List<BpmnNodeParamsAssigneeVo> bpmnNodeParamsAssigneeVos =
            businessConfigurationAdaptor.doFindBusinessPerson(bpmnNodeVo, startConditionsVo);
        return bpmnNodeParamsAssigneeVos;
    }
}