using Backend.Api;
using Backend.Application.BackgroundJobs;
using Backend.Application.Notifications;
using Backend.Common;
using Backend.Common.Interfaces;
using Backend.Common.Models;
using Backend.Features.Auth;
using Backend.Features.Tenants._Shared;
using Backend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Tenants
{
    public sealed class SeedBasicData
    {
        public sealed class SeedDataRequestInput
        {

        }
        public sealed class Route : IRouteRegistrar
        {
            public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
            {
                var tenant = endpointRouteBuilder.MapGroup(KrafterRoute.Tenants).AddFluentValidationFilter();
                tenant.MapPost("/seed-data", async
                    ([FromBody] SeedDataRequestInput requestInput, [FromServices] DataSeedService tenantSeedService) =>
                {
                    var res = await tenantSeedService.SeedBasicData(requestInput);
                    return Results.Json(res, statusCode: res.StatusCode);
                })
                .Produces<Common.Models.Response>()
                    ;
            }
        }
    }
}
