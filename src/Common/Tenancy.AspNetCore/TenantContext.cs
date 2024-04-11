using Microsoft.AspNetCore.Http;

namespace YourBrand.Tenancy;

public sealed class TenantContext(IHttpContextAccessor httpContextAccessor) : ISettableTenantContext
{
    private TenantId? _tenantId;

    /*
        public TenantId? TenantId
        {
            get
            {
                if(_tenantId is null)  
                {
                    var tenantIdStr = httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value;

                    if(tenantIdStr is not null) 
                    {
                        return (TenantId)tenantIdStr;
                    }
                }

                return _tenantId;
            }
        }
        */

    public TenantId? TenantId => _tenantId ??= (httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value ?? null)!;


    public void SetTenantId(TenantId tenantId) => _tenantId = tenantId;
}