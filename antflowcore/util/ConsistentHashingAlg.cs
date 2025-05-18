using AntFlowCore.Constants;

namespace antflowcore.util;

public class ConsistentHashingAlg
{
    private readonly string[] servers;
    private readonly List<string> realNodes = new List<string>();
    private readonly SortedDictionary<int, string> virtualNodes = new SortedDictionary<int, string>();
    private readonly int VIRTUAL_NODES = 5;

    private ConsistentHashingAlg(string name, int count)
    {
        servers = new string[count];
        for (int i = 0; i < count; i++)
        {
            servers[i] = $"{name}_{i}";
            realNodes.Add(servers[i]);
        }

        foreach (var str in realNodes)
        {
            for (int i = 0; i < VIRTUAL_NODES; i++)
            {
                string virtualNodeName = $"{str}&&VN{i}";
                int hash = GetHash(virtualNodeName);
                virtualNodes[hash] = virtualNodeName;
            }
        }
    }

    /// <summary>
    /// 单例实例，使用Lazy确保线程安全且延迟加载
    /// </summary>
    private static Lazy<ConsistentHashingAlg> _mainInstance = new Lazy<ConsistentHashingAlg>(() =>
        new ConsistentHashingAlg(StringConstants.LOWFLOW_FORM_DATA_MAIN_TABLE_NAME, 1));

    private static Lazy<ConsistentHashingAlg> _fieldInstance = new Lazy<ConsistentHashingAlg>(() =>
        new ConsistentHashingAlg(StringConstants.LOWFLOW_FORM_DATA_FIELD_TABLE_NAME, 1));

    /// <summary>
    /// 获取单例实例
    /// </summary>
    public static ConsistentHashingAlg mainInstance => _mainInstance.Value;

    public static ConsistentHashingAlg fieldInstance => _fieldInstance.Value;

    public int GetHash(string str)
    {
        const uint p = 16777619;
        uint hash = 2166136261;
        foreach (var c in str)
        {
            hash = (hash ^ c) * p;
        }

        hash += hash << 13;
        hash ^= hash >> 7;
        hash += hash << 3;
        hash ^= hash >> 17;
        hash += hash << 5;

        return (int)(hash & 0x7FFFFFFF);
    }


    public string GetServer(string key)
    {
        int hash = GetHash(key);
        foreach (var pair in virtualNodes)
        {
            if (pair.Key >= hash)
            {
                return ExtractRealNode(pair.Value);
            }
        }

        if (virtualNodes.Count > 0)
        {
            return ExtractRealNode(virtualNodes.Values.GetEnumerator().Current);
        }

        return null;
    }

    private string ExtractRealNode(string virtualNode)
    {
        if (string.IsNullOrWhiteSpace(virtualNode))
            return null;

        int index = virtualNode.IndexOf("&&", StringComparison.Ordinal);
        if (index > 0)
        {
            return virtualNode.Substring(0, index);
        }

        return virtualNode;
    }
}