using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

/// <summary>
/// 快捷入口仓储接口. 对应 Java QuickEntryMapper.
/// </summary>
public interface IQuickEntryRepository : IBaseRepository<QuickEntry>
{
}