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
        BUTTON_TYPE_ZB = 21                // 转办
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
