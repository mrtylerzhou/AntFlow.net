using System.Text;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service
{
    /// <summary>
    /// 部门服务. 对应 Java DepartmentServiceImpl(path 层级懒加载 + 搜索补祖先链).
    /// 实现内部接口 IDepartmentService(完整 Department) 与抽象层 IAfDepartmentService
    /// (BaseIdTranStruVo id+name, 对等 Java AfDepartmentService; 同名方法用显式接口实现区分返回类型).
    /// </summary>
    public class DepartmentService : IDepartmentService, IAfDepartmentService
    {
        private readonly AntFlowOrmContext _ormContext;

        public DepartmentService(IDepartmentRepository repository, AntFlowOrmContext ormContext)
        {
            _repository = repository;
            _ormContext = ormContext;
        }

        public IDepartmentRepository _repository { get; }

        /// <summary>
        /// path 深度(段数): /1/2 -> 2
        /// </summary>
        private static int DepthOf(string path)
        {
            return string.IsNullOrEmpty(path) ? 0 : path.Count(c => c == '/');
        }

        public List<Department> QueryByNameFuzzy(string name)
        {
            List<Department> matched = _repository.Find(d => d.Name.Contains(name));
            if (matched.Count == 0)
            {
                return matched;
            }
            //补祖先链: 收集匹配记录 path 的全部前缀(如 /1/2/3 -> /1, /1/2), 一次查出祖先并合并返回
            List<string> ancestorPaths = new();
            foreach (Department d in matched)
            {
                string path = d.Path;
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }
                string[] segs = path.Split('/');
                StringBuilder sb = new();
                for (int i = 1; i < segs.Length - 1; i++)
                {
                    sb.Append('/').Append(segs[i]);
                    ancestorPaths.Add(sb.ToString());
                }
            }
            if (ancestorPaths.Count == 0)
            {
                return matched;
            }
            List<Department> ancestors = _repository.Find(d => ancestorPaths.Contains(d.Path));
            Dictionary<int, Department> merged = new();
            foreach (Department a in ancestors)
            {
                merged.TryAdd(a.Id, a);
            }
            foreach (Department m in matched)
            {
                merged.TryAdd(m.Id, m);
            }
            return merged.Values.ToList();
        }

        public List<Department> GetDepartmentsByParentId(int? parentId)
        {
            if (parentId == null)
            {
                //初始两级: path 深度 1~2(根+根的直接子级), 前端一次请求渲染两级
                List<Department> twoLevels = _repository.GetTopTwoLevels();
                FillIsLeaf(twoLevels);
                return twoLevels;
            }
            //子节点: 先查父部门 path, 再按 path 前缀+深度查询直接子级
            Department parent = _repository.FirstOrDefault(d => d.Id == parentId.Value);
            if (parent == null || string.IsNullOrEmpty(parent.Path))
            {
                return new List<Department>();
            }
            List<Department> children = _repository.GetChildrenByParentPath(parent.Path, DepthOf(parent.Path));
            FillIsLeaf(children);
            return children;
        }

        /// <summary>
        /// 内存计算 isLeaf: 是否存在 path 深度+1 的直接子级
        /// </summary>
        private void FillIsLeaf(List<Department> list)
        {
            if (list.Count == 0)
            {
                return;
            }
            int maxDepth = list.Max(d => DepthOf(d.Path));
            //查下一层(深度=maxDepth+1)的 path 集合, 判断每个节点是否有直接子级
            List<string?> nextLevelPaths = _repository.GetPathsByDepth(maxDepth + 1);
            foreach (Department d in list)
            {
                string path = d.Path;
                if (string.IsNullOrEmpty(path))
                {
                    d.IsLeaf = true;
                    continue;
                }
                d.IsLeaf = !nextLevelPaths.Any(p => p != null && p.StartsWith(path + "/"));
            }
        }

        // ==================== IAfDepartmentService 抽象层(id+name) ====================

        public BaseIdTranStruVo GetDepartmentById(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !int.TryParse(id, out int idInt))
            {
                return null;
            }
            return ToVo(_repository.FirstOrDefault(d => d.Id == idInt));
        }

        public List<BaseIdTranStruVo> ListSubDepartmentByEmployeeId(string employeeId)
        {
            // demo 占位实现(对等 Java DepartmentMapper.ListSubDepartmentByEmployeeId: id = employeeId)
            if (string.IsNullOrWhiteSpace(employeeId) || !int.TryParse(employeeId, out int eid))
            {
                return new List<BaseIdTranStruVo>();
            }
            return ToVos(_repository.Find(d => d.Id == eid));
        }

        public List<BaseIdTranStruVo> GetByIds(List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return new List<BaseIdTranStruVo>();
            }
            List<int> intIds = ids.Where(s => !string.IsNullOrWhiteSpace(s) && int.TryParse(s, out _))
                .Select(int.Parse).ToList();
            if (intIds.Count == 0)
            {
                return new List<BaseIdTranStruVo>();
            }
            return ToVos(_repository.Find(d => intIds.Contains(d.Id)));
        }

        // 显式接口实现: 与 IDepartmentService.QueryByNameFuzzy(string)->List<Department> 区分
        List<BaseIdTranStruVo> IAfDepartmentService.QueryByNameFuzzy(string name)
        {
            return ToVos(QueryByNameFuzzy(name));
        }

        // 显式接口实现: 与 IDepartmentService.GetDepartmentsByParentId(int?)->List<Department> 区分
        List<BaseIdTranStruVo> IAfDepartmentService.GetDepartmentsByParentId(string parentId)
        {
            int? pid = string.IsNullOrWhiteSpace(parentId) || !int.TryParse(parentId, out int p) ? null : p;
            return ToVos(GetDepartmentsByParentId(pid));
        }

        public List<BaseIdTranStruVo> GetDepartmentByCompanyId(string companyId)
        {
            if (string.IsNullOrWhiteSpace(companyId))
            {
                return new List<BaseIdTranStruVo>();
            }
            // t_department.company_id 列未映射实体, 使用原生 SQL(对等 Java DepartmentMapper.getDepartmentByCompanyId)
            List<Department> list = _ormContext.FreeSql.Ado.Query<Department>(
                "select id, name from t_department where company_id = @companyId and is_del = 0",
                new { companyId });
            return ToVos(list);
        }

        public ResultAndPage<BaseIdTranStruVo> GetDepartmentPageList(int page, int pageSize, string name)
        {
            if (page <= 0)
            {
                page = 1;
            }
            if (pageSize <= 0)
            {
                pageSize = 10;
            }
            List<Department> all = _repository.Find(d =>
                (string.IsNullOrWhiteSpace(name) || (d.Name != null && d.Name.Contains(name))) && d.IsDel != true);
            var pageData = new Page<Department>(page, pageSize)
            {
                Records = all.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                Total = all.Count
            };
            return PageUtils.GetResultAndPage(pageData, ToVo);
        }

        // ==================== 转换 ====================

        private BaseIdTranStruVo ToVo(Department d)
        {
            return d == null ? null : new BaseIdTranStruVo(d.Id.ToString(), d.Name);
        }

        private List<BaseIdTranStruVo> ToVos(List<Department> deps)
        {
            if (deps == null || deps.Count == 0)
            {
                return new List<BaseIdTranStruVo>();
            }
            return deps.Select(ToVo).Where(v => v != null).ToList();
        }
    }
}