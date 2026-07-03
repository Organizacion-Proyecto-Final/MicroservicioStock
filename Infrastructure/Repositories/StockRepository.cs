using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly AppDbContext _context;

        public StockRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Stock?> GetByIdAsync(Guid id)
        {
            return await _context.Stock
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Stock>> GetAllAsync()
        {
            return await _context.Stock
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<(List<Stock> Items, int TotalCount, int PageNumber)> GetPageAsync(int pageNumber, int pageSize)
        {
            var query = _context.Stock
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages > 0 && pageNumber > totalPages)
                pageNumber = totalPages;

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount, pageNumber);
        }

        public async Task<List<Stock>> GetByDrinkIdsAsync(IReadOnlyCollection<Guid> drinkIds)
        {
            return await _context.Stock
                .AsNoTracking()
                .Where(stock => stock.Id_Drink.HasValue && drinkIds.Contains(stock.Id_Drink.Value))
                .ToListAsync();
        }

        public async Task<bool> HasAssignedDishesAsync(Guid stockId)
        {
            return await _context.IngredientDish
                .AsNoTracking()
                .AnyAsync(ingredientDish => ingredientDish.Ingredient.Id_Stock == stockId);
        }

        public async Task AddAsync(Stock stock)
        {
            await _context.Stock.AddAsync(stock);
        }

        public Task UpdateAsync(Stock stock, byte[] rowVersion)
        {
            _context.Entry(stock)
                .Property(s => s.RowVersion)
                .OriginalValue = rowVersion;

            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var stock = await _context.Stock.FindAsync(id);

            if (stock != null)
            {
                _context.Stock.Remove(stock);
            }
        }
    }
}
