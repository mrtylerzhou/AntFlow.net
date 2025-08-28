namespace AntFlow.Core.Entity;

/// <summary>
///     Generic employee DTO, ���ڱ�ʾ��¼Ա����ͨ����Ϣ
/// </summary>
public class GenericEmployee
{
    /// <summary>
    ///     �û� ID
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    ///     �û���
    /// </summary>
    public string Username { get; set; } = "";

    /// <summary>
    ///     ��ʵ����
    /// </summary>
    public string GivenName { get; set; } = "";

    /// <summary>
    ///     ����
    /// </summary>
    public string JobNum { get; set; }

    /// <summary>
    ///     ��λ����
    /// </summary>
    public string JobName { get; set; }

    /// <summary>
    ///     ��λ�ȼ�����
    /// </summary>
    public string JobLevelName { get; set; }

    /// <summary>
    ///     ��Ƭ·��
    /// </summary>
    public string PhotoPath { get; set; }

    /// <summary>
    ///     ͷ��·��
    /// </summary>
    public string HeadImg { get; set; }

    /// <summary>
    ///     ����
    /// </summary>
    public string Mail { get; set; }

    /// <summary>
    ///     �ֻ���
    /// </summary>
    public string Mobile { get; set; }

    /// <summary>
    ///     �Ƿ�Ϊϵͳ����Ա
    /// </summary>
    public bool? IsMaster { get; set; }

    /// <summary>
    ///     ������˾ ID
    /// </summary>
    public long? CompanyId { get; set; }

    /// <summary>
    ///     ֱ���ϼ�
    /// </summary>
    public GenericEmployee DirectLeader { get; set; }

    /// <summary>
    ///     ӵ�е�Ȩ����
    /// </summary>
    public HashSet<string> Permissions { get; set; } = new() { "3060101" };

    /// <summary>
    ///     �㱨·��
    /// </summary>
    public string Path { get; set; }
}