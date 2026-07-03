using Application.DTOs;
using Application.DTOs.Stock;
using Application.Interfaces.Handlers.Stock;
using Application.UseCases.Stock.Commands;
using Application.UseCases.Stock.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicioStock.Controllers
{
    [ApiController]
    [Route("api/v1/stocks")]
    public class StockController : ControllerBase
    {
        private readonly ICreateStockHandler _createStockHandler;
        private readonly IUpdateStockHandler _updateStockHandler;
        private readonly IDeleteStockHandler _deleteStockHandler;
        private readonly IGetAllStockHandler _getAllStockHandler;
        private readonly IGetByIdStockHandler _getByIdStockHandler;
        private readonly IGetDrinkStocksByDrinkIdsHandler _getDrinkStocksByDrinkIdsHandler;
        private readonly IConsumeStockForOrderHandler _consumeStockForOrderHandler;
        private readonly IReleaseStockForOrderHandler _releaseStockForOrderHandler;

        public StockController(
            ICreateStockHandler createStockHandler,
            IUpdateStockHandler updateStockHandler,
            IDeleteStockHandler deleteStockHandler,
            IGetAllStockHandler getAllStockHandler,
            IGetByIdStockHandler getByIdStockHandler,
            IGetDrinkStocksByDrinkIdsHandler getDrinkStocksByDrinkIdsHandler,
            IConsumeStockForOrderHandler consumeStockForOrderHandler,
            IReleaseStockForOrderHandler releaseStockForOrderHandler)
        {
            _createStockHandler = createStockHandler;
            _updateStockHandler = updateStockHandler;
            _deleteStockHandler = deleteStockHandler;
            _getAllStockHandler = getAllStockHandler;
            _getByIdStockHandler = getByIdStockHandler;
            _getDrinkStocksByDrinkIdsHandler = getDrinkStocksByDrinkIdsHandler;
            _consumeStockForOrderHandler = consumeStockForOrderHandler;
            _releaseStockForOrderHandler = releaseStockForOrderHandler;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PagedResponseDTO<StockResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var stocks = await _getAllStockHandler.Handle(
                new GetAllStockQuery(pageNumber, pageSize));

            return Ok(stocks);
        }

        [HttpGet("drinks")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<StockResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByDrinkIds([FromQuery] Guid[] drinkIds)
        {
            var stocks = await _getDrinkStocksByDrinkIdsHandler.Handle(
                new GetDrinkStocksByDrinkIdsQuery(drinkIds));

            return Ok(stocks);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(StockResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var stock = await _getByIdStockHandler.Handle(
                new GetByIdStockQuery(id));

            return Ok(stock);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(
            [FromBody] CreateStockCommand command)
        {
            await _createStockHandler.Handle(command);

            return Created(string.Empty, null);
        }

        [HttpPost("consumptions")]
        [Authorize(Roles = "Admin,Waitress")]
        [ProducesResponseType(typeof(StockOperationResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ConsumeForOrder(
            [FromBody] ConsumeStockForOrderCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _consumeStockForOrderHandler.Handle(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost("releases")]
        [Authorize(Roles = "Admin,Waitress,Kitchen")]
        [ProducesResponseType(typeof(StockOperationResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReleaseForOrder(
            [FromBody] ReleaseStockForOrderCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _releaseStockForOrderHandler.Handle(command, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] StockRequestDTO dto)
        {
            var command = new UpdateStockCommand(dto.Count, dto.RowVersion);

            await _updateStockHandler.Handle(id, command);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _deleteStockHandler.Handle(
                new DeleteStockCommand(id));

            return NoContent();
        }
    }
}
