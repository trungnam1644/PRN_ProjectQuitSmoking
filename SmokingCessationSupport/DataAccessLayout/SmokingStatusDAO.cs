using BusinessObjects;

namespace DataAccessLayout
{
    public static class SmokingStatusDAO
    {
        public static bool AddSmokingStatus(int userId, int cigarettesPerDay, string healthStatus, DateTime recordDate)
        {
            using var db = new AppDbContext();
            var smokingStatus = new SmokingStatus
            {
                UserId = userId,
                CigarettesPerDay = cigarettesPerDay,
                HealthStatus = healthStatus,
                RecordDate = recordDate
            };
            db.SmokingStatuses.Add(smokingStatus);
            db.SaveChanges();
            return smokingStatus != null;
        }

        public static List<SmokingStatus> GetSmokingStatusesByUserId(int userId)
        {
            using var db = new AppDbContext();
            return db.SmokingStatuses
                     .Where(s => s.UserId == userId)
                     .OrderByDescending(s => s.RecordDate)
                     .ToList();
        }
    }
}
