using Backend.Common.Models;
using Backend.Features.Users._Shared;

namespace Backend.Features.Auth.Token;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokensAndUpdateUser(string userId, string ipAddress);
    Task<TokenResponse> GenerateTokensAndUpdateUser(KrafterUser user, string ipAddress);
}