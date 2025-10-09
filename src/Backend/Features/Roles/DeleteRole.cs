using Backend.Api;
using Backend.Api.Authorization;
using Backend.Application.Common;
using Backend.Common;
using Backend.Common.Auth;
using Backend.Common.Auth.Permissions;
using Backend.Common.Interfaces;
using Backend.Common.Models;
using Backend.Features.Roles._Shared;
using Backend.Features.Users._Shared;
using Backend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Roles;

public sealed class DeleteRole
{ 
    internal sealed class Handler(RoleManager<KrafterRole> roleManager,
        UserManager<KrafterUser> userManager,
        KrafterContext db, ITenantGetterService tenantGetterService) : IScopedHandler
    {
        public async Task<Response> DeleteAsync(DeleteRequestInput requestInput)
        {
            var role = await roleManager.FindByIdAsync(requestInput.Id);

            _ = role ?? throw new NotFoundException("Role Not Found");

            if (KrafterRoleConstant.IsDefault(role.Name!))
            {
                throw new ForbiddenException($"Not allowed to delete {role.Name} Role.");
            }

            role.IsDeleted = true;
            role.DeleteReason = requestInput.DeleteReason;
            db.Roles.Update(role);

            var krafterRoleClaims = await db.RoleClaims
                .Where(c => c.RoleId == requestInput.Id &&
                            c.ClaimType == KrafterClaims.Permission)
                .ToListAsync();
            foreach (var krafterRoleClaim in krafterRoleClaims)
            {
                krafterRoleClaim.IsDeleted = true;
            }

            await db.SaveChangesAsync([nameof(KrafterRole)]);
            return new Response();
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var roleGroup = endpointRouteBuilder.MapGroup(KrafterRoute.Roles)
                .AddFluentValidationFilter();

            roleGroup.MapPost("/delete", async
                ([FromBody] DeleteRequestInput roleRequestInput,
                    [FromServices] Handler roleService) =>
                {
                    var res = await roleService.DeleteAsync(roleRequestInput);
                    return TypedResults.Ok(res);
                })
                .Produces<Response>()

                .MustHavePermission(KrafterAction.Delete, KrafterResource.Roles);
        }
    }
}