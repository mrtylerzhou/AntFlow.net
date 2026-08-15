using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository
{
    /// <summary>
    /// 版本关联数据仓储实现(FreeSql). 对应 Java BpmProcessAppDataServiceImpl(存储部分).
    /// </summary>
    public class FsBpmProcessAppDataRepository : RepositoryBase<BpmProcessAppData>, IBpmProcessAppDataRepository
    {
        public FsBpmProcessAppDataRepository(AntFlowOrmContext context) : base(context)
        {
        }
    }
}