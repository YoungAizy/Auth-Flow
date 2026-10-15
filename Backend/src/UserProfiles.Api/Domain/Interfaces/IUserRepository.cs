using Shared.Domain.DTOs;
using UserProfiles.Api.Domain.DTOs;

namespace UserProfiles.Api.Domain.Interfaces
{
    public interface IUserRepository
    {
        //I want to save, get , and delete
        Task<Result<bool>> SaveUserAsync(CreateProfileRequest request, CancellationToken ct);
        Task<Result<ProfileResponse>> GetUserBySubAsync(string sub, CancellationToken ct);

        Task<Result<bool>> DeleteUserBySubAsync(string sub, CancellationToken ct);

        Task<Result<bool>> UpdateVerifiedStatus(string sub, CancellationToken ct);
    }
}
