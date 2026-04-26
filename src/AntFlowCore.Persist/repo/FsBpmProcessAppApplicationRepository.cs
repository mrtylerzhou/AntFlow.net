using AntFlowCore.Abstraction.Orm.ext;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;
using FreeSql.Internal.Model;

namespace antflowcore.conf.ef;

public class FsBpmProcessAppApplicationRepository : RepositoryBase<BpmProcessAppApplication>, IBpmProcessAppApplicationRepository
{
    public FsBpmProcessAppApplicationRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmProcessAppApplication> GetApplicationUrl(string businessCode, string processKey)
    {
        return Find(a =>
            a.BusinessCode.Equals(businessCode) && a.ProcessKey.Equals(processKey) && a.IsDel == 0).ToList();
    }

    public List<BpmProcessAppApplication> SelectApplicationList()
    {
        return Find(a => true).ToList();
    }

    public List<BpmProcessAppApplicationVo> NewListPage(PagingInfo pagingInfo)
    {
        BasePagingInfo basePagingInfo = pagingInfo.ToBasePagingInfo();
        List<BpmProcessAppApplicationVo> bpmProcessAppApplicationVos = _ormContext.FreeSql.Select<BpmProcessAppApplication, OutSideBpmBusinessParty>()
            .LeftJoin((app, party) => app.BusinessCode == party.BusinessPartyMark)
            .Where((app, party) => app.IsDel == 0)
            .OrderByDescending((app, party) => app.CreateTime)
            .Page(basePagingInfo)
            .ToList((app, party) => new BpmProcessAppApplicationVo
            {
                Id = app.Id,
                BusinessName = party.Name,
                BusinessPartyId = party.Id,
                Title = app.Title,
                BusinessCode = app.BusinessCode,
                ApplyType = app.ApplyType,
                PcIcon = app.PcIcon,
                EffectiveSource = app.EffectiveSource,
                IsSon = app.IsSon,
                LookUrl = app.LookUrl,
                SubmitUrl = app.SubmitUrl,
                ConditionUrl = app.ConditionUrl,
                ParentId = app.ParentId,
                ApplicationUrl = app.ApplicationUrl,
                Route = app.Route,
                ProcessKey = app.ProcessKey,
                PermissionsCode = app.PermissionsCode,
                CreateUserId = app.CreateUserId,
                CreateTime = app.CreateTime
            });
        pagingInfo.Count = basePagingInfo.Count;
        pagingInfo.PageSize = basePagingInfo.PageSize;
        pagingInfo.PageNumber = basePagingInfo.PageNumber;
        return bpmProcessAppApplicationVos;
    }

    public bool UpdateApplication(BpmProcessAppApplication entity)
    {
        int count = _ormContext.FreeSql.GetRepository<BpmProcessAppApplication>().Update(entity);
        return count > 0;
    }

    public bool InsertApplication(BpmProcessAppApplication entity)
    {
        BpmProcessAppApplication bpmProcessAppApplication = _ormContext.FreeSql.GetRepository<BpmProcessAppApplication>().Insert(entity);
        return bpmProcessAppApplication != null;
    }

    public bool ExistsByTitle(string title)
    {
        return GetQueryable().Where(a => a.Title == title && a.IsDel == 0).Any();
    }
}
