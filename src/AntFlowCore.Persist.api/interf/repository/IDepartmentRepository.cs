using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        /// <summary>
        /// 树懒加载-初始两级: path 深度 1~2 的部门(根+根的直接子级)
        /// </summary>
        List<Department> GetTopTwoLevels();

        /// <summary>
        /// 树懒加载-子节点: path 以父path开头且深度=父深度+1
        /// </summary>
        List<Department> GetChildrenByParentPath(string parentPath, int parentDepth);

        /// <summary>
        /// 指定深度层的全部 path(供 isLeaf 判断)
        /// </summary>
        List<string?> GetPathsByDepth(int depth);
    }
}