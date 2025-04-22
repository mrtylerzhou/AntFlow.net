using antflowcore.conf.json;
using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class FormConfigWrapper
    {
        [JsonPropertyName("widgetList")]
        public List<LFWidget> WidgetList { get; set; }

        [JsonPropertyName("formConfig")]
        public FormConfig formConfig { get; set; }

        public class LFWidget
        {
            [JsonPropertyName("key")]
            public int Key { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("category")]
            public string Category { get; set; }

            [JsonPropertyName("icon")]
            public string Icon { get; set; }

            [JsonPropertyName("formItemFlag")]
            public bool FormItemFlag { get; set; }

            [JsonPropertyName("options")]
            public LFOption Options { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("tabs")]
            public List<LFWidget> Tabs { get; set; }

            [JsonPropertyName("rows")]
            public List<TableRow> Rows { get; set; }

            [JsonPropertyName("cols")]
            public List<LFWidget> Cols { get; set; }

            [JsonPropertyName("widgetList")]
            public List<LFWidget> WidgetList { get; set; }

            public class LFOption
            {
                [JsonPropertyName("name")]
                public string Name { get; set; }

                [JsonPropertyName("label")]
                public string Label { get; set; }

                [JsonPropertyName("labelAlign")]
                public string LabelAlign { get; set; }

                [JsonPropertyName("type")]
                public string Type { get; set; }

                [JsonPropertyName("fieldType"), JsonConverter(typeof(StringToIntConverter))]
                public int FieldType { get; set; }

                [JsonPropertyName("defaultValue"), JsonConverter(typeof(IntToStringConverter))]
                public string DefaultValue { get; set; }

                [JsonPropertyName("startPlaceholder")]
                public string StartPlaceholder { get; set; }

                [JsonPropertyName("endPlaceholder")]
                public string EndPlaceholder { get; set; }

                [JsonPropertyName("columnWidth")]
                public string ColumnWidth { get; set; }

                [JsonPropertyName("size")]
                public string Size { get; set; }

                [JsonPropertyName("autoFullWidth")]
                public bool? AutoFullWidth { get; set; }

                [JsonPropertyName("labelWidth")]
                public double? LabelWidth { get; set; }

                [JsonPropertyName("labelHidden")]
                public bool? LabelHidden { get; set; }

                [JsonPropertyName("readonly")]
                public bool? Readonly { get; set; }

                [JsonPropertyName("disabled")]
                public bool? Disabled { get; set; }

                [JsonPropertyName("hidden")]
                public bool? Hidden { get; set; }

                [JsonPropertyName("clearable")]
                public bool? Clearable { get; set; }

                [JsonPropertyName("editable")]
                public bool? Editable { get; set; }

                [JsonPropertyName("format")]
                public string Format { get; set; }

                [JsonPropertyName("valueFormat")]
                public string ValueFormat { get; set; }

                [JsonPropertyName("required")]
                public bool? Required { get; set; }

                [JsonPropertyName("requiredHint")]
                public string RequiredHint { get; set; }

                [JsonPropertyName("validation")]
                public string Validation { get; set; }

                [JsonPropertyName("validationHint")]
                public string ValidationHint { get; set; }

                [JsonPropertyName("customClass"), JsonConverter(typeof(StringOrArrayConverter))]
                public string[] CustomClass { get; set; }

                [JsonPropertyName("labelIconClass")]
                public object LabelIconClass { get; set; }

                [JsonPropertyName("labelIconPosition")]
                public string LabelIconPosition { get; set; }

                [JsonPropertyName("labelTooltip")]
                public string LabelTooltip { get; set; }

                [JsonPropertyName("onCreated")]
                public string OnCreated { get; set; }

                [JsonPropertyName("onMounted")]
                public string OnMounted { get; set; }

                [JsonPropertyName("onChange")]
                public string OnChange { get; set; }

                [JsonPropertyName("onFocus")]
                public string OnFocus { get; set; }

                [JsonPropertyName("onBlur")]
                public string OnBlur { get; set; }

                [JsonPropertyName("onValidate")]
                public string OnValidate { get; set; }
            }
        }

        public class TableRow
        {
            [JsonPropertyName("cols")]
            public List<LFWidget> Cols { get; set; }
        }

        public class FormConfig
        {
            [JsonPropertyName("modelName")]
            public string ModelName { get; set; }

            [JsonPropertyName("refName")]
            public string RefName { get; set; }

            [JsonPropertyName("rulesName")]
            public string RulesName { get; set; }

            [JsonPropertyName("labelWidth")]
            public int LabelWidth { get; set; }

            [JsonPropertyName("labelPosition")]
            public string LabelPosition { get; set; }

            [JsonPropertyName("size")]
            public string Size { get; set; }

            [JsonPropertyName("labelAlign")]
            public string LabelAlign { get; set; }

            [JsonPropertyName("cssCode")]
            public string CssCode { get; set; }

            [JsonPropertyName("customClass"), JsonConverter(typeof(StringOrArrayConverter))]
            public string[] CustomClass { get; set; }

            [JsonPropertyName("functions")]
            public string Functions { get; set; }

            [JsonPropertyName("layoutType")]
            public string LayoutType { get; set; }

            [JsonPropertyName("jsonVersion")]
            public int JsonVersion { get; set; }

            [JsonPropertyName("onFormCreated")]
            public string OnFormCreated { get; set; }

            [JsonPropertyName("onFormMounted")]
            public string OnFormMounted { get; set; }

            [JsonPropertyName("onFormDataChange")]
            public string OnFormDataChange { get; set; }
        }
    }
}