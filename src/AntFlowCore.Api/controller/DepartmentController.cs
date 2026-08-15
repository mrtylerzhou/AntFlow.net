using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller
{
    /// <summary>
    /// 部门接口(树懒加载/名称搜索). 对应 Java DepartmentController.
    /// </summary>
    [Route("department")]
    public class DepartmentController
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        /// <summary>
        /// 树懒加载: parentId为空返回两级(根+直接子级, 带isLeaf), 有值返回直接子级
        /// </summary>
        [HttpGet("getDepartmentsByParentId")]
        public Result<List<Department>> GetDepartmentsByParentId([FromQuery] int? parentId)
        {
            return ResultHelper.Success(_departmentService.GetDepartmentsByParentId(parentId));
        }

        /// <summary>
        /// 部门名称模糊查询(匹配+祖先链)
        /// </summary>
        [HttpGet("queryByNameFuzzy")]
        public Result<List<Department>> QueryByNameFuzzy([FromQuery] string? name)
        {
            return ResultHelper.Success(_departmentService.QueryByNameFuzzy(name ?? ""));
        }
    }
}