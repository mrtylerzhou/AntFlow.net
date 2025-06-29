using AntFlowCore.Constants;
using antflowcore.exception;
using antflowcore.util;
namespace antflowcore.entity;

using System;
using System.Text.RegularExpressions;


   public class BpmnConf
{
    /// <summary>
    /// auto incr id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// bpmn Code
    /// </summary>
    public string BpmnCode { get; set; }

    /// <summary>
    /// bpmn Name
    /// </summary>
    public string BpmnName { get; set; }

    /// <summary>
    /// bpmn Type
    /// </summary>
    public int? BpmnType { get; set; }

    /// <summary>
    /// formCode
    /// </summary>
    public string FormCode { get; set; }

    /// <summary>
    /// appId
    /// </summary>
    public int? AppId { get; set; }

    /// <summary>
    /// dedup type(1 - no dedup; 2 - dedup forward; 3 - dedup backward)
    /// </summary>
    public int? DeduplicationType { get; set; }

    /// <summary>
    /// effective status 0 for no and 1 for yes
    /// </summary>
    public int EffectiveStatus { get; set; }

    /// <summary>
    /// is for all 0 no and 1 yes
    /// </summary>
    public int IsAll { get; set; }

    /// <summary>
    /// is third party process 0 for no and 1 yes
    /// </summary>
    public int? IsOutSideProcess { get; set; }

    public int? IsLowCodeFlow { get; set; }

    /// <summary>
    /// business party mark
    /// </summary>
    public long? BusinessPartyId { get; set; }

    /// <summary>
    /// remark
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// is del 0 for no and 1 yes
    /// </summary>
    public int IsDel { get; set; }

    /// <summary>
    /// create user
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// create time
    /// </summary>
    public DateTime? CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// update user
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    /// update time
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    public int? ExtraFlags { get; set; }

    /// <summary>
    /// Validate BpmnName for business rules.
    /// </summary>
    /// <param name="bpmnName">Bpmn Name</param>
    public void SetBpmnName(string bpmnName)
    {
        ValidateBpmnName(bpmnName);
        this.BpmnName = bpmnName;
    }

    /// <summary>
    /// Validate BpmnName according to business rules.
    /// </summary>
    /// <param name="bpmnName">Bpmn Name</param>
    public static void ValidateBpmnName(string bpmnName)
    {
        if (string.IsNullOrEmpty(bpmnName))
        {
            throw new AFBizException("审批流名称必须存在!");
        }

        if (Regex.IsMatch(bpmnName, PATTERN))
        {
            throw new AFBizException("审批流名称不合法");
        }

        if (string.IsNullOrWhiteSpace(bpmnName))
        {
            throw new AFBizException("审批流名称不得包含空格");
        }

        if (Regex.IsMatch(bpmnName, StringConstants.SPECIAL_CHARACTERS))
        {
            throw new AFBizException("审批流名称中不得包含特殊字符!");
        }

        if (bpmnName.Length > NumberConstants.BPMN_NAME_MAX_LEN)
        {
            throw new AFBizException("审批流名称过长");
        }
    }

    /// <summary>
    /// Pattern for BpmnCode
    /// </summary>
    public static readonly int BPMN_CODE_LEN = 5;

    /// <summary>
    /// Pattern for BpmnCode
    /// </summary>
    public static readonly string PATTERN = @".*-([0-9]{" + BPMN_CODE_LEN + "})";

    /// <summary>
    /// Format Mark
    /// </summary>
    public static readonly string FormatMark = string.Format("%0{0}d", BPMN_CODE_LEN);
}
