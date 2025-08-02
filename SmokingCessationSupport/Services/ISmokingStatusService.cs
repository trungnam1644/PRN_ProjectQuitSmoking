using BusinessObjects;

namespace Services
{
    public interface ISmokingStatusService
    {
        bool AddSmokingStatus(int userId, int cigarettesPerDay, string healthStatus, DateTime recordDate);
        List<SmokingStatus> GetSmokingStatusesByUserId(int userId);
    }
}
