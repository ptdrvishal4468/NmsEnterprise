using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface ICustomerRepository : IGenericRepository<Customer, Guid>
{
    Task<Customer?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, Guid? excludeCustomerId = null, CancellationToken cancellationToken = default);
    Task<Customer?> GetWithHierarchyAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetWithContactsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Customer>> GetChildrenAsync(Guid parentCustomerId, CancellationToken cancellationToken = default);
    Task<bool> HasCircularHierarchyAsync(Guid customerId, Guid prospectiveParentId, CancellationToken cancellationToken = default);
}