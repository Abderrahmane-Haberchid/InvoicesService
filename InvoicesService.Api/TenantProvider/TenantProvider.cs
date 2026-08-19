using Application.Abstractions;

namespace InvoicesService.TenantProvider;

public class TenantProvider : ITenantProvider
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; }
    public void SetTenantId(Guid tenantId) => TenantId =  tenantId;
    public void SetUserId(Guid userId) =>  UserId = userId;

    public void SetUserEmail(string userEmail) =>  UserEmail = userEmail;
}