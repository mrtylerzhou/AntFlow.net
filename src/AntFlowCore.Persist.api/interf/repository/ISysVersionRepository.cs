using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

/// <summary>
/// 系统版本仓储接口. 对应 Java SysVersionMapper.
/// </summary>
public interface ISysVersionRepository : IBaseRepository<SysVersion>
{
}