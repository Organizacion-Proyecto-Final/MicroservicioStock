using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IStockRepository
    {
        Task<Stock?> GetByIdAsync(Guid id);
        Task<IEnumerable<Stock>> GetAllAsync();
        Task<(List<Stock> Items, int TotalCount, int PageNumber)> GetPageAsync(int pageNumber, int pageSize);
        Task<List<Stock>> GetByDrinkIdsAsync(IReadOnlyCollection<Guid> drinkIds);
        Task<bool> HasAssignedDishesAsync(Guid stockId);
        Task AddAsync(Stock stock);
        Task UpdateAsync(Stock stock, byte[] rowVersion);
        Task DeleteAsync(Guid id);
    }
}
