using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tandem.Model;
using Tandem.Service.Config;
using Tandem.Service.OrderHandlers;

namespace Tandem.Service.OrderService
{
  public class OrderService : IOrderService
  {
    private readonly IOrderHandler _handler;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOptionsSnapshot<OrderServiceConfig> config, IServiceProvider serviceProvider, ILogger<OrderService> logger)
    {
      if (config == null || config.Value == null)
      {
        throw new ArgumentNullException(nameof(config));
      }

      if (serviceProvider == null)
      {
        throw new ArgumentNullException(nameof(serviceProvider));
      }

      if (logger == null)
      {
        throw new ArgumentNullException(nameof(logger));
      }

      _logger = logger;

      // setup chain
      var physicalProductHandler = serviceProvider.GetRequiredService<IPhysicalProductHandler>();
      var bookHandler = serviceProvider.GetRequiredService<IBookHandler>();
      var membershipHandler = serviceProvider.GetRequiredService<IMembershipHandler>();
      var membershipChangeHandler = serviceProvider.GetRequiredService<IMembershipChangeHandler>();
      var notificationHandler = serviceProvider.GetRequiredService<INotificationHandler>();

      physicalProductHandler.Enable(config.Value.GenerateShippingPackingSlip);
      bookHandler.Enable(config.Value.GenerateRoyaltyPackingSlip);
      membershipHandler.Enable(config.Value.ActivateMembership);
      membershipChangeHandler.Enable(config.Value.ApplyMembershipChange);
      notificationHandler.Enable(config.Value.NotifyClient);

      physicalProductHandler
        .SetNext(bookHandler)
        .SetNext(membershipHandler)
        .SetNext(membershipChangeHandler)
        .SetNext(notificationHandler);

      _handler = physicalProductHandler;
    }

    public async Task ProcessOrderAsync(Order order, CancellationToken ct)
    {
      if (order == null)
      {
        throw new ArgumentNullException(nameof(order));
      }

      if (order.Products == null || order.Products.Count == 0)
      {
        throw new ArgumentException("Missing products.");
      }

      try
      {
        _logger.LogDebug($"Handling order {order.Id}.");
        await _handler.HandleAsync(order, ct).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error while handling order {order.Id}.", ex);
        throw;
      }
    }
  }
}
