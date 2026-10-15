using Microsoft.EntityFrameworkCore;
using Shared.Domain.DTOs;
using UserProfiles.Api.Domain.Interfaces;
using UserProfiles.Api.Domain.Entities;
using UserProfiles.Api.Infrastructure.Persistence;

namespace UserProfiles.Api.Infrastructure.Persistence.Repositories
{
    public class JobRepository(AppDBContext context) : IJobRepository
    {
        public async Task<Result<bool>> UpdateJobId(string userSub, string jobId)
        {
            // 1. Try to update existing record directly in DB
            int affectedRows = await context.Jobs
                .Where(j => j.UserSub == userSub)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(j => j.JobId, jobId)
                );

            // 2. If no rows were updated, insert the new record
            if (affectedRows == 0)
            {
                context.Jobs.Add(new Job { UserSub = userSub, JobId = jobId });
                await context.SaveChangesAsync();
            }

            return Result<bool>.Success(true);

        }

        public async Task<Result<bool>> DeleteJob(string userSub)
        {
            var job = await context.Jobs.FindAsync(userSub);

            if (job != null)
            {
                // 2. Mark it for deletion
                context.Jobs.Remove(job);

                // 3. Commit the change to the database
                await context.SaveChangesAsync();
            }

            return Result<bool>.Success(true);
        }

    }
}
