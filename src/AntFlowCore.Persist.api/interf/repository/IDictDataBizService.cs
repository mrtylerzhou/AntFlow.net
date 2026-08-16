using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository
{
    /// <summary>
    /// 字典管理 业务服务接口. 对应 Java DictDataBizServiceImpl.
    /// </summary>
    public interface IDictDataBizService
    {
        /// <summary>
        /// 分页列表(过滤 is_del=0, dict_type 映射汉字含义)
        /// </summary>
        ResultAndPage<DictDataVo> ListPage(DictDataPageReq req);

        /// <summary>
        /// 新增(唯一性校验, lowcodeflow 禁止手动新增)
        /// </summary>
        long Save(DictDataSaveVo vo);

        /// <summary>
        /// 编辑(lowcodeflow 禁止编辑, 唯一性校验排除自身)
        /// </summary>
        void Update(DictDataSaveVo vo);

        /// <summary>
        /// 删除(逻辑删除 is_del=1, lowcodeflow 禁止删除)
        /// </summary>
        void Delete(long id);
    }
}