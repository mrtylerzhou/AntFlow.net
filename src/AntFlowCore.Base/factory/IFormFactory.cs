using AntFlowCore.Core.adaptor.formoperation;
using AntFlowCore.Vo;

namespace AntFlowCore.Core.factory;

public interface IFormFactory
{
    IFormOperationAdaptor<BusinessDataVo> GetFormAdaptor(string formCode);
    IFormOperationAdaptor<BusinessDataVo> GetFormAdaptor(BusinessDataVo vo);
    BusinessDataVo DataFormConversion(string parameters, string formCode);
}
