using System.Text.Json.Serialization;

namespace antflowcore.vo;

public class BaseKeyValueStruVo
{
    // 字段定义（如果使用属性自动实现，可以省略字段）
    private string _key;
    private string _value;
    private string _type;
    private string _remark;
    private DateTime _createTime;
    private bool _hasStarUserChooseModule = false;

    // 无参构造函数
    public BaseKeyValueStruVo()
    {
    }

    // 全参构造函数
    public BaseKeyValueStruVo(string key, string value, string type, string remark, DateTime createTime,
        bool hasStarUserChooseModule)
    {
        _key = key;
        _value = value;
        _type = type;
        _remark = remark;
        _createTime = createTime;
        _hasStarUserChooseModule = hasStarUserChooseModule;
    }

    // 属性定义（自动实现 getter/setter）
    [JsonPropertyName("key")] // 如果需要 JSON 序列化
    public string Key
    {
        get => _key;
        set => _key = value;
    }

    [JsonPropertyName("value")]
    public string Value
    {
        get => _value;
        set => _value = value;
    }

    [JsonPropertyName("type")]
    public string Type
    {
        get => _type;
        set => _type = value;
    }

    [JsonPropertyName("remark")]
    public string Remark
    {
        get => _remark;
        set => _remark = value;
    }

    [JsonPropertyName("createTime")]
    public DateTime CreateTime
    {
        get => _createTime;
        set => _createTime = value;
    }

    /// <summary>
    /// 是否包含发起人自选模块,否为不包含,true为包含
    /// </summary>
    [JsonPropertyName("hasStarUserChooseModule")]
    public bool HasStarUserChooseModule
    {
        get => _hasStarUserChooseModule;
        set => _hasStarUserChooseModule = value;
    }
}