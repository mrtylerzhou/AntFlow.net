using System.Collections;
using System.Reflection;
using System.Text.Json;
using AntFlowCore.Abstraction;
using AntFlowCore.Bpmn;
using AntFlowCore.Common.constant.enums;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.exception;
using AntFlowCore.Common.util;
using AntFlowCore.Common.util.Extension;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Entity;
using AntFlowCore.Enums;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;

namespace AntFlowCore.Persist.repository;

public class BpmVariableMessageService : AFBaseCurdRepositoryService<BpmVariableMessage>,IBpmVariableMessageService
{
    public BpmVariableMessageService(IFreeSql freeSql) : base(freeSql)
    {
    }
}