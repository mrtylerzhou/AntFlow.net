using System.Text.Json.Serialization;

namespace AntFlowCore.Base.entity.jsonconf;

public class VariableConfigJson
{
    [JsonPropertyName("buttons")]
    public List<VariableButtonItem> Buttons { get; set; } = new();

    [JsonPropertyName("messages")]
    public List<VariableMessageItem> Messages { get; set; } = new();

    [JsonPropertyName("signUps")]
    public List<VariableSignUpItem> SignUps { get; set; } = new();

    [JsonPropertyName("approveReminds")]
    public List<VariableApproveRemindItem> ApproveReminds { get; set; } = new();
}

public class VariableButtonItem
{
    [JsonPropertyName("elementId")]
    public string? ElementId { get; set; }

    [JsonPropertyName("buttonPageType")]
    public int? ButtonPageType { get; set; }

    [JsonPropertyName("viewType")]
    public int? ViewType { get; set; }

    [JsonPropertyName("buttonType")]
    public int? ButtonType { get; set; }

    [JsonPropertyName("buttonName")]
    public string? ButtonName { get; set; }
}

public class VariableMessageItem
{
    [JsonPropertyName("elementId")]
    public string? ElementId { get; set; }

    [JsonPropertyName("messageType")]
    public int? MessageType { get; set; }

    [JsonPropertyName("eventType")]
    public int? EventType { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

public class VariableSignUpItem
{
    [JsonPropertyName("elementId")]
    public string? ElementId { get; set; }

    [JsonPropertyName("nodeId")]
    public string? NodeId { get; set; }

    [JsonPropertyName("afterSignUpWay")]
    public int? AfterSignUpWay { get; set; }

    [JsonPropertyName("subElements")]
    public string? SubElements { get; set; }

    [JsonPropertyName("personnelByElement")]
    public Dictionary<string, List<VariablePersonnelItem>> PersonnelByElement { get; set; } = new();
}

public class VariablePersonnelItem
{
    [JsonPropertyName("assignee")]
    public string? Assignee { get; set; }

    [JsonPropertyName("assigneeName")]
    public string? AssigneeName { get; set; }
}

public class VariableApproveRemindItem
{
    [JsonPropertyName("elementId")]
    public string? ElementId { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
