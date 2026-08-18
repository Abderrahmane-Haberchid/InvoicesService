using Application.Abstractions;

namespace InvoicesService.TenantProvider;

public class TenantProvider : ITenantProvider
{
    public Guid TenantId { get; set; }
    public void SetTenantId(Guid tenantId) => TenantId =  tenantId;
}