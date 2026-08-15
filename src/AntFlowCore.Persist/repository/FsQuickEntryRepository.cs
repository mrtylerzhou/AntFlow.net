using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository
{
    /// <summary>
    /// 快捷入口仓储实现(FreeSql). 对应 Java QuickEntryServiceImpl(存储部分).
    /// </summary>
    public class FsQuickEntryRepository : RepositoryBase<QuickEntry>, IQuickEntryRepository
    {
        public FsQuickEntryRepository(AntFlowOrmContext context) : base(context)
        {
        }
    }
}