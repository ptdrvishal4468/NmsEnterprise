using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Customers.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Customers.Queries.GetCustomersPaged;

public record GetCustomersPagedQuery(
    int PageIndex = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    CustomerStatus? Status = null,
    CustomerTier? Tier = null,
    Guid? ParentCustomerId = null) : IRequest<PagedResult<CustomerDto>>;