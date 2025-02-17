using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using AntFlowCore.Constants;
using antflowcore.util;
using hyjiacan.py4n;


namespace AntOffice.Base.Util
{
    /// <summary>
    /// StrUtils
    /// String utility class
    /// </summary>
    public static  class StrUtils
    {
        public const int BPMN_CODE_LEN = 5;

        // .*-(\d{5})
        public static readonly string BPMNCONF_PATTERN = @".*-(\d{" + BPMN_CODE_LEN + @"})";

        public static readonly string BPMNCONF_FORMATMARK = $"0:D{BPMN_CODE_LEN}";

        public static string GetFirstLetters(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            return GetFirstPinYin(input);
        }

        /// <summary>
        /// Joins BPMN code with a specific format.
        /// </summary>
        /// <param name="bpmncodePart">The part before the '_'</param>
        /// <param name="bpmnCode">The BPMN code</param>
        public static string JoinBpmnCode(string bpmncodePart, string bpmnCode)
        {
            int defaultNum = 1;
            string maxNumStr = string.Empty;

            if (Regex.IsMatch(bpmnCode, BPMNCONF_PATTERN))
            {
                var parts = bpmnCode.Split(new[] { StringConstants.BPMN_CODE_SPLITMARK }, StringSplitOptions.None);
                maxNumStr = parts[^1];
            }

            if (!string.IsNullOrEmpty(maxNumStr))
            {
                defaultNum = int.Parse(maxNumStr) + 1;
            }

            string stringWithRawFormat = bpmncodePart + StringConstants.BPMN_CODE_SPLITMARK +"{"+ BPMNCONF_FORMATMARK+"}";
            return string.Format(stringWithRawFormat, defaultNum);
        }

        public static string[] GetNullPropertyNames(object source)
        {
            if (source == null) return Array.Empty<string>();

            var propertyNames = TypeDescriptor.GetProperties(source)
                .Cast<PropertyDescriptor>()
                .Where(prop => ObjectUtils.IsEmpty(prop.GetValue(source)))
                .Select(prop => prop.Name)
                .ToArray();

            return propertyNames;
        }

        public static string GetBeanName(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            string simpleName = type.Name;
            return char.ToLower(simpleName[0]) + simpleName.Substring(1);
        }

        private static string GetFirstPinYin(string hanyu)
        {
            PinyinFormat format = PinyinFormat.WITHOUT_TONE | PinyinFormat.UPPERCASE | PinyinFormat.WITH_U_UNICODE;

            var firstPinyin = new System.Text.StringBuilder();
            char[] hanyuArr = hanyu.Trim().ToCharArray();

            foreach (var ch in hanyuArr)
            {
                if (Regex.IsMatch(ch.ToString(), @"[\u4E00-\u9FA5]"))
                {
                    string[] pys = Pinyin4Net.GetPinyin(ch, format);
                    firstPinyin.Append(pys[0][0]);
                }
                else
                {
                    firstPinyin.Append(ch);
                }
            }

            return firstPinyin.ToString();
        }
    }
}
