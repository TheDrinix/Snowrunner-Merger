using System.Net;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SnowrunnerMerger.Api.Data;
using SnowrunnerMerger.Api.Exceptions;
using SnowrunnerMerger.Api.Models.Auth;
using SnowrunnerMerger.Api.Models.Auth.Dtos;
using SnowrunnerMerger.Api.Services.Interfaces;

namespace SnowrunnerMerger.Api.Services;

/// <summary>
/// Service for managing user-related operations.
/// </summary>
public class UserService(
    IHttpContextAccessor httpContextAccessor,
    ILogger<UserService> logger,
    IAuthService authService,
    AppDbContext dbContext
    ) : IUserService
{
    /// <summary>
    /// Retrieves the current user's session data from the JWT.
    /// </summary>
    /// <returns>JwtData containing user session information.</returns>
    /// <exception cref="HttpResponseException">Thrown with an HTTP status code of HttpStatusCode.Unauthorized (401) when user session data is not found.</exception>
    public JwtData GetUserSessionData()
    {
        var principal = httpContextAccessor.HttpContext?.User;
        
        var id = principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = principal?.FindFirstValue(ClaimTypes.Role);

        if (id is null || role is null)
        {
            logger.LogError("User session data not found");
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        
        return new JwtData(Guid.Parse(id), role);
    }

    /// <summary>
    /// Retrieves the current user from the database.
    /// </summary>
    /// <returns>The current user.</returns>
    /// <exception cref="HttpResponseException">Thrown with an HTTP status code of HttpStatusCode.Unauthorized (401) when the user session data is not found.</exception>
    public async Task<User> GetCurrentUser()
    {
        var userData = GetUserSessionData();
        
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userData.Id);
        
        if (user is null)
        {
            logger.LogError("User not found");
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        
        return user;
    }

    /// <summary>
    /// Updates the username of the current user.
    /// </summary>
    /// <param name="username">The new username.</param>
    /// <returns>The updated user.</returns>
    public async Task<User> UpdateUsername(string username)
    {
        var user = await GetCurrentUser();
        
        user.Username = username;
        user.NormalizedUsername = username.ToUpper();
        
        dbContext.Update(user);
        await dbContext.SaveChangesAsync();

        return user;
    }

    /// <summary>
    ///     Updates the password of the current user.
    /// </summary>
    /// <param name="data">A <see cref="UpdatePasswordDto"/> object containing the user's current password and new password.</param>
    /// <returns>The updated user.</returns>
    public async Task<User> UpdatePassword(UpdatePasswordDto data)
    {
        var user = await GetCurrentUser();

        user = await authService.UpdatePassword(user, data);

        return user;
    }

    /// <summary>
    /// Deletes the current user from the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteUser()
    {
        await authService.Logout();
        
        var user = await GetCurrentUser();
        
        dbContext.Users.Remove(user);
        
        await dbContext.SaveChangesAsync();
    }
}