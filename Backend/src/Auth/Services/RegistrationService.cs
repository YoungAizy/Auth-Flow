using Auth.Data;
using Auth.Data.DTOs;
using Auth.Infrastructure.Mappers;
using Auth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;

namespace Auth.Services
{
    public class RegistrationService(
        AppDBContext context,
        UserMapper mapper,
        IMemoryCache cache)
    {
        public void Registration() 
        {

        }

        public async Task<ServiceResult> SaveProfile(
            RegistrationRequest userForm, CancellationToken ct) 
        {
            User user = mapper.ToEntity(userForm);

            try
            {
                context.Users.Add(user);
                await context.SaveChangesAsync(ct);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                cache.Set($"user_{user.Id}", user, cacheOptions);

            }
            catch(DbUpdateException ex) when(ex.InnerException is PostgresException { SqlState: "23505" })
            {
                return new(false,"Email is already in use.");
            }
            catch (NpgsqlException ex)
            {
                // 2. Handle DATABASE INFRASTRUCTURE error (Abrupt close, connection refused)
                // Log this error internally!
                return new(false,"The database is currently unavailable. Please try again later.", ex.Message);
            }
            catch (Exception)
            {
                // 3. Handle UNEXPECTED system error (Bug, out of memory, etc.)
                // This is usually caught by Global Exception Handling, but can be caught here for specific cleanup
                throw;
            }

            return new(true);
        }
    }
}
