using MyApiRestDapperOracle.Models.Entities;

namespace MyApiRestDapperOracle.Services.Interfaces
{
    public interface IOrderItemService
    {
        Task<List<OrderItem>> GetAll();
         Task<OrderItem> GetById(int orderId,int lineItemId);
        Task<int> Add(OrderItem order);
        Task Update(OrderItem order);
        Task Delete(int orderId,int lineItemId);
    }
}