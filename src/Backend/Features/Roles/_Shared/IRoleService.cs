using Backend.Common.Models;

namespace Backend.Features.Roles._Shared;

public interface IRoleService
{
    Task<Response<RoleDto>> GetByIdAsync(string id);
}