using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tandem.Model;
using Tandem.Service.Services;

namespace Tandem.Service.OrderHandlers
{
  public class MembershipChangeHandler : BaseOrderHandler, IMembershipChangeHandler
  {
    private readonly IMembershipService _membershipService;
    private readonly ILogger<MembershipChangeHandler> _logger;

    public MembershipChangeHandler(IMembershipService membershipService, ILogger<MembershipChangeHandler> logger)
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
      if (Enabled && order.Products.Any(x => x is MembershipChange))
      {
        foreach (var membershipChange in order.Products.Where(x => x is MembershipChange).Cast<MembershipChange>())
        {
          try
          {
            _logger.LogDebug($"Applying membership change for order {order.Id} client {order.ClientId} membership {membershipChange.MembershipId}.");
            await _membershipService.ApplyMembershipChangeAsync(membershipChange, ct).ConfigureAwait(false);
          }
          catch (Exception ex)
          {
            _logger.LogError($"Error while applying membership change for order {order.Id} client {order.ClientId} membership {membershipChange.MembershipId}.", ex);
            throw;
          }
        }
      }

      // advance the chain
      await base.HandleAsync(order, ct);
    }
  }
}
