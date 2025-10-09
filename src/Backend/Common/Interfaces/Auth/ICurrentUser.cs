using System.Security.Claims;

namespace Backend.Common.Interfaces.Auth
{
    public interface ICurrentUser
    {
        string? Name { get; }

        string GetUserId();

        string? GetUserEmail();

        // string? GetTenant();

        bool IsAuthenticated();

        bool IsInRole(string role);

        IEnumerable<Claim>? GetUserClaims();
    }
    public interface ICurrentUserInitializer
    {
        void SetCurrentUser(ClaimsPrincipal user);

        void SetCurrentUserId(string userId);
    }

}
