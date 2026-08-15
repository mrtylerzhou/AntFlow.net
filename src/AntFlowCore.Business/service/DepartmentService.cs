using System.Text;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service
{
    /// <summary>
    /// 部门服务. 对应 Java DepartmentServiceImpl(path 层级懒加载 + 搜索补祖先链).
    /// </summary>
    public class DepartmentService : IDepartmentService
    {
        public DepartmentService(IDepartmentRepository repository)
        {
            _repository = repository;
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
    }
}