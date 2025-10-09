using Backend.Api;
using Backend.Application.BackgroundJobs;
using Backend.Application.Notifications;
using Backend.Common;
using Backend.Common.Auth;
using Backend.Common.Auth.Permissions;
using Backend.Common.Interfaces;
using Backend.Common.Models;
using Backend.Features.Roles._Shared;
using Backend.Features.Tenants._Shared;
using Backend.Features.Users._Shared;
using Backend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Backend.Features.Tenants.SeedBasicData;

namespace Backend.Features.Tenants._Shared;

public class DataSeedService(
    ITenantGetterService tenantGetterService, 
    IJobService jobService, 
    KrafterContext krafterContext, 
    TenantDbContext dbContext, 
    RoleManager<KrafterRole> roleManager,
    UserManager<KrafterUser> userManager) : IScopedHandler
{
    public async Task<Response> SeedBasicData(SeedDataRequestInput requestInput)
    {
        var currentTenantResponse = tenantGetterService.Tenant;

        var roleCount = await krafterContext.Roles.CountAsync();
        if (roleCount == 0)
        {
            var role = new KrafterRole(KrafterRoleConstant.Basic, KrafterRoleConstant.Basic)
            {
                Id = Guid.NewGuid().ToString(),
            };
            var result = await roleManager.CreateAsync(role);
            var role1 = new KrafterRole(KrafterRoleConstant.Admin, KrafterRoleConstant.Admin)
            {
                Id = Guid.NewGuid().ToString(),
            };
            var result1 = await roleManager.CreateAsync(role1);
        }

        var adminRole = await roleManager.FindByNameAsync(KrafterRoleConstant.Admin);
        if (adminRole != null)
        {
            var adminClaims = await roleManager.GetClaimsAsync(adminRole);
            var adMinRolePermissions = adminClaims
                .Where(c => c.Type == KrafterClaims.Permission).Select(p => p.Value).ToList();

            var allPermissions =
                currentTenantResponse.Id == KrafterInitialConstants.RootTenant.Id
                    ? KrafterPermissions.All
                    : KrafterPermissions.Admin;

            var allPermissionsString = allPermissions.Select(krafterPermission =>
                    KrafterPermission.NameFor(krafterPermission.Action, krafterPermission.Resource))
                .ToList();
            var permissionNotWithAdmin = allPermissionsString.Except(adMinRolePermissions).ToList();
            if (permissionNotWithAdmin.Count > 0)
            {
                foreach (var permission in permissionNotWithAdmin)
                {
                    krafterContext.RoleClaims.Add(new KrafterRoleClaim
                    {
                        RoleId = adminRole.Id,
                        ClaimType = KrafterClaims.Permission,
                        ClaimValue = permission,
                    });
                }

                await krafterContext.SaveChangesAsync();
            }
        }

        var basicRole = await roleManager.FindByNameAsync(KrafterRoleConstant.Basic);
        if (basicRole != null)
        {
            var basicClaims = await roleManager.GetClaimsAsync(basicRole);
            var basicRolePermissions = basicClaims
                .Where(c => c.Type == KrafterClaims.Permission).Select(p => p.Value).ToList();

            var allBasicPermissions =
                     KrafterPermissions.Basic;

            var allPermissionsString = allBasicPermissions.Select(krafterPermission =>
                    KrafterPermission.NameFor(krafterPermission.Action, krafterPermission.Resource))
                .ToList();
            var permissionNotWithBasic = allPermissionsString.Except(basicRolePermissions).ToList();
            if (permissionNotWithBasic.Count > 0)
            {
                foreach (var permission in permissionNotWithBasic)
                {
                    krafterContext.RoleClaims.Add(new KrafterRoleClaim
                    {
                        RoleId = basicRole.Id,
                        ClaimType = KrafterClaims.Permission,
                        ClaimValue = permission,
                    });
                }
                await krafterContext.SaveChangesAsync();
            }
        }

        var userCount = await krafterContext.Users.CountAsync();
        if (userCount == 0)
        {
            string password = KrafterInitialConstants.DefaultPassword;
            KrafterUser rootUser;
            if (tenantGetterService.Tenant.Id == KrafterInitialConstants.RootTenant.Id)
            {
                rootUser = new KrafterUser
                {
                    Id = KrafterInitialConstants.RootUser.Id,
                    FirstName = KrafterInitialConstants.RootUser.FirstName,
                    LastName = KrafterInitialConstants.RootUser.LastName,
                    Email = KrafterInitialConstants.RootUser.EmailAddress,
                    UserName = KrafterInitialConstants.RootUser.EmailAddress,
                    IsActive = true,
                    IsOwner = true,
                };
            }
            else
            {
                rootUser = new KrafterUser
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "Admin",
                    LastName = "User",
                    Email = tenantGetterService.Tenant.AdminEmail,
                    UserName = tenantGetterService.Tenant.AdminEmail,
                    IsActive = true,
                    IsOwner = true,
                };
                //generate aspnet core valid password
                password = PasswordGenerator.GeneratePassword();
            }
            var res = await userManager.CreateAsync(rootUser, password);
            var tenant = await dbContext.Tenants.FirstOrDefaultAsync(c => c.Id == tenantGetterService.Tenant.Id);
            if (adminRole is not null)
            {
                krafterContext.UserRoles.Add(new KrafterUserRole()
                {
                    RoleId = adminRole.Id,
                    UserId = rootUser.Id,
                    CreatedById = rootUser.Id
                });
            }

            var basic = await roleManager.FindByNameAsync(KrafterRoleConstant.Basic);
            if (basic is not null)
            {
                krafterContext.UserRoles.Add(new KrafterUserRole()
                {
                    RoleId = basic.Id,
                    UserId = rootUser.Id,
                    CreatedById = rootUser.Id
                });
            }

            await krafterContext.SaveChangesAsync();

            if (tenant is not null)
            {
                if (tenantGetterService.Tenant.Id != KrafterInitialConstants.RootTenant.Id)
                {
                    string loginUrl = $"{tenantGetterService.Tenant.TenantLink}/login";

                    await jobService.EnqueueAsync(new SendEmailRequestInput
                    {
                        Email = tenant.AdminEmail,
                        Subject = "Welcome to Krafter",
                        HtmlMessage = $"Dear {rootUser.FirstName} {rootUser.LastName} ({rootUser.Email}),<br><br>" +
                        "Your Krafter account has been successfully created. Here are your login details:<br><br>" +
                        $"Username: {rootUser.Email}<br>" +
                        $"Password: {password}<br><br>" +
                        $"Please <a href='{loginUrl}'>click here</a> to log in.<br><br>" +
                        "We recommend changing your password after your first login for security reasons.<br><br>" +
                        "Best Regards,<br>" +
                        "The Krafter Team"
                    }, "SendEmailJob", CancellationToken.None);
                }
            }
            if (string.IsNullOrWhiteSpace(currentTenantResponse.TablesToCopy))
            {
                // currentTenantResponse.TablesToCopy = nameof(Unit);
            }

            if (!string.IsNullOrWhiteSpace(currentTenantResponse.TablesToCopy))
            {
                var tablesToCopy = currentTenantResponse.TablesToCopy?.Split(",");
                if (tablesToCopy is not null && tablesToCopy.Length > 0)
                {
                    var utcNow = DateTime.UtcNow;
                    foreach (var table in tablesToCopy)
                    {
                        // #region Global reference
                        //
                        // if (table == nameof(Unit))
                        // {
                        //     var units = await krafterContext.Units.AsNoTracking().IgnoreQueryFilters().Where(c =>
                        //             c.IsDeleted == false && c.TenantId == KrafterInitialConstants.RootTenant.Id)
                        //         .ToListAsync();
                        //
                        //     foreach (var unit in units)
                        //     {
                        //         unit.Id = Guid.NewGuid().ToString();
                        //         unit.TenantId = currentTenantResponse.Id;
                        //         unit.CreatedOn = utcNow;
                        //         unit.ChangeId = newChangeId;
                        //         unit.CreatedById = rootUser.Id;
                        //     }
                        //
                        //     await krafterContext.BulkInsertAsync(units);
                        // }
                        //
                        // if (table == nameof(Country))
                        // {
                        //     var countries = await krafterContext.Countries.AsNoTracking().IgnoreQueryFilters().Where(c =>
                        //             c.IsDeleted == false && c.TenantId == KrafterInitialConstants.RootTenant.Id)
                        //         .ToListAsync();
                        //
                        //     foreach (var country in countries)
                        //     {
                        //         country.Id = Guid.NewGuid().ToString();
                        //         country.TenantId = currentTenantResponse.Id;
                        //         country.CreatedOn = utcNow;
                        //         country.ChangeId = newChangeId;
                        //         country.CreatedById = rootUser.Id;
                        //     }
                        //     await krafterContext.BulkInsertAsync(countries);
                        // }
                        //
                        // if (table == nameof(Language))
                        // {
                        //     var languages = await krafterContext.Languages.AsNoTracking().IgnoreQueryFilters().Where(c =>
                        //             c.IsDeleted == false && c.TenantId == KrafterInitialConstants.RootTenant.Id)
                        //         .ToListAsync();
                        //
                        //     foreach (var language in languages)
                        //     {
                        //         language.Id = Guid.NewGuid().ToString();
                        //         language.TenantId = currentTenantResponse.Id;
                        //         language.CreatedOn = utcNow;
                        //         language.ChangeId = newChangeId;
                        //         language.CreatedById = rootUser.Id;
                        //     }
                        //     await krafterContext.BulkInsertAsync(languages);
                        // }
                        //
                        // if (table == nameof(Currency))
                        // {
                        //     var currencies = await krafterContext.Currencies.AsNoTracking().IgnoreQueryFilters().Where(c =>
                        //             c.IsDeleted == false && c.TenantId == KrafterInitialConstants.RootTenant.Id)
                        //         .ToListAsync();
                        //
                        //     foreach (var currency in currencies)
                        //     {
                        //         currency.Id = Guid.NewGuid().ToString();
                        //         currency.TenantId = currentTenantResponse.Id;
                        //         currency.CreatedOn = utcNow;
                        //         currency.ChangeId = newChangeId;
                        //         currency.CreatedById = rootUser.Id;
                        //     }
                        //     await krafterContext.BulkInsertAsync(currencies);
                        // }
                        //
                        // #endregion
                        // #region Document Control
                        //
                        // if (table==nameof(StorageArea))
                        // {
                        //     var storageAreas = await krafterContext.StorageAreas.AsNoTracking().IgnoreQueryFilters().Where(c =>
                        //             c.IsDeleted == false && c.TenantId == KrafterInitialConstants.RootTenant.Id)
                        //         .ToListAsync();
                        //     foreach (var documentType in storageAreas)
                        //     {
                        //         documentType.Id = Guid.NewGuid().ToString();
                        //         documentType.TenantId = currentTenantResponse.Id;
                        //         documentType.CreatedOn = utcNow;
                        //         documentType.ChangeId = newChangeId;
                        //         documentType.CreatedById = rootUser.Id;
                        //     }
                        //     await krafterContext.BulkInsertAsync(storageAreas);
                        // }
                        // if (table==nameof(DocumentType))
                        // {
                        //     var documentTypes = await krafterContext.DocumentTypes.AsNoTracking().IgnoreQueryFilters().Where(c =>
                        //             c.IsDeleted == false && c.TenantId == KrafterInitialConstants.RootTenant.Id)
                        //         .ToListAsync();
                        //
                        //     foreach (var documentType in documentTypes)
                        //     {
                        //         documentType.Id = Guid.NewGuid().ToString();
                        //         documentType.TenantId = currentTenantResponse.Id;
                        //         documentType.CreatedOn = utcNow;
                        //         documentType.ChangeId = newChangeId;
                        //         documentType.CreatedById = rootUser.Id;
                        //     }
                        //     await krafterContext.BulkInsertAsync(documentTypes);
                        // }
                        // if (table==nameof(StorageFormat))
                        // {
                        //     var storageFormats = await krafterContext.StorageFormats.AsNoTracking().IgnoreQueryFilters().Where(c =>
                        //             c.IsDeleted == false && c.TenantId == KrafterInitialConstants.RootTenant.Id)
                        //         .ToListAsync();
                        //
                        //     foreach (var storageFormat in storageFormats)
                        //     {
                        //         storageFormat.Id = Guid.NewGuid().ToString();
                        //         storageFormat.TenantId = currentTenantResponse.Id;
                        //         storageFormat.CreatedOn = utcNow;
                        //         storageFormat.ChangeId = newChangeId;
                        //         storageFormat.CreatedById = rootUser.Id;
                        //     }
                        //     await krafterContext.BulkInsertAsync(storageFormats);
                        // }
                        // if (table==nameof(Risk))
                        // {
                        //     var risks = await krafterContext.Risks.AsNoTracking().IgnoreQueryFilters().Where(c =>
                        //             c.IsDeleted == false && c.TenantId == KrafterInitialConstants.RootTenant.Id)
                        //         .ToListAsync();
                        //
                        //     foreach (var risk in risks)
                        //     {
                        //         risk.Id = Guid.NewGuid().ToString();
                        //         risk.TenantId = currentTenantResponse.Id;
                        //         risk.CreatedOn = utcNow;
                        //         risk.ChangeId = newChangeId;
                        //         risk.CreatedById = rootUser.Id;
                        //     }
                        //     await krafterContext.BulkInsertAsync(risks);
                        // }
                        // #endregion
                        // #region Knowledge
                        //
                        // if (table==nameof(TrainingRisk))
                        // {
                        //     var trainingRisks = await krafterContext.TrainingRisks.AsNoTracking().IgnoreQueryFilters().Where(c =>
                        //             c.IsDeleted == false && c.TenantId == KrafterInitialConstants.RootTenant.Id)
                        //         .ToListAsync();
                        //
                        //     foreach (var trainingRisk in trainingRisks)
                        //     {
                        //         trainingRisk.Id = Guid.NewGuid().ToString();
                        //         trainingRisk.TenantId = currentTenantResponse.Id;
                        //         trainingRisk.CreatedOn = utcNow;
                        //         trainingRisk.ChangeId = newChangeId;
                        //         trainingRisk.CreatedById = rootUser.Id;
                        //     }
                        //     await krafterContext.BulkInsertAsync(trainingRisks);
                        // }
                        // if (table==nameof(TrainingType))
                        // {
                        //     var trainingTypes = await krafterContext.TrainingTypes.AsNoTracking().IgnoreQueryFilters().Where(c =>
                        //             c.IsDeleted == false && c.TenantId == KrafterInitialConstants.RootTenant.Id)
                        //         .ToListAsync();
                        //
                        //     foreach (var trainingType in trainingTypes)
                        //     {
                        //         trainingType.Id = Guid.NewGuid().ToString();
                        //         trainingType.TenantId = currentTenantResponse.Id;
                        //         trainingType.CreatedOn = utcNow;
                        //         trainingType.ChangeId = newChangeId;
                        //         trainingType.CreatedById = rootUser.Id;
                        //     }
                        //     await krafterContext.BulkInsertAsync(trainingTypes);
                        // }
                        // if (table==nameof(EvaluationMethod))
                        // {
                        //     var evaluationMethods = await krafterContext.EvaluationMethods.AsNoTracking().IgnoreQueryFilters().Where(c =>
                        //             c.IsDeleted == false && c.TenantId == KrafterInitialConstants.RootTenant.Id)
                        //         .ToListAsync();
                        //
                        //     foreach (var trainingType in evaluationMethods)
                        //     {
                        //         trainingType.Id = Guid.NewGuid().ToString();
                        //         trainingType.TenantId = currentTenantResponse.Id;
                        //         trainingType.CreatedOn = utcNow;
                        //         trainingType.ChangeId = newChangeId;
                        //         trainingType.CreatedById = rootUser.Id;
                        //     }
                        //     await krafterContext.BulkInsertAsync(evaluationMethods);
                        // }
                        // #endregion
                        // var sql = $"INSERT INTO {table} SELECT * FROM {KrafterInitialConstants.RootTenant.Id}_{table}";
                        // await dbContext.Database.ExecuteSqlRawAsync(sql);
                    }
                }
            }
        }
        return new Response();
    }
}
