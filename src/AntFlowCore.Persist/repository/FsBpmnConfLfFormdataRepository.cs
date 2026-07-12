using AntFlowCore.Abstraction.Orm.ext;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmnConfLfFormdataRepository : RepositoryBase<BpmnConfLfFormdata>, IBpmnConfLfFormdataRepository
{
    public FsBpmnConfLfFormdataRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public BpmnConfLfFormdata GetLFFormDataByFormCode(string formCode)
    {
        return _ormContext.FreeSql
            .Select<BpmnConfLfFormdata, BpmnConf>()
            .InnerJoin((a, b) => a.BpmnConfId == b.Id && b.EffectiveStatus == 1)
            .Where(m => m.t2.FormCode == formCode)
            .First();
    }

    /// <summary>
    /// 分页查询独立表单的当前生效版本（每族一行生效版本）
    /// </summary>
    public List<LfFormManageVo> ListEffectiveFormPage(Page<LfFormManageVo> page, LfFormManageVo vo)
    {
        var pagingInfo = page.ToPagingInfo().ToBasePagingInfo();

        // 1. 查询版本数(按 form_code 分组,统计所有非软删版本;在内存中分组以规避 FreeSql 分组聚合 API 类型推断问题)
        //    仅取 FormCode 列,数据量可控
        var allFormCodes = _ormContext.FreeSql
            .Select<BpmnConfLfFormdata>()
            .Where(a => a.BpmnConfId == null && a.IsDel == 0)
            .ToList(a => a.FormCode);

        var formCode2VersionCount = allFormCodes
            .Where(c => c != null)
            .GroupBy(c => c)
            .ToDictionary(g => g.Key, g => g.Count());

        // 2. 查询生效版本(每族至多一条,由 Effective 互斥保证) with 分页 + 搜索
        var query = _ormContext.FreeSql
            .Select<BpmnConfLfFormdata>()
            .Where(a => a.BpmnConfId == null && a.IsDel == 0 && a.EffectiveStatus == 1);

        if (!string.IsNullOrEmpty(vo.Search))
        {
            string search = vo.Search;
            query = query.Where(a => a.FormName.Contains(search) || a.FormCode.Contains(search));
        }

        // 总数
        page.Total = (int)query.Count();

        var pagedData = query
            .OrderByDescending(a => a.UpdateTime)
            .Page(pagingInfo)
            .ToList(a => new LfFormManageVo
            {
                Id = a.Id,
                FormCode = a.FormCode,
                FormName = a.FormName,
                EffectiveStatus = a.EffectiveStatus,
                CreateUser = a.CreateUser,
                CreateTime = a.CreateTime,
                UpdateUser = a.UpdateUser,
                UpdateTime = a.UpdateTime,
            });

        // 填充 VersionCount
        foreach (var item in pagedData)
        {
            if (item.FormCode != null && formCode2VersionCount.TryGetValue(item.FormCode, out var count))
            {
                item.VersionCount = count;
            }
        }

        return pagedData;
    }

    /// <summary>
    /// 查询某家族所有版本（历史版本查看，含已软删）
    /// </summary>
    public List<LfFormManageVo> ListVersionsByFormCode(string formCode)
    {
        var result = _ormContext.FreeSql
            .Select<BpmnConfLfFormdata>()
            .Where(a => a.FormCode == formCode && a.BpmnConfId == null)
            .OrderByDescending(a => a.Id)
            .ToList(a => new LfFormManageVo
            {
                Id = a.Id,
                FormCode = a.FormCode,
                FormName = a.FormName,
                EffectiveStatus = a.EffectiveStatus,
                CreateUser = a.CreateUser,
                CreateTime = a.CreateTime,
                UpdateUser = a.UpdateUser,
                UpdateTime = a.UpdateTime,
            });

        return result;
    }

    /// <summary>
    /// 所有生效独立表单（流程设计多选下拉框，含formdata以供前端解析条件字段）
    /// </summary>
    public List<LfFormManageVo> ListAllEffectiveForms()
    {
        var result = _ormContext.FreeSql
            .Select<BpmnConfLfFormdata>()
            .Where(a => a.BpmnConfId == null && a.IsDel == 0 && a.EffectiveStatus == 1)
            .OrderByDescending(a => a.UpdateTime)
            .ToList(a => new LfFormManageVo
            {
                Id = a.Id,
                FormCode = a.FormCode,
                FormName = a.FormName,
                Formdata = a.Formdata,
            });

        return result;
    }

    /// <summary>
    /// 按 id 列表批量查询（含已软删，供运行中流程实例读取）
    /// </summary>
    public List<BpmnConfLfFormdata> ListByIdsIgnoreDeleted(List<long> ids)
    {
        if (ids == null || !ids.Any())
        {
            return new List<BpmnConfLfFormdata>();
        }

        var result = _ormContext.FreeSql
            .Select<BpmnConfLfFormdata>()
            .Where(a => ids.Contains(a.Id))
            .ToList();

        // 保持传入 ids 的顺序(即 t_bpmn_conf.lf_formdata_ids 中设计时的选择顺序),
        // 而非数据库的默认排序(如按主键),避免多表单 tab 渲染顺序与设计时不一致
        var indexMap = ids
            .Select((id, index) => new { id, index })
            .ToDictionary(x => x.id, x => x.index);
        result.Sort((a, b) =>
        {
            indexMap.TryGetValue(a.Id, out var ia);
            indexMap.TryGetValue(b.Id, out var ib);
            return ia.CompareTo(ib);
        });

        return result;
    }

    /// <summary>
    /// 生成新的家族 formCode（返回最大值）
    /// </summary>
    public string? GetMaxFormCode(string prefix)
    {
        return _ormContext.FreeSql
            .Select<BpmnConfLfFormdata>()
            .Where(a => a.FormCode.StartsWith(prefix))
            .Max(a => a.FormCode);
    }
}
