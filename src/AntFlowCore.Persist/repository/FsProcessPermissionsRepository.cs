using AntFlowCore.Abstraction.Orm.ext;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using FreeSql;
using FreeSql.Internal.Model;

namespace AntFlowCore.Persist.repository
{
    /// <summary>
    /// 流程权限仓储实现(FreeSql). 对应 Java FsProcessPermissionsRepository.
    /// </summary>
    public class FsProcessPermissionsRepository : RepositoryBase<BpmProcessPermissions>, IProcessPermissionsRepository
    {
        public FsProcessPermissionsRepository(AntFlowOrmContext context) : base(context)
        {
        }

        public List<BpmProcessPermissions> QueryPageList(ProcessPermissionsPageReq req,
            List<string> userIds, List<string> depIds, List<string> roleIds,
            string tenantId, Page<BpmProcessPermissions> page)
        {
            BasePagingInfo pagingInfo = page.ToPagingInfo().ToBasePagingInfo();
            var query = _ormContext.FreeSql.Select<BpmProcessPermissions>()
                .Where(a => a.IsDel == 0);
            if (!string.IsNullOrEmpty(tenantId))
            {
                query = query.Where(a => a.TenantId == tenantId);
            }
            if (!string.IsNullOrEmpty(req.FormCode))
            {
                query = query.Where(a => a.ProcessKey.Contains(req.FormCode));
            }
            if (req.PermissionsType != null)
            {
                query = query.Where(a => a.PermissionsType == req.PermissionsType);
            }
            if (req.ObjectType == 4)
            {
                //全员: 只按 object_type=4 过滤, 无对象 id(忽略 objectId/objectName)
                query = query.Where(a => a.ObjectType == 4);
            }
            else if (!string.IsNullOrEmpty(req.ObjectId))
            {
                //下拉搜索选中: 精确过滤 object_type + object_id
                if (req.ObjectType != null)
                {
                    query = query.Where(a => a.ObjectType == req.ObjectType);
                }
                query = query.Where(a => a.ObjectId == req.ObjectId);
            }
            else if (!string.IsNullOrEmpty(req.ObjectName))
            {
                //关键字搜索: 三段式(人员/部门/角色任一命中)
                query = query.Where(a =>
                    (userIds.Count > 0 && a.ObjectType == 1 && userIds.Contains(a.ObjectId)) ||
                    (depIds.Count > 0 && a.ObjectType == 2 && depIds.Contains(a.ObjectId)) ||
                    (roleIds.Count > 0 && a.ObjectType == 3 && roleIds.Contains(a.ObjectId)));
            }
            List<BpmProcessPermissions> list = query.OrderByDescending(a => a.CreateTime)
                .Page(pagingInfo)
                .ToList();
            page.Total = (int)pagingInfo.Count;
            return list;
        }
    }
}