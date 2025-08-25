using Quartz;
using SnowrunnerMerger.Api.Data;

namespace SnowrunnerMerger.Api.Jobs;

public class PurgeExpiredSessionsJob(AppDbContext dbContext) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var expiredSessions = dbContext.UserSessions.Where(x => x.ExpiresAt < DateTime.UtcNow).ToList();
        
        dbContext.UserSessions.RemoveRange(expiredSessions);
        await dbContext.SaveChangesAsync();
    }
}