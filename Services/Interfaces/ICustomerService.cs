using MyApiRestDapperOracle.Models.Entities;

namespace MyApiRestDapperOracle.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer> GetById(int id);
        Task<List<Customer>> GetAll();
        Task<int> Add(Customer customer);
        Task Update(Customer customer);
        Task Delete(int id);
    }
}