namespace Application.Abstractions;

public interface ITenantProvider
{
    Guid TenantId { get;}
    void SetTenantId(Guid tenantId);
}