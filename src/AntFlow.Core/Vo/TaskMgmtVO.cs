using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class TaskMgmtVO
{
    [JsonPropertyName("taskId")] public string TaskId { get; set; }

    [JsonPropertyName("taskName")] public string TaskName { get; set; }

    [JsonPropertyName("processName")] public string ProcessName { get; set; }

    [JsonPropertyName("processInstanceId")]
    public string ProcessInstanceId { get; set; }

    [JsonPropertyName("confId")] public long ConfId { get; set; }

    [JsonPropertyName("processId")] public string ProcessId { get; set; }

    [JsonPropertyName("processKey")] public string ProcessKey { get; set; }

    [JsonPropertyName("createTime")] public DateTime? CreateTime { get; set; }

    [JsonPropertyName("taskOwner")] public string TaskOwner { get; set; }

    [JsonPropertyName("description")] public string Description { get; set; }

    [JsonPropertyName("startTime")] public string StartTime { get; set; }

    [JsonPropertyName("runTime")] public DateTime? RunTime { get; set; }

    [JsonPropertyName("endTime")] public string EndTime { get; set; }

    [JsonPropertyName("applyUser")] public string ApplyUser { get; set; }

    [JsonPropertyName("applyUserName")] public string ApplyUserName { get; set; }

    [JsonPropertyName("applyDate")] public string ApplyDate { get; set; }

    [JsonPropertyName("applyDept")] public string ApplyDept { get; set; }

    [JsonPropertyName("actualName")] public string ActualName { get; set; }

    [JsonPropertyName("deptName")] public string DeptName { get; set; }

    [JsonPropertyName("originalName")] public string OriginalName { get; set; }

    [JsonPropertyName("processNumber")] public string ProcessNumber { get; set; }

    [JsonPropertyName("taskState")] public string TaskState { get; set; }

    [JsonPropertyName("taskStype")] public int TaskStype { get; set; }

    [JsonPropertyName("processState")] public int? ProcessState { get; set; }

    [JsonPropertyName("businessId")] public string BusinessId { get; set; }

    [JsonPropertyName("handleUserName")] public string HandleUserName { get; set; }

    [JsonPropertyName("search")] public string Search { get; set; }

    [JsonPropertyName("processType")] public string ProcessType { get; set; }

    [JsonPropertyName("processTypeName")] public string ProcessTypeName { get; set; }

    [JsonPropertyName("type")] public int Type { get; set; }

    [JsonPropertyName("code")] public string Code { get; set; }

    [JsonPropertyName("nodeType")] public int NodeType { get; set; }

    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("userId")] public string UserId { get; set; }

    [JsonPropertyName("userName")] public string UserName { get; set; }

    [JsonPropertyName("roleIds")] public List<int> RoleIds { get; set; }

    [JsonPropertyName("routeUrl")] public string RouteUrl { get; set; }

    [JsonPropertyName("entryId")] public string EntryId { get; set; }

    [JsonPropertyName("version")] public int Version { get; set; }

    [JsonPropertyName("appTime")] public string AppTime { get; set; }

    [JsonPropertyName("concernState")] public int ConcernState { get; set; }

    [JsonPropertyName("disagreeType")] public int DisagreeType { get; set; }

    [JsonPropertyName("operationType")] public int OperationType { get; set; }

    [JsonPropertyName("approvalComment")] public string ApprovalComment { get; set; }

    [JsonPropertyName("userIds")] public List<int> UserIds { get; set; }

    [JsonPropertyName("overtimeUrl")] public string OvertimeUrl { get; set; }

    [JsonPropertyName("isLeftStroke")] public bool IsLeftStroke { get; set; }

    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("isBatchSubmit")] public bool IsBatchSubmit { get; set; }

    [JsonPropertyName("processKeyList")] public List<string> ProcessKeyList { get; set; }

    [JsonPropertyName("isForward")] public bool IsForward { get; set; }

    [JsonPropertyName("isOld")] public bool IsOld { get; set; }

    [JsonPropertyName("taskIds")] public List<string> TaskIds { get; set; }

    [JsonPropertyName("changeHandlers")] public List<ContansDataVo> ChangeHandlers { get; set; }

    [JsonPropertyName("createUser")] public string CreateUser { get; set; }

    [JsonPropertyName("todoCount")] public int TodoCount { get; set; }

    [JsonPropertyName("doneTodayCount")] public int DoneTodayCount { get; set; }

    [JsonPropertyName("doneCreateCount")] public int DoneCreateCount { get; set; }

    [JsonPropertyName("draftCount")] public int DraftCount { get; set; }

    [JsonPropertyName("processCode")] public string ProcessCode { get; set; }

    [JsonPropertyName("processNumbers")] public List<string> ProcessNumbers { get; set; }

    [JsonPropertyName("processDigest")] public string ProcessDigest { get; set; }

    [JsonPropertyName("isRead")] public int IsRead { get; set; }

    [JsonPropertyName("headImg")] public string HeadImg { get; set; }

    [JsonPropertyName("departmentPath")] public string DepartmentPath { get; set; }

    [JsonPropertyName("applyUserId")] public int ApplyUserId { get; set; }

    [JsonPropertyName("isOutSideProcess")] public bool IsOutSideProcess { get; set; }

    [JsonPropertyName("isLowCodeFlow")] public bool IsLowCodeFlow { get; set; }

    [JsonPropertyName("accessType")] public int AccessType { get; set; }

    [JsonPropertyName("versionProcessKeys")]
    public List<string> VersionProcessKeys { get; set; }
}