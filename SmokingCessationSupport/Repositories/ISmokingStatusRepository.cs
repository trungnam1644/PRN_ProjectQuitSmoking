using BusinessObjects;

namespace Repositories
{
    public interface ISmokingStatusRepository
    {
        bool AddSmokingStatus(int userId, int cigarettesPerDay, string healthStatus, DateTime recordDate);
        List<SmokingStatus> GetSmokingStatusesByUserId(int userId);
    }
}
