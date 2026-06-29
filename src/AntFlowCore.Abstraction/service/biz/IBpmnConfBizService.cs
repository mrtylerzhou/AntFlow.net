using AntFlowCore.Base.dto;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IBpmnConfBizService
{
    void Edit(BpmnConfVo bpmnConfVo);
    ResultAndPage<BpmnConfVo> SelectPage(PageDto pageDto, BpmnConfVo vo);
    BpmnConfVo Detail(long id);
    BpmnConfVo Detail(string bpmnCode);
    BpmnConfVo DetailByFormCode(string formCode);
    int? GetCustomizeNodeSignType(long nodeId);
    
    /// <summary>
    /// Save process notice configuration (notice channel types and advanced
    /// notification templates) by partially updating conf_config_json.
    /// </summary>
    /// <param name="vo">process configuration vo carrying processKey, notifyTypeIds and templateVos</param>
    void SaveProcessNotices(ProcessConfVo vo);
}
