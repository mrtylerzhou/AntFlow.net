using System;
using System.Collections.Generic;

namespace AntFlowCore.Enums
{
    public enum ProcessOperationEnum
    {
        BUTTON_TYPE_SUBMIT = 1,            // 流程提交
        BUTTON_TYPE_RESUBMIT = 2,          // 重新提交
        BUTTON_TYPE_AGREE = 3,             // 同意
        BUTTON_TYPE_DIS_AGREE = 4,         // 不同意
        BUTTON_TYPE_VIEW_BUSINESS_PROCESS = 5, // 查看流程详情
        BUTTON_TYPE_ABANDON = 7,           // 作废
        BUTTON_TYPE_UNDERTAKE = 10,        // 承办
        BUTTON_TYPE_CHANGE_ASSIGNEE = 11,  // 变更处理人
        BUTTON_TYPE_STOP = 12,             // 终止
        BUTTON_TYPE_FORWARD = 15,          // 转发
        BUTTON_TYPE_BACK_TO_MODIFY = 18,   // 打回修改
        BUTTON_TYPE_JP = 19,               // 加批
        BUTTON_TYPE_ZB = 21,                // 转办
        BUTTON_TYPE_CHOOSE_ASSIGNEE=22,//自选审批人
        BUTTON_TYPE_BACK_TO_ANY_NODE=23,//退回任意节点
        BUTTON_TYPE_REMOVE_ASSIGNEE=24,//减签
        BUTTON_TYPE_ADD_ASSIGNEE=25,//加签//加批生成了新的节点,加签在当前节点增加审批人
        BUTTON_TYPE_CHANGE_FUTURE_ASSIGNEE=26,//变更未来节点处理人
        BUTTON_TYPE_REMOVE_FUTURE_ASSIGNEE=27,//未来节点减签
        BUTTON_TYPE_ADD_FUTURE_ASSIGNEE=28,//未来节点加签
        BUTTON_TYPE_PROCESS_DRAW_BACK=29,//流程撤回
        BUTTON_TYPE_SAVE_DRAFT=30,//保存草稿
        BUTTON_TYPE_RECOVER_TO_HIS=31,//恢复已结束流程
        BUTTON_TYPE_DRAW_BACK_AGREE=32,//撤销同意,同意之后反悔了,撤销同意重新审批
        BUTTON_TYPE_PROCESS_MOVE_AHEAD=33,//流程推进,管理员向前推进流程进度
        
    }

    public static class ProcessOperationEnumExtensions
    {
        // 获取描述
        public static string GetDesc(this ProcessOperationEnum operation)
        {
            switch (operation)
            {
                case ProcessOperationEnum.BUTTON_TYPE_SUBMIT:
                    return "流程提交";
                case ProcessOperationEnum.BUTTON_TYPE_RESUBMIT:
                    return "重新提交";
                case ProcessOperationEnum.BUTTON_TYPE_AGREE:
                    return "同意";
                case ProcessOperationEnum.BUTTON_TYPE_DIS_AGREE:
                    return "不同意";
                case ProcessOperationEnum.BUTTON_TYPE_VIEW_BUSINESS_PROCESS:
                    return "查看流程详情";
                case ProcessOperationEnum.BUTTON_TYPE_ABANDON:
                    return "作废";
                case ProcessOperationEnum.BUTTON_TYPE_UNDERTAKE:
                    return "承办";
                case ProcessOperationEnum.BUTTON_TYPE_CHANGE_ASSIGNEE:
                    return "变更处理人";
                case ProcessOperationEnum.BUTTON_TYPE_STOP:
                    return "终止";
                case ProcessOperationEnum.BUTTON_TYPE_FORWARD:
                    return "转发";
                case ProcessOperationEnum.BUTTON_TYPE_BACK_TO_MODIFY:
                    return "打回修改";
                case ProcessOperationEnum.BUTTON_TYPE_JP:
                    return "加批";
                case ProcessOperationEnum.BUTTON_TYPE_ZB:
                    return "转办";
                case ProcessOperationEnum.BUTTON_TYPE_CHOOSE_ASSIGNEE:
                    return "自选审批人";
                case ProcessOperationEnum.BUTTON_TYPE_BACK_TO_ANY_NODE:
                    return "退回任意节点";
                case ProcessOperationEnum.BUTTON_TYPE_REMOVE_ASSIGNEE:
                    return "减签";
                case ProcessOperationEnum.BUTTON_TYPE_ADD_ASSIGNEE:
                    return "加签";
                case ProcessOperationEnum.BUTTON_TYPE_CHANGE_FUTURE_ASSIGNEE:
                    return "变更未来节点处理人";
                case ProcessOperationEnum.BUTTON_TYPE_REMOVE_FUTURE_ASSIGNEE:
                    return "未来节点减签";
                case ProcessOperationEnum.BUTTON_TYPE_ADD_FUTURE_ASSIGNEE:
                    return "未来节点加签";
                case ProcessOperationEnum.BUTTON_TYPE_PROCESS_DRAW_BACK:
                    return "撤回";
                case ProcessOperationEnum.BUTTON_TYPE_SAVE_DRAFT:
                    return "保存草稿";
                case ProcessOperationEnum.BUTTON_TYPE_RECOVER_TO_HIS:
                    return "恢复已结束流程";
                case ProcessOperationEnum.BUTTON_TYPE_DRAW_BACK_AGREE:
                    return "撤销同意";
                case ProcessOperationEnum.BUTTON_TYPE_PROCESS_MOVE_AHEAD:
                    return "流程推进";
                default:
                    return null;
            }
        }

        // 根据Code获得枚举
        public static ProcessOperationEnum? GetEnumByCode(int? code)
        {
            if (code == null)
            {
                return null;
            }
            if (Enum.IsDefined(typeof(ProcessOperationEnum), code))
            {
                return (ProcessOperationEnum)code;
            }
            return null;
        }

        // 获取所有的枚举项及其描述
        public static Dictionary<int, string> GetAllEnums()
        {
            var enumDescriptions = new Dictionary<int, string>();
            foreach (var value in Enum.GetValues(typeof(ProcessOperationEnum)))
            {
                enumDescriptions.Add((int)value, ((ProcessOperationEnum)value).GetDesc());
            }
            return enumDescriptions;
        }
    }
}
