using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface ICustomerContactRepository : IGenericRepository<CustomerContact, Guid>
{
    Task<IReadOnlyList<CustomerContact>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<CustomerContact?> GetPrimaryContactAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsInCustomerAsync(Guid customerId, string email, Guid? excludeContactId = null, CancellationToken cancellationToken = default);
}