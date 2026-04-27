using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class UserEntrustService : IUserEntrustService
{
    public UserEntrustService(IUserEntrustRepository repository)
    {
        _repository = repository;
    }

    public IUserEntrustRepository _repository { get; }

    public ResultAndPage<Entrust> GetEntrustPageList(PageDto pageDto, Entrust vo, int type) {
        if (type == 1){
            vo.ReceiverId=SecurityUtils.GetLogInEmpIdSafe();
        }
        Page<Entrust> page = PageUtils.GetPageByPageDto<Entrust>(pageDto);
        List<Entrust> resultData = _repository.QueryEntrustPageList(vo.ReceiverId);
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
            UserEntrust userEntrust = new UserEntrust
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
                var existing = _repository.FirstOrDefault(a => a.Id == idsVo.Id);
                if (existing == null)
                    throw new AFBizException("300001", "更新的记录不存在");

                userEntrust.Id = idsVo.Id??0;
                userEntrust.PowerId = existing.PowerId;
                userEntrust.CreateUser = SecurityUtils.GetLogInEmpNameSafe();
                _repository.Update(existing);
            }
            else if (!string.IsNullOrEmpty(idsVo.PowerId))
            {
                if (userEntrust.ReceiverId == null)
                    throw new AFBizException("300002", "请选择委托对象");

                userEntrust.PowerId = idsVo.PowerId;
                userEntrust.CreateUser = SecurityUtils.GetLogInEmpNameSafe();
                userEntrust.TenantId = MultiTenantUtil.GetCurrentTenantId();
                List<UserEntrust> userEntrusts = _repository
                    .Find(a=>a.Sender==userEntrust.Sender
                              &&a.ReceiverId==userEntrust.ReceiverId
                              &&a.PowerId==userEntrust.PowerId);
                if (!userEntrusts.IsEmpty())
                {
                    throw new AFBizException(BusinessError.DATA_ALREADY_EXISTED, "委托记录已存在,请确认!");
                }
                _repository.Add(userEntrust);

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
        List<UserEntrust> list = _repository.Find(x => x.PowerId == powerId && x.Sender == employeeId);
        if (string.IsNullOrEmpty(MultiTenantUtil.GetCurrentTenantId()))
        {
            List<UserEntrust> currentOnes =
                list.Where(a => a.TenantId == MultiTenantUtil.GetCurrentTenantId()).ToList();
            //如果当前租户有添加,则取当前租户的,如果没有,则尝试取全局的
            if(currentOnes.IsEmpty()&&!MultiTenantUtil.StrictTenantMode())
            {

                list = list.Where(a => !string.IsNullOrEmpty(a.TenantId)).ToList();
            }else{
                list=currentOnes;
            }
        }
        foreach (UserEntrust u in list)
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

    public UserEntrust GetEntrustDetail(int id)
    {
        UserEntrust userEntrust = _repository.FirstOrDefault(a=>a.Id == id);
        return userEntrust;
    }
}
