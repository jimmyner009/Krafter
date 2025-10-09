using Backend.Common.Models;

namespace Backend.Features.Users._Shared;

public interface IUserService
{
    Task<Response<List<string>>> GetPermissionsAsync(string userId, CancellationToken cancellationToken);

    Task<Response<bool>> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken = default);

    Task<Response> CreateOrUpdateAsync(CreateUserRequest request);
}