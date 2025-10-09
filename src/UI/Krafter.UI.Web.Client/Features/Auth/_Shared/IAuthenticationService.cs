using Krafter.Api.Client.Models;

namespace Krafter.UI.Web.Client.Features.Auth._Shared;

public interface IAuthenticationService
{
    event Action<string?>? LoginChange;

    Task<bool> LoginAsync(TokenRequestInput model);

    Task LogoutAsync(string methodName);

    Task<bool> RefreshAsync();
}