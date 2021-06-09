using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tandem.Model;
using Tandem.Service.Services;

namespace Tandem.Service.OrderHandlers
{
  public class NotificationHandler : BaseOrderHandler, INotificationHandler
  {
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationHandler> _logger;

    public NotificationHandler(INotificationService notificationService, ILogger<NotificationHandler> logger)
    {
      if (notificationService == null)
      {
        throw new ArgumentNullException(nameof(notificationService));
      }

      if (logger == null)
      {
        throw new ArgumentNullException(nameof(logger));
      }

      _notificationService = notificationService;
      _logger = logger;
    }

    public override async Task HandleAsync(Order order, CancellationToken ct)
    {
      if (Enabled && order.Products.Any(x => x is Membership || x is MembershipChange))
      {
        var products = order.Products.Where(x => x is Membership || x is MembershipChange).ToList();
        
        try
        {
          _logger.LogDebug($"Generating products notification for order {order.Id} client {order.ClientId} products {string.Join(",", products.Select(x => x.Id))}.");
          await _notificationService.GenerateProductsNotificationAsync(products, ct).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error while generating products notification for order {order.Id} client {order.ClientId} products {string.Join(",", products.Select(x => x.Id))}.", ex);
          throw;
        }
      }

      // advance the chain
      await base.HandleAsync(order, ct);
    }
  }
}
