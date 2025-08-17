namespace antflowcore.service.interf;

public interface IAfTaskInstService
{
    public int DoneTodayProcess(String createUserId);
    public int DoneCreateProcess(String createUserId);
}