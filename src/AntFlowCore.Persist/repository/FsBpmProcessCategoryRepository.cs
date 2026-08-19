using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository
{
    /// <summary>
    /// 流程分类仓储实现(FreeSql). 对应 Java BpmProcessCategoryMapper.
    /// </summary>
    public class FsBpmProcessCategoryRepository : RepositoryBase<BpmProcessCategory>, IBpmProcessCategoryRepository
    {
        public FsBpmProcessCategoryRepository(AntFlowOrmContext context) : base(context)
        {
        }
    }
}
