using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository
{
    public interface IDepartmentService : IAntFlowRepositoryMix<Department, IDepartmentRepository>
    {
        /// <summary>
        /// 部门名称模糊查询(匹配+沿path补祖先链)
        /// </summary>
        List<Department> QueryByNameFuzzy(string name);

        /// <summary>
        /// 树懒加载: parentId为空返回两级(根+直接子级, 带isLeaf), 有值返回直接子级(带isLeaf)
        /// </summary>
        List<Department> GetDepartmentsByParentId(int? parentId);
    }
}