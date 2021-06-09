using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tandem.Model;
using Tandem.Service.Services;

namespace Tandem.Service.OrderHandlers
{
  public class BookHandler : BaseOrderHandler, IBookHandler
  {
    private readonly IPackingService _packingService;
    private readonly ILogger<BookHandler> _logger;

    public BookHandler(IPackingService packingService, ILogger<BookHandler> logger)
    {
      if (packingService == null)
      {
        throw new ArgumentNullException(nameof(packingService));
      }

      if (logger == null)
      {
        throw new ArgumentNullException(nameof(logger));
      }

      _packingService = packingService;
      _logger = logger;
    }

    public override async Task HandleAsync(Order order, CancellationToken ct)
    {
      if (Enabled && order.Products.Any(x => x is Book))
      {
        foreach (var book in order.Products.Where(x => x is Book).Cast<Book>())
        {
          try
          {
            _logger.LogDebug($"Generating royalty packing slip for order {order.Id} client {order.ClientId} book {book.Id}.");
            await _packingService.GenerateRoyaltyPackingSlipAsync(book, ct).ConfigureAwait(false);
          }
          catch (Exception ex)
          {
            _logger.LogError($"Error while generating royalty packing slip for order {order.Id} client {order.ClientId} book {book.Id}.", ex);
            throw;
          }
        }
      }

      // advance the chain
      await base.HandleAsync(order, ct);
    }
  }
}
