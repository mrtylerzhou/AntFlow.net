using System.Text.RegularExpressions;
using antflowcore.constant.enus;
using AntFlowCore.Entity;
using antflowcore.service.repository;
using AntFlowCore.Vo;

namespace antflowcore.util;

public class InformationTemplateUtils
    {
        

        /// <summary>
        /// 翻译信息模板
        /// </summary>
        /// <param name="informationTemplateVo">输入模板</param>
        /// <returns>翻译后的模板 VO</returns>
        public static InformationTemplateVo TranslateInformationTemplate(InformationTemplateVo informationTemplateVo)
        {
            InformationTemplateService informationTemplateService = ServiceProviderUtils.GetService<InformationTemplateService>();
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