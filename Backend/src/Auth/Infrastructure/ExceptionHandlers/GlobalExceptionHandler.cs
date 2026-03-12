using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Auth.Infrastructure.ExceptionHandlers
{
    public class GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
           HttpContext httpContext,
           Exception exception,
           CancellationToken cancellationToken)
        {
            if (exception is OperationCanceledException or TaskCanceledException)
            {
                // Log as Information/Debug only - this isn't a server "crash"
                logger.LogInformation("Request was cancelled by the client.");

                httpContext.Response.StatusCode = 499; // Standard "Client Closed Request" code
                return true;
            }

            // 2. Filter Sensitive Exceptions (e.g., Postgres Connection Issues)
            if (exception is PostgresException pgEx && pgEx.SqlState == "28P01")
            {
                // 28P01 = Invalid Password / Auth Failure
                // LOG A GENERIC MESSAGE: Don't log the Exception object which might contain the connection string!
                logger.LogCritical("Database authentication failed. Check credentials in safe storage.");
            }
            else 
            {
                logger.LogError(
                    exception, 
                    "An unhandled exception occurred: {Message}", 
                    exception.Message);
            }

            var problemDetails = new ProblemDetails
            {
                Status = exception switch
                {
                    NpgsqlException => StatusCodes.Status503ServiceUnavailable,
                    _ => StatusCodes.Status500InternalServerError
                },
                Title = "Server Error",
                Type = "https://datatracker.ietf.org",
                Detail = "An unexpected error occurred on our end. Please try again later."
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
    }
