using Krafter.Api.Client.Models;
using Krafter.UI.Web.Client.Common.Models;

namespace Krafter.UI.Web.Client.Infrastructure.Api
{
    public interface IApiService
    {
        Task<Response<TokenResponse>> CreateTokenAsync(TokenRequestInput request, CancellationToken cancellation);

        Task<Response<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellation);

        Task<Response<TokenResponse>> ExternalAuthAsync(TokenRequestInput request, CancellationToken cancellation);

        Task<Response<TokenResponse>> GetCurrentTokenAsync(CancellationToken cancellation);

        Task LogoutAsync(CancellationToken cancellation);
    }
}