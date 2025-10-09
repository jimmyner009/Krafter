using Backend.Api;
using Backend.Api.Authorization;
using Backend.Common;
using Backend.Common.Auth;
using Backend.Common.Auth.Permissions;
using Backend.Common.Models;
using Backend.Features.Roles._Shared;
using Backend.Infrastructure.Persistence;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Roles;

public sealed class GetRoleByIdWithPermissions
{
    internal sealed class Handler(
        RoleManager<KrafterRole> roleManager,
        KrafterContext db) : IScopedHandler
    {
        public async Task<Response<RoleDto>> GetByIdWithPermissionsAsync(
            string roleId,
            CancellationToken cancellationToken)
        {
            var role = await roleManager.Roles.SingleOrDefaultAsync(x => x.Id == roleId, cancellationToken);

            if (role is null)
            {
                return new Response<RoleDto>
                {
                    IsError = true,
                    StatusCode = 404,
                    Message = "Role Not Found"
                };
            }

            var roleDto = role.Adapt<RoleDto>();

            roleDto.Permissions = await db.RoleClaims
                .Where(c => c.RoleId == roleId &&
                            c.ClaimType == KrafterClaims.Permission &&
                            c.IsDeleted == false)
                .Select(c => c.ClaimValue!)
                .ToListAsync(cancellationToken);

            return new Response<RoleDto>
            {
                Data = roleDto
            };
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var roleGroup = endpointRouteBuilder.MapGroup(KrafterRoute.Roles)
                .AddFluentValidationFilter();

            roleGroup.MapGet("/get-by-id-with-permissions/{roleId}", async (
                [FromRoute] string roleId,
                [FromServices] Handler handler,
                CancellationToken cancellationToken) =>
            {
                var res = await handler.GetByIdWithPermissionsAsync(roleId, cancellationToken);
                return Results.Json(res, statusCode: res.StatusCode);
            })
            .Produces<Common.Models.Response<RoleDto>>()
            .MustHavePermission(KrafterAction.View, KrafterResource.Roles);
        }
    }
}
