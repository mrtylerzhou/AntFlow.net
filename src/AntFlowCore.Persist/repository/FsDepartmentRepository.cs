using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository
{
    public class FsDepartmentRepository : RepositoryBase<Department>, IDepartmentRepository
    {
        public FsDepartmentRepository(AntFlowOrmContext context) : base(context)
        {
        }

        public List<Department> GetTopTwoLevels()
        {
            return _ormContext.FreeSql.Select<Department>()
                .Where("length(path) - length(replace(path, '/', '')) between 1 and 2")
                .OrderBy("sort asc, id asc")
                .ToList();
        }

        public List<Department> GetChildrenByParentPath(string parentPath, int parentDepth)
        {
            return _ormContext.FreeSql.Select<Department>()
                .Where("path like concat(?, '/%')", parentPath)
                .Where("length(path) - length(replace(path, '/', '')) = ?", parentDepth + 1)
                .OrderBy("sort asc, id asc")
                .ToList();
        }

        public List<string?> GetPathsByDepth(int depth)
        {
            return _ormContext.FreeSql.Select<Department>()
                .Where("path is not null")
                .Where("length(path) - length(replace(path, '/', '')) = ?", depth)
                .ToList(a => a.Path);
        }
    }
}