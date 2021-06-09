using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tandem.Model;
using Tandem.Service.Services;

namespace Tandem.Service.OrderHandlers
{
  public class MembershipHandler : BaseOrderHandler, IMembershipHandler
  {
    private readonly IMembershipService _membershipService;
    private readonly ILogger<MembershipHandler> _logger;

    public MembershipHandler(IMembershipService membershipService, ILogger<MembershipHandler> logger)
    {
      if (membershipService == null)
      {
        throw new ArgumentNullException(nameof(membershipService));
      }

      if (logger == null)
      {
        throw new ArgumentNullException(nameof(logger));
      }

      _membershipService = membershipService;
      _logger = logger;
    }

    public override async Task HandleAsync(Order order, CancellationToken ct)
    {
      if (Enabled && order.Products.Any(x => x is Membership))
      {
        foreach (var membership in order.Products.Where(x => x is Membership).Cast<Membership>())
        {
          try
          {
            _logger.LogDebug($"Activating membership for order {order.Id} client {order.ClientId}.");
            await _membershipService.ActivateMembershipAsync(membership, ct).ConfigureAwait(false);
          }
          catch (Exception ex)
          {
            _logger.LogError($"Error while activating membership for order {order.Id} client {order.ClientId}.", ex);
            throw;
          }
        }
      }

      // advance the chain
      await base.HandleAsync(order, ct);
    }
  }
}
