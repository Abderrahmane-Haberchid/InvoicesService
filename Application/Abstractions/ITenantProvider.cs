namespace Application.Abstractions;

public interface ITenantProvider
{
    Guid TenantId { get; set; }
    void SetTenantId(Guid tenantId);
}