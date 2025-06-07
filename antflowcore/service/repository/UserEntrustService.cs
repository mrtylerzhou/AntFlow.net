using System.Linq.Expressions;
using antflowcore.dto;
using AntFlowCore.Entities;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.repository;

public class UserEntrustService :AFBaseCurdRepositoryService<UserEntrust>
{
    public UserEntrustService(IFreeSql freeSql) : base(freeSql)
    {
        
    }
    public ResultAndPage<Entrust> GetEntrustPageList(PageDto pageDto, Entrust vo, int type) {
        if (type == 1){
            vo.ReceiverId=SecurityUtils.GetLogInEmpIdSafe();
        }
        Page<Entrust> page = PageUtils.GetPageByPageDto<Entrust>(pageDto);
        List<Entrust> resultData = this.QueryEntrustPageList(page, vo.ReceiverId);
        if (resultData==null||!resultData.Any()) {
            return PageUtils.GetResultAndPage(page);
        }
        page.Records=resultData;
        return PageUtils.GetResultAndPage(page);
    }

    public void UpdateEntrustList(DataVo dataVo)
    {
        foreach (var idsVo in dataVo.Ids)
        {
            var userEntrust = new UserEntrust
            {
                UpdateUser = SecurityUtils.GetLogInEmpNameSafe(),
                BeginTime = dataVo.BeginTime,
                EndTime = dataVo.EndTime,
                ReceiverId = dataVo.ReceiverId,
                ReceiverName = dataVo.ReceiverName,
                Sender = dataVo.Sender
            };

            if (idsVo.Id > 0)
            {
                var existing = this.baseRepo.Where(a => a.Id == idsVo.Id).ToOne();
                if (existing == null)
                    throw new AFBizException("300001", "更新的记录不存在");

                userEntrust.Id = idsVo.Id??0;
                userEntrust.PowerId = existing.PowerId;
                userEntrust.CreateUser = SecurityUtils.GetLogInEmpNameSafe();
                this.baseRepo.Update(existing);
            }
            else if (!string.IsNullOrEmpty(idsVo.PowerId))
            {
                if (userEntrust.ReceiverId == null)
                    throw new AFBizException("300002", "请选择委托对象");

                userEntrust.PowerId = idsVo.PowerId;
                userEntrust.CreateUser = SecurityUtils.GetLogInEmpNameSafe();
                this.baseRepo.Insert(userEntrust);
                
            }
        }
    }
    public BaseIdTranStruVo GetEntrustEmployee(string employeeId, string employeeName, string powerId)
    {
        if (string.IsNullOrWhiteSpace(employeeId) || string.IsNullOrWhiteSpace(powerId))
        {
            return new BaseIdTranStruVo { Id = employeeId, Name = employeeName };
        }

        return GetEntrustEmployeeOnly(employeeId, employeeName, powerId);
    }
    public BaseIdTranStruVo GetEntrustEmployeeOnly(string employeeId, string employeeName, string powerId)
    {
        if (string.IsNullOrWhiteSpace(employeeId) || string.IsNullOrWhiteSpace(powerId))
        {
            return new BaseIdTranStruVo { Id = employeeId, Name = employeeName };
        }

        DateTime now = DateTime.Now;
        List<UserEntrust> list = this.baseRepo.Where(x => x.PowerId == powerId && x.Sender == employeeId).ToList();

        foreach (var u in list)
        {
            if (u.BeginTime.HasValue && u.EndTime.HasValue && 
                now >= u.BeginTime.Value.Date && now <= u.EndTime.Value.Date.AddDays(1).AddTicks(-1))
            {
                return new BaseIdTranStruVo { Id = u.ReceiverId, Name = u.ReceiverName };
            }
            else if (u.BeginTime.HasValue && !u.EndTime.HasValue && now >= u.BeginTime.Value.Date)
            {
                return new BaseIdTranStruVo { Id = u.ReceiverId, Name = u.ReceiverName };
            }
            else if (!u.BeginTime.HasValue && !u.EndTime.HasValue)
            {
                return new BaseIdTranStruVo { Id = u.ReceiverId, Name = u.ReceiverName };
            }
            else if (!u.BeginTime.HasValue && u.EndTime.HasValue && now <= u.EndTime.Value.Date)
            {
                return new BaseIdTranStruVo { Id = u.ReceiverId, Name = u.ReceiverName };
            }
        }

        return new BaseIdTranStruVo { Id = employeeId, Name = employeeName };
    }
    private List<Entrust> QueryEntrustPageList(Page<Entrust> page, string userId)
    {
        Expression<Func<UserEntrust,User, bool>> expression = (a,b) => 1 == 1;
        if (!string.IsNullOrEmpty(userId))
        {
            expression.And((a,b) => a.Sender==userId||a.ReceiverId==userId);
        }

        List<Entrust> entrusts = this.Frsql
            .Select<UserEntrust,User>()
            .LeftJoin((a,b)=>a.Sender==b.Id.ToString())
            .Where(expression)
            .ToList<Entrust>(a => new Entrust()
            {
                Id=a.t1.Id,
                Name = a.t2.Name,
                Sender = a.t1.Sender,
                ReceiverId = a.t1.ReceiverId,
                ReceiverName = a.t1.ReceiverName,
                PowerId = a.t1.PowerId,
                BeginTime = a.t1.BeginTime,
                EndTime = a.t1.EndTime,
                CreateTime = a.t1.CreateTime,
            });
        return entrusts;
    }

    public UserEntrust GetEntrustDetail(int id)
    {
        UserEntrust userEntrust = this.baseRepo.Where(a=>a.Id == id).ToOne();
        return userEntrust;
    }
}