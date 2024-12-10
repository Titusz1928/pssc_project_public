using Lab2.Domain.Models;
using static Lab2.Domain.Models.Order;

namespace Lab2.Domain.Repositories;

public interface IOrderHeaderRepository
{
    Task AddOrderHeaderAsync(OrderHeader orderHeader);

    Task<OrderId> GetOrderIdForHeaderAsync(OrderHeader orderHeader);
}