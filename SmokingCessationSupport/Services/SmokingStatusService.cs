using BusinessObjects;
using Repositories;

namespace Services
{
    public class SmokingStatusService : ISmokingStatusService
    {
        private readonly ISmokingStatusRepository iSmokingStatusRepository;

        public SmokingStatusService()
        {
            iSmokingStatusRepository = new SmokingStatusRepository();
        }

        public bool AddSmokingStatus(int userId, int cigarettesPerDay, string healthStatus, DateTime recordDate)
        {
            return iSmokingStatusRepository.AddSmokingStatus(userId, cigarettesPerDay, healthStatus, recordDate);
        }

        public List<SmokingStatus> GetSmokingStatusesByUserId(int userId)
        {
            return iSmokingStatusRepository.GetSmokingStatusesByUserId(userId);
        }
    }
}
