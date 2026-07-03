using Application.DTOs.Stock;
using Application.Interfaces.Handlers.Stock;
using Application.Interfaces.Repositories;
using Application.UseCases.Stock.Queries;
using Domain.Exceptions;

namespace Application.UseCases.Stock.Handlers
{
    public class GetDrinkStocksByDrinkIdsHandler : IGetDrinkStocksByDrinkIdsHandler
    {
        private const int MaxDrinkIds = 100;
        private readonly IStockRepository _stockRepository;

        public GetDrinkStocksByDrinkIdsHandler(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task<List<StockResponseDTO>> Handle(GetDrinkStocksByDrinkIdsQuery query)
        {
            var drinkIds = query.DrinkIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray();

            if (drinkIds.Length == 0)
                throw new ValidationException("Debe informar al menos una bebida.");

            if (drinkIds.Length > MaxDrinkIds)
                throw new ValidationException($"No se pueden consultar mas de {MaxDrinkIds} bebidas por solicitud.");

            var stocks = await _stockRepository.GetByDrinkIdsAsync(drinkIds);

            return stocks.Select(stockEntity => new StockResponseDTO
            {
                Id = stockEntity.Id,
                Count = stockEntity.Count,
                RowVersion = Convert.ToBase64String(stockEntity.RowVersion),
                Id_Drink = stockEntity.Id_Drink
            }).ToList();
        }
    }
}
