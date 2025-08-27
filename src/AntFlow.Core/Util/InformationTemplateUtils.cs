using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Vo;
using System.Text.RegularExpressions;

namespace AntFlow.Core.Util;

public class InformationTemplateUtils
{
    /// <summary>
    ///     转换信息模板
    /// </summary>
    /// <param name="informationTemplateVo">信息模板VO</param>
    /// <returns>转换后的信息 VO</returns>
    public static InformationTemplateVo TranslateInformationTemplate(InformationTemplateVo informationTemplateVo)
    {
        InformationTemplateService informationTemplateService =
            ServiceProviderUtils.GetService<InformationTemplateService>();
        InformationTemplate informationTemplate = informationTemplateService.baseRepo
            .Where(a => a.Id == informationTemplateVo.Id).ToOne() ?? new InformationTemplate();

        return new InformationTemplateVo
        {
            SystemTitle = Translate(informationTemplate.SystemTitle, informationTemplateVo.WildcardCharacterMap),
            SystemContent =
                Translate(informationTemplate.SystemContent, informationTemplateVo.WildcardCharacterMap),
            MailTitle = Translate(informationTemplate.MailTitle, informationTemplateVo.WildcardCharacterMap),
            MailContent = Translate(informationTemplate.MailContent, informationTemplateVo.WildcardCharacterMap),
            NoteContent = Translate(informationTemplate.NoteContent, informationTemplateVo.WildcardCharacterMap),
            JumpUrl = informationTemplate.JumpUrl
        };
    }

    private static string Translate(string info, Dictionary<int, string> map)
    {
        if (string.IsNullOrWhiteSpace(info))
        {
            return "";
        }

        foreach (WildcardCharacterEnum? wildcard in WildcardCharacterEnum.Values)
        {
            string pattern = Regex.Escape(wildcard.TransfDesc) + @"\(" + wildcard.Code + @"\)";
            string replacement = map.TryGetValue(wildcard.Code, out string? value) && !string.IsNullOrWhiteSpace(value)
                ? value
                : "";

            info = Regex.Replace(info, pattern, replacement);
        }

        return info;
    }
}