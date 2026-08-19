namespace Application.Abstractions;

public interface ITenantProvider
{
    Guid TenantId { get;}
    Guid UserId { get; }
    string UserEmail { get; }
    void SetTenantId(Guid tenantId);
    void SetUserId(Guid userId);
    void SetUserEmail(string userEmail);
}