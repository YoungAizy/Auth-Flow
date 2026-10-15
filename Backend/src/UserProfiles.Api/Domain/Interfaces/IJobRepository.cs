using Shared.Domain.DTOs;

namespace UserProfiles.Api.Domain.Interfaces
{
    public interface IJobRepository
    {
        //Task<Result<bool>> NewJob(string userSub, string jobId);

        Task<Result<bool>> UpdateJobId(string userSub, string jobId);

        Task<Result<bool>> DeleteJob(string userSub);
    }
}
