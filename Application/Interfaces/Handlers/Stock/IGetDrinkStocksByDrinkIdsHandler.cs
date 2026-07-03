using Application.DTOs.Stock;
using Application.UseCases.Stock.Queries;

namespace Application.Interfaces.Handlers.Stock
{
    public interface IGetDrinkStocksByDrinkIdsHandler
    {
        Task<List<StockResponseDTO>> Handle(GetDrinkStocksByDrinkIdsQuery query);
    }
}
