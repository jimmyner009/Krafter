using Backend.Common.Models;
using Backend.Features.Tenants;

namespace Backend.Common.Interfaces
{
    public interface ITenantFinderService
    {
        Task<Tenant> Find(string? identifier);
    }
    public interface ITenantGetterService
    {
        public CurrentTenantDetails Tenant { get; }
    }
    public interface ITenantSetterService
    {
        void SetTenant(CurrentTenantDetails tenant);
    }

}
