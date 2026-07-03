namespace Application.UseCases.Stock.Queries
{
    public class GetDrinkStocksByDrinkIdsQuery
    {
        public IReadOnlyCollection<Guid> DrinkIds { get; }

        public GetDrinkStocksByDrinkIdsQuery(IReadOnlyCollection<Guid> drinkIds)
        {
            DrinkIds = drinkIds;
        }
    }
}
