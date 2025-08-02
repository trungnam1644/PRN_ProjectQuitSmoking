using BusinessObjects;
using DataAccessLayout;

namespace Repositories
{
    public class SmokingStatusRepository : ISmokingStatusRepository
    {
        public bool AddSmokingStatus(int userId, int cigarettesPerDay, string healthStatus, DateTime recordDate)
        {
            return SmokingStatusDAO.AddSmokingStatus(userId, cigarettesPerDay, healthStatus, recordDate);
        }

        public List<SmokingStatus> GetSmokingStatusesByUserId(int userId)
        {
            return SmokingStatusDAO.GetSmokingStatusesByUserId(userId);
        }
    }
}
