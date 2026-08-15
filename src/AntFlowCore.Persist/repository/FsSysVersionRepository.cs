using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository
{
    /// <summary>
    /// 系统版本仓储实现(FreeSql). 对应 Java SysVersionServiceImpl(存储部分).
    /// </summary>
    public class FsSysVersionRepository : RepositoryBase<SysVersion>, ISysVersionRepository
    {
        public FsSysVersionRepository(AntFlowOrmContext context) : base(context)
        {
        }
    }
}