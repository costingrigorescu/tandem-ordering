using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tandem.Model;
using Tandem.Service.Services;

namespace Tandem.Service.OrderHandlers
{
  public class PhysicalProductHandler : BaseOrderHandler, IPhysicalProductHandler
  {
    private readonly IPackingService _packingService;
    private readonly ILogger<PhysicalProductHandler> _logger;

    public PhysicalProductHandler(IPackingService packingService, ILogger<PhysicalProductHandler> logger)
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
      if (Enabled && order.Products.Any(x => x is PhysicalProduct))
      {
        foreach (var product in order.Products.Where(x => x is PhysicalProduct).Cast<PhysicalProduct>())
        {
          try
          {
            _logger.LogDebug($"Generating shipping packing slip for order {order.Id} client {order.ClientId} product {product.Id}.");
            await _packingService.GenerateShippingPackingSlipAsync(product, ct).ConfigureAwait(false);
          }
          catch (Exception ex)
          {
            _logger.LogError($"Error while generating shipping packing slip for order {order.Id} client {order.ClientId} product {product.Id}.", ex);
            throw;
          }
        }
      }

      // advance the chain
      await base.HandleAsync(order, ct);
    }
  }
}
