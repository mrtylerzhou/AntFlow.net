using AntFlowCore.Abstraction;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Base.adaptor.personnel.businesstableadp;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Bpmn.adaptor.personnel.provider;

public class BusinessTablePersonnelProvider: IBpmnPersonnelProviderService
{
    private readonly IAdaptorFactory _adaptorFactory;

    public BusinessTablePersonnelProvider(IAdaptorFactory adaptorFactory)
    {
        _adaptorFactory = adaptorFactory;
    }
    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        BpmnNodePropertysVo property = bpmnNodeVo.Property;
        if(property==null){
            throw new AFBizException("property can not be null");
        }
        int? configurationTableType = property.ConfigurationTableType;
        int? tableFieldType = property.TableFieldType;
        if (configurationTableType == null)
        {
            throw new AFBizException("configuration table type can not be null");
        }
        if(tableFieldType==null){
            throw new AFBizException("table field type can not be null!");
        }
        BusinessConfTableFieldEnum? tableFieldEnumByCode = BusinessConfTableFieldEnumExtensions.GetTableFieldEnumByCode(tableFieldType.Value);
        if(tableFieldEnumByCode==null){
            throw new AFBizException("can not find BusinessConfTableFieldEnum by given fieldType");
        }
        ConfigurationTableAdapterEnum? byTableFieldEnum = ConfigurationTableAdapterEnumExtensions.GetByTableFieldEnum(tableFieldEnumByCode.Value);
        if(byTableFieldEnum==null){
            throw new AFBizException("can not find ConfigurationTableAdapterEnum by given fieldType");
        }
        AbstractBusinessConfigurationAdaptor businessConfigurationAdaptor = _adaptorFactory.GetBusinessConfigurationAdaptor(byTableFieldEnum.Value);
        List<BpmnNodeParamsAssigneeVo> bpmnNodeParamsAssigneeVos = businessConfigurationAdaptor.doFindBusinessPerson(bpmnNodeVo, startConditionsVo);
        return bpmnNodeParamsAssigneeVos;
    }
}