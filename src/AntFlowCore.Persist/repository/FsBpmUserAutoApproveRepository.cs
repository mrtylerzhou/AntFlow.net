using AntFlowCore.Abstraction.Orm.ext;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using FreeSql;
using FreeSql.Internal.Model;

namespace AntFlowCore.Persist.repository;

/// <summary>
/// 用户自动审批设置 仓储实现.
/// </summary>
public class FsBpmUserAutoApproveRepository : RepositoryBase<BpmUserAutoApprove>, IBpmUserAutoApproveRepository
{
    public FsBpmUserAutoApproveRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmUserAutoApprove> QueryPageList(string ownerUserName, string ownerUserId, string formCode, string tenantId, Page<BpmUserAutoApprove> page)
    {
        BasePagingInfo pagingInfo = page.ToPagingInfo().ToBasePagingInfo();
        var query = _ormContext.FreeSql.Select<BpmUserAutoApprove>()
            .Where(a => a.IsDel == 0);
        if (!string.IsNullOrEmpty(tenantId))
        {
            query = query.Where(a => a.TenantId == tenantId);
        }
        //下拉搜索选中归属人: 精确匹配 id(优先于姓名模糊)
        if (!string.IsNullOrEmpty(ownerUserId))
        {
            query = query.Where(a => a.OwnerUserId == ownerUserId);
        }
        else if (!string.IsNullOrEmpty(ownerUserName))
        {
            query = query.Where(a => a.OwnerUserName.Contains(ownerUserName));
        }
        if (!string.IsNullOrEmpty(formCode))
        {
            query = query.Where(a => a.FormCode.Contains(formCode));
        }
        List<BpmUserAutoApprove> list = query.OrderByDescending(a => a.Id)
            .Page(pagingInfo)
            .ToList();
        page.Total = (int)pagingInfo.Count;
        return list;
    }
}
