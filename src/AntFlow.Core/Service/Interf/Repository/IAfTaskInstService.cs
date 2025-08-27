namespace AntFlow.Core.Service.Interface;

public interface IAfTaskInstService
{
    public int DoneTodayProcess(string createUserId);
    public int DoneCreateProcess(string createUserId);
}