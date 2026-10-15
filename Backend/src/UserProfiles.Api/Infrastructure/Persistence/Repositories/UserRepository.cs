using Microsoft.EntityFrameworkCore;
using Npgsql;
using UserProfiles.Api.Domain.DTOs;
using UserProfiles.Api.Domain.Interfaces;
using UserProfiles.Api.Domain.Entities;
using UserProfiles.Api.Infrastructure.Mappers;
using Shared.Domain.DTOs;
using UserProfiles.Api.Infrastructure.Persistence;

namespace UserProfiles.Api.Infrastructure.Persistence.Repositories
{
    public class UserRepository(AppDBContext context, UserMapper mapper) : IUserRepository
    {
        public async Task<Result<bool>> SaveUserAsync(CreateProfileRequest request, CancellationToken ct)
        {
            User user = mapper.ToEntity(request);
            //user.Sub = userSub;

            try
            {
                context.Users.Add(user);
                await context.SaveChangesAsync(ct);
                return Result<bool>.Success(true);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
            {
                return Result<bool>.Failure("Email is already in use.", "Email");
            }
        }

        public async Task<Result<ProfileResponse>> GetUserBySubAsync(string sub, CancellationToken ct)
        {
            // Use AsNoTracking for read-only queries to reduce change-tracking overhead.
            var userProfile = await context.Users
                .Where(u => u.Sub == sub)
                .Select(u => new ProfileResponse(u.FirstName, u.LastName, u.Email, u.IsVerified))
                .FirstOrDefaultAsync();

            return userProfile is not null ?
                Result<ProfileResponse>.Success(userProfile) :
                Result<ProfileResponse>.Failure("User not found");
        }

        public async Task<Result<bool>> DeleteUserBySubAsync(string sub, CancellationToken ct)
        {
            try 
            { 
                // Find the user (tracked so we can remove)
                var user = await context.Users.SingleOrDefaultAsync(u => u.Sub == sub, ct);
                if (user is null)
                {
                    //logger.LogInformation("No user found for sub {Sub}", sub);
                    return Result<bool>.Failure("No user found with the corresponding information.");
                }

                // Remove from DB
                context.Users.Remove(user);
                await context.SaveChangesAsync(ct);

                return Result<bool>.Success(user.IsVerified);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result<bool>.Failure("The user was already deleted by another process.");
            }
        }

        public async Task<Result<bool>> UpdateVerifiedStatus(string sub, CancellationToken ct) 
        {
            var result = await context.Users
                .Where(u => u.Sub == sub)
                .ExecuteUpdateAsync(updates => updates
                    .SetProperty(u => u.IsVerified, true), ct);

            return result > 0 ?
                Result<bool>.Success(true) :
                Result<bool>.Failure("User not found or already verified.");
        }
    }
}
