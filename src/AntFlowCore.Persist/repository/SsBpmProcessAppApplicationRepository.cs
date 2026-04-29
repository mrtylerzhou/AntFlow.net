using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsBpmProcessAppApplicationRepository : RepositoryBase<BpmProcessAppApplication>, IBpmProcessAppApplicationRepository
{
    public SsBpmProcessAppApplicationRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmProcessAppApplication> GetApplicationUrl(string businessCode, string processKey)
    {
        return Db.Queryable<BpmProcessAppApplication>()
            .Where(a => a.BusinessCode.Equals(businessCode) && a.ProcessKey.Equals(processKey) && a.IsDel == 0)
            .ToList();
    }

    public List<BpmProcessAppApplication> SelectApplicationList()
    {
        return Db.Queryable<BpmProcessAppApplication>().ToList();
    }

    public List<BpmProcessAppApplicationVo> NewListPage(PagingInfo pagingInfo)
    {
        int totalCount = 0;
        List<BpmProcessAppApplicationVo> bpmProcessAppApplicationVos = Db.Queryable<BpmProcessAppApplication, OutSideBpmBusinessParty>(
                (app, party) => new JoinQueryInfos(JoinType.Left, app.BusinessCode == party.BusinessPartyMark))
            .Where((app, party) => app.IsDel == 0)
            .OrderByDescending((app, party) => app.CreateTime)
            .Select((app, party) => new BpmProcessAppApplicationVo
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
            })
            .ToPageList(pagingInfo.PageNumber, pagingInfo.PageSize, ref totalCount);
        pagingInfo.Count = totalCount;
        return bpmProcessAppApplicationVos;
    }

    public bool UpdateApplication(BpmProcessAppApplication entity)
    {
        int count = Db.Updateable(entity).ExecuteCommand();
        return count > 0;
    }

    public bool InsertApplication(BpmProcessAppApplication entity)
    {
        BpmProcessAppApplication result = Db.Insertable(entity).ExecuteReturnEntity();
        return result != null;
    }

    public bool ExistsByTitle(string title)
    {
        return Db.Queryable<BpmProcessAppApplication>().Any(a => a.Title == title && a.IsDel == 0);
    }
}
