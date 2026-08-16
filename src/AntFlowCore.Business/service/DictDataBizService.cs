using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service
{
    /// <summary>
    /// 字典管理 业务服务. 对应 Java DictDataBizServiceImpl.
    /// 规则: 列表过滤 is_del=0; lowcodeflow 系统数据禁止编辑/删除;
    ///       dict_type+dict_label+dict_value 三者全同(is_del=0 范围)禁止新增/编辑.
    /// </summary>
    public class DictDataBizService : IDictDataBizService
    {
        private readonly IDictDataRepository _repository;

        public DictDataBizService(IDictDataRepository repository)
        {
            _repository = repository;
        }

        // ==================== 列表 ====================

        public ResultAndPage<DictDataVo> ListPage(DictDataPageReq req)
        {
            PageDto pageDto = req.PageDto ?? PageDto.First();
            Page<DictData> page = PageUtils.GetPageByPageDto<DictData>(pageDto);
            string tenantId = MultiTenantUtil.GetCurrentTenantId();
            List<DictData> records = _repository.QueryPageList(req, tenantId, page);
            List<DictDataVo> vos = records.Select(ToVo).ToList();
            return PageUtils.GetResultAndPage(vos, PageUtils.GetPageDto(page));
        }

        private DictDataVo ToVo(DictData e)
        {
            return new DictDataVo
            {
                Id = e.Id,
                DictLabel = e.Label,
                DictValue = e.Value,
                DictType = e.DictType,
                //后端映射汉字含义, 未知类型原样展示
                DictTypeLabel = AFSpecialDictCategoryEnumExtensions.GetLabelByDesc(e.DictType),
                Sort = e.Sort,
                Remark = e.Remark,
                CreateUser = e.CreateUser,
                CreateTime = e.CreateTime,
                UpdateTime = e.UpdateTime,
            };
        }

        // ==================== 新增 ====================

        public long Save(DictDataSaveVo vo)
        {
            ValidateSaveVo(vo);
            //唯一性校验: dict_type+dict_label+dict_value 三者全同(is_del=0 范围)禁止添加
            if (ExistsSame(vo.DictType, vo.DictLabel, vo.DictValue, null))
            {
                throw new AFBizException("400001", "相同字典数据已存在");
            }
            DictData entity = new DictData
            {
                DictType = vo.DictType,
                Label = vo.DictLabel,
                Value = vo.DictValue,
                Sort = vo.Sort ?? 0,
                Remark = vo.Remark,
                IsDefault = "N",
                IsDel = 0,
                CreateUser = SecurityUtils.GetLogInEmpNameSafe(),
                CreateTime = DateTime.Now,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };
            _repository.Add(entity);
            return entity.Id;
        }

        // ==================== 编辑 ====================

        public void Update(DictDataSaveVo vo)
        {
            if (vo.Id == null || vo.Id.Value <= 0)
            {
                throw new AFBizException("400002", "参数错误:缺少主键");
            }
            ValidateSaveVo(vo);
            DictData exist = _repository.FirstOrDefault(a => a.Id == vo.Id.Value);
            if (exist == null || exist.IsDel == 1)
            {
                throw new AFBizException("400003", "字典数据不存在");
            }
            //lowcodeflow 系统数据禁止编辑
            if (AFSpecialDictCategoryEnumExtensions.IsLowCodeFlow(exist.DictType))
            {
                throw new AFBizException("400004", "低代码流程数据禁止编辑");
            }
            //唯一性校验(排除自身)
            if (ExistsSame(vo.DictType, vo.DictLabel, vo.DictValue, vo.Id.Value))
            {
                throw new AFBizException("400001", "相同字典数据已存在");
            }
            exist.DictType = vo.DictType;
            exist.Label = vo.DictLabel;
            exist.Value = vo.DictValue;
            exist.Sort = vo.Sort ?? 0;
            exist.Remark = vo.Remark;
            exist.UpdateUser = SecurityUtils.GetLogInEmpNameSafe();
            exist.UpdateTime = DateTime.Now;
            _repository.Update(exist);
        }

        // ==================== 删除(逻辑删除 is_del=1) ====================

        public void Delete(long id)
        {
            DictData exist = _repository.FirstOrDefault(a => a.Id == id);
            if (exist == null || exist.IsDel == 1)
            {
                throw new AFBizException("400003", "字典数据不存在");
            }
            //lowcodeflow 系统数据禁止删除
            if (AFSpecialDictCategoryEnumExtensions.IsLowCodeFlow(exist.DictType))
            {
                throw new AFBizException("400004", "低代码流程数据禁止删除");
            }
            exist.IsDel = 1;
            exist.UpdateUser = SecurityUtils.GetLogInEmpNameSafe();
            exist.UpdateTime = DateTime.Now;
            _repository.Update(exist);
        }

        // ==================== 私有方法 ====================

        private void ValidateSaveVo(DictDataSaveVo vo)
        {
            if (string.IsNullOrEmpty(vo.DictLabel))
            {
                throw new AFBizException("400005", "字典标签不能为空");
            }
            if (string.IsNullOrEmpty(vo.DictValue))
            {
                throw new AFBizException("400006", "字典键值不能为空");
            }
            if (string.IsNullOrEmpty(vo.DictType))
            {
                throw new AFBizException("400007", "字典类型不能为空");
            }
            //lowcodeflow 系统自动写入, 不允许手动新增
            if (AFSpecialDictCategoryEnumExtensions.IsLowCodeFlow(vo.DictType))
            {
                throw new AFBizException("400008", "低代码流程类型不允许手动新增");
            }
            //仅允许 udr/processlabel 两个手动类型
            if (AFSpecialDictCategoryEnumExtensions.GetLabelByDesc(vo.DictType) == null)
            {
                throw new AFBizException("400009", "字典类型不合法");
            }
        }

        /// <summary>
        /// 唯一性校验: dict_type+dict_label+dict_value 三者全同(is_del=0 范围)视为已存在
        /// </summary>
        /// <param name="excludeId">编辑时排除自身</param>
        private bool ExistsSame(string dictType, string dictLabel, string dictValue, long? excludeId)
        {
            return _repository.Any(a => a.DictType == dictType
                && a.Label == dictLabel
                && a.Value == dictValue
                && a.IsDel == 0
                && (excludeId == null || a.Id != excludeId.Value));
        }
    }
}