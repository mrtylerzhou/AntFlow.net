using AntFlowCore.Constants;
using antflowcore.exception;
using antflowcore.util;
using FreeSql.DataAnnotations;

namespace antflowcore.entity;

using FreeSql.DataAnnotations;
using System;
using System.Text.RegularExpressions;


    [Table(Name = "t_bpmn_conf")]
    public class BpmnConf
    {
        /// <summary>
        /// auto incr id
        /// </summary>
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// bpmn Code
        /// </summary>
        [Column(Name = "bpmn_code")]
        public string BpmnCode { get; set; }

        /// <summary>
        /// bpmn Name
        /// </summary>
        [Column(Name = "bpmn_name")]
        public string BpmnName { get; set; }

        /// <summary>
        /// bpmn Type
        /// </summary>
        [Column(Name = "bpmn_type")]
        public int? BpmnType { get; set; }

        /// <summary>
        /// formCode
        /// </summary>
        [Column(Name = "form_code")]
        public string FormCode { get; set; }

        /// <summary>
        /// appId
        /// </summary>
        [Column(Name = "app_id")]
        public int? AppId { get; set; }

        /// <summary>
        /// dedup type(1 - no dedup; 2 - dedup forward; 3 - dedup backward)
        /// </summary>
        [Column(Name = "deduplication_type")]
        public int? DeduplicationType { get; set; }

        /// <summary>
        /// effective status 0 for no and 1 for yes
        /// </summary>
        [Column(Name = "effective_status")]
        public int EffectiveStatus { get; set; }

        /// <summary>
        /// is for all 0 no and 1 yes
        /// </summary>
        [Column(Name = "is_all")]
        public int IsAll { get; set; }

        /// <summary>
        /// is third party process 0 for no and 1 yes
        /// </summary>
        [Column(Name = "is_out_side_process")]
        public int? IsOutSideProcess { get; set; }

        [Column(Name = "is_lowcode_flow")]
        public int? IsLowCodeFlow { get; set; }

        /// <summary>
        /// business party mark
        /// </summary>
        [Column(Name = "business_party_id")]
        public int? BusinessPartyId { get; set; }

        /// <summary>
        /// remark
        /// </summary>
        [Column(Name = "remark")]
        public string Remark { get; set; }

        /// <summary>
        /// is del 0 for no and 1 yes
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// create user
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// create time
        /// </summary>
        [Column(Name = "create_time",IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// update user
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// update time
        /// </summary>
        [Column(Name = "update_time",IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }

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
