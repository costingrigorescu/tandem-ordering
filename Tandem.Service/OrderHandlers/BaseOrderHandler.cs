using System.Threading;
using System.Threading.Tasks;
using Tandem.Model;

namespace Tandem.Service.OrderHandlers
{
  public abstract class BaseOrderHandler : IOrderHandler
  {
    private IOrderHandler _next;
    public bool Enabled { get; private set; }

    public void Enable(bool enabled)
    {
      Enabled = enabled;
    }

    public IOrderHandler SetNext(IOrderHandler next)
    {
      _next = next;
      return next;
    }

    public virtual async Task HandleAsync(Order order, CancellationToken ct)
    {
      if (_next != null)
      {
        await _next.HandleAsync(order, ct);
      }
    }
  }
}
