using Quartz;
using SnowrunnerMerger.Api.Data;

namespace SnowrunnerMerger.Api.Jobs;

public class PurgeExpiredTokensJob(AppDbContext dbContext) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var expiredTokens = dbContext.UserTokens.Where(x => x.ExpiresAt < DateTime.UtcNow).ToList();
        
        dbContext.UserTokens.RemoveRange(expiredTokens);
        await dbContext.SaveChangesAsync();
    }
}