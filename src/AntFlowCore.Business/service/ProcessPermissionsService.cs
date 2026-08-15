using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service
{
    /// <summary>
    /// 流程权限管理 业务服务. 对应 Java ProcessPermissionsBizServiceImpl.
    /// </summary>
    public class ProcessPermissionsService : IProcessPermissionsService
    {
        public IProcessPermissionsRepository _repository { get; }
        private readonly IBpmnConfService _bpmnConfService;
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;
        private readonly IRoleService _roleService;

        public ProcessPermissionsService(
            IProcessPermissionsRepository repository,
            IBpmnConfService bpmnConfService,
            IUserService userService,
            IDepartmentService departmentService,
            IRoleService roleService)
        {
            _repository = repository;
            _bpmnConfService = bpmnConfService;
            _userService = userService;
            _departmentService = departmentService;
            _roleService = roleService;
        }

        // ==================== 列表 ====================

        public ResultAndPage<ProcessPermissionsListVo> ListPage(ProcessPermissionsPageReq req)
        {
            PageDto pageDto = req.PageDto ?? PageDto.First();
            Page<BpmProcessPermissions> page = PageUtils.GetPageByPageDto<BpmProcessPermissions>(pageDto);
            string tenantId = MultiTenantUtil.GetCurrentTenantId();

            //授权对象名称关键字 -> 后置解析用户/部门/角色 id 集合(不 join demo 表)
            List<string> userIds = new();
            List<string> depIds = new();
            List<string> roleIds = new();
            if (!string.IsNullOrEmpty(req.ObjectName))
            {
                userIds = ResolveUserIdsByName(req.ObjectName);
                depIds = ResolveDepIdsByName(req.ObjectName);
                roleIds = ResolveRoleIdsByName(req.ObjectName);
                if (userIds.Count == 0 && depIds.Count == 0 && roleIds.Count == 0)
                {
                    return PageUtils.GetResultAndPage(new List<ProcessPermissionsListVo>(), PageUtils.GetPageDto(page));
                }
            }

            List<BpmProcessPermissions> records = _repository.QueryPageList(req, userIds, depIds, roleIds, tenantId, page);
            List<ProcessPermissionsListVo> vos = BuildListVos(records);
            return PageUtils.GetResultAndPage(vos, PageUtils.GetPageDto(page));
        }

        private List<ProcessPermissionsListVo> BuildListVos(List<BpmProcessPermissions> records)
        {
            //1. 流程名称: formCode -> bpmn_name(effective_status=1)
            List<string> formCodes = records.Select(r => r.ProcessKey)
                .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            Dictionary<string, string> formCode2Name = new();
            if (formCodes.Count > 0)
            {
                foreach (BpmnConf c in _bpmnConfService._repository
                    .Find(c => formCodes.Contains(c.FormCode) && c.EffectiveStatus == 1))
                {
                    formCode2Name.TryAdd(c.FormCode, c.BpmnName);
                }
            }

            //2. 人员名称: objectType=1 -> name
            List<string> userIds = records.Where(r => r.ObjectType == 1)
                .Select(r => r.ObjectId).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            Dictionary<string, string> userId2Name = new();
            if (userIds.Count > 0)
            {
                foreach (var u in _userService.QueryUserByIds(userIds))
                {
                    userId2Name.TryAdd(u.Id, u.Name);
                }
            }

            //3. 部门名称: objectType=2 -> name
            List<int> depIds = records.Where(r => r.ObjectType == 2)
                .Select(r => r.ObjectId)
                .Where(x => !string.IsNullOrEmpty(x) && int.TryParse(x, out _))
                .Select(int.Parse).Distinct().ToList();
            Dictionary<int, string> depId2Name = new();
            if (depIds.Count > 0)
            {
                foreach (Department d in _departmentService._repository.Find(d => depIds.Contains(d.Id)))
                {
                    depId2Name.TryAdd(d.Id, d.Name);
                }
            }

            //4. 角色名称: objectType=3 -> name
            List<string> roleIds = records.Where(r => r.ObjectType == 3)
                .Select(r => r.ObjectId).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            Dictionary<string, string> roleId2Name = new();
            if (roleIds.Count > 0)
            {
                foreach (var r in _roleService.QueryRoleByIds(roleIds))
                {
                    roleId2Name.TryAdd(r.Id, r.Name);
                }
            }

            //5. 创建人名称: createUser id -> name
            List<string> createUserIds = records.Select(r => r.CreateUser)
                .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            Dictionary<string, string> createUserId2Name = new();
            if (createUserIds.Count > 0)
            {
                foreach (var c in _userService.QueryUserByIds(createUserIds))
                {
                    createUserId2Name.TryAdd(c.Id, c.Name);
                }
            }

            return records.Select(r =>
            {
                int objectType = r.ObjectType ?? 1;
                string objectName = objectType switch
                {
                    3 => roleId2Name.GetValueOrDefault(r.ObjectId, r.ObjectId),
                    2 => int.TryParse(r.ObjectId, out int depId) && depId2Name.TryGetValue(depId, out string depName)
                            ? depName : r.ObjectId,
                    _ => userId2Name.GetValueOrDefault(r.ObjectId, r.ObjectId),
                };
                return new ProcessPermissionsListVo
                {
                    Id = r.Id,
                    ProcessKey = r.ProcessKey,
                    BpmnName = formCode2Name.GetValueOrDefault(r.ProcessKey),
                    PermissionsType = r.PermissionsType,
                    IsDepartment = objectType == 2,
                    ObjectType = objectType,
                    ObjectName = objectName,
                    CreateUser = r.CreateUser,
                    CreateUserName = createUserId2Name.GetValueOrDefault(r.CreateUser, r.CreateUser),
                    CreateTime = r.CreateTime,
                };
            }).ToList();
        }

        // ==================== 保存(三层笛卡尔积, 幂等跳过) ====================

        public ProcessPermissionsSaveResult Save(ProcessPermissionsSaveVo vo)
        {
            if (vo.ProcessKeys == null || vo.ProcessKeys.Count == 0)
            {
                throw new AFBizException("400001", "请选择流程");
            }
            if (vo.PermissionsTypes == null || vo.PermissionsTypes.Count == 0)
            {
                throw new AFBizException("400002", "请选择权限类型");
            }
            //对象类型: 兼容旧调用(IsDepartment)与新增的 ObjectType(1=人员 2=部门 3=角色)
            int objectType = vo.ObjectType ?? (vo.IsDepartment == true ? 2 : 1);
            if (objectType != 1 && objectType != 2 && objectType != 3)
            {
                throw new AFBizException("400008", "授权对象类型不合法");
            }
            //部门权限禁止监控/模板编辑
            if (objectType == 2 && (vo.PermissionsTypes.Contains(3) || vo.PermissionsTypes.Contains(4)))
            {
                throw new AFBizException("400003", "部门权限不支持选择监控/模板编辑权限");
            }
            if (vo.ObjectIds == null || vo.ObjectIds.Count == 0)
            {
                string msg = objectType == 1 ? "请选择人员" : (objectType == 2 ? "请选择部门" : "请选择角色");
                throw new AFBizException(objectType == 1 ? "400005" : (objectType == 2 ? "400004" : "400007"), msg);
            }

            string loginUserId = SecurityUtils.GetLogInEmpIdStr();
            string tenantId = MultiTenantUtil.GetCurrentTenantId();
            List<BpmProcessPermissions> toInsert = new();
            int skipCount = 0;
            foreach (string processKey in vo.ProcessKeys)
            {
                foreach (int permissionsType in vo.PermissionsTypes)
                {
                    foreach (string objectId in vo.ObjectIds)
                    {
                        if (Exists(processKey, permissionsType, objectType, objectId))
                        {
                            skipCount++;
                        }
                        else
                        {
                            toInsert.Add(new BpmProcessPermissions
                            {
                                ProcessKey = processKey,
                                PermissionsType = permissionsType,
                                ObjectType = objectType,
                                ObjectId = objectId,
                                CreateUser = loginUserId,
                                CreateTime = DateTime.Now,
                                IsDel = 0,
                                TenantId = tenantId,
                            });
                        }
                    }
                }
            }
            foreach (BpmProcessPermissions entity in toInsert)
            {
                _repository.Add(entity);
            }
            return new ProcessPermissionsSaveResult
            {
                InsertCount = toInsert.Count,
                SkipCount = skipCount,
            };
        }

        /// <summary>
        /// 幂等判断: process_key + permissions_type + object_type + object_id 四者一致视为已存在
        /// </summary>
        private bool Exists(string processKey, int permissionsType, int objectType, string objectId)
        {
            return _repository.Any(a => a.ProcessKey == processKey
                && a.PermissionsType == permissionsType
                && a.ObjectType == objectType
                && a.ObjectId == objectId);
        }

        // ==================== 删除(物理) ====================

        public void Delete(long id)
        {
            BpmProcessPermissions permission = _repository.FirstOrDefault(a => a.Id == id);
            if (permission == null)
            {
                throw new AFBizException("400006", "权限记录不存在");
            }
            _repository.Remove(permission);
        }

        // ==================== 名称解析 ====================

        private List<string> ResolveUserIdsByName(string name)
        {
            return _userService.QueryUserByNameFuzzy(name).Select(u => u.Id).ToList();
        }

        private List<string> ResolveDepIdsByName(string name)
        {
            return _departmentService.QueryByNameFuzzy(name).Select(d => d.Id.ToString()).ToList();
        }

        private List<string> ResolveRoleIdsByName(string name)
        {
            return _roleService.QueryRoleByNameFuzzy(name).Select(r => r.Id).ToList();
        }
    }
}