using System.Text.RegularExpressions;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.util;

public class InformationTemplateUtils
    {
        

        /// <summary>
        /// 翻译信息模板
        /// </summary>
        /// <param name="informationTemplateVo">输入模板</param>
        /// <returns>翻译后的模板 VO</returns>
        public static InformationTemplateVo TranslateInformationTemplate(InformationTemplateVo informationTemplateVo)
        {
            IInformationTemplateService informationTemplateService = ServiceProviderUtils.GetService<IInformationTemplateService>();
            InformationTemplate informationTemplate = informationTemplateService.baseRepo
                .Where(a => a.Id == informationTemplateVo.Id).ToOne() ?? new InformationTemplate();

            return new InformationTemplateVo
            {
                SystemTitle = Translate(informationTemplate.SystemTitle, informationTemplateVo.WildcardCharacterMap),
                SystemContent = Translate(informationTemplate.SystemContent, informationTemplateVo.WildcardCharacterMap),
                MailTitle = Translate(informationTemplate.SystemTitle, informationTemplateVo.WildcardCharacterMap),
                MailContent = Translate(informationTemplate.SystemContent, informationTemplateVo.WildcardCharacterMap),
                NoteContent = Translate(informationTemplate.NoteContent, informationTemplateVo.WildcardCharacterMap),
                JumpUrl = informationTemplate.JumpUrl
            };
        }

        private static string Translate(string info, Dictionary<int, string> map)
        {
            if (string.IsNullOrWhiteSpace(info))
            {
                return info;
            }

            foreach (var wildcard in WildcardCharacterEnum.Values)
            {
                string pattern =wildcard.TransfDesc;
                string replacement = map.TryGetValue(wildcard.Code, out var value) && !string.IsNullOrWhiteSpace(value)
                    ? value
                    : "";

                info = Regex.Replace(info, pattern, replacement);
            }

            return info;
        }
    }