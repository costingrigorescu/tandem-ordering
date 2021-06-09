using System.Threading;
using System.Threading.Tasks;
using Tandem.Model;

namespace Tandem.Service.OrderHandlers
{
  public interface IOrderHandler
  {
    void Enable(bool enabled);
    IOrderHandler SetNext(IOrderHandler next);
    Task HandleAsync(Order order, CancellationToken ct);
  }

  public interface IPhysicalProductHandler : IOrderHandler { }

  public interface IBookHandler : IOrderHandler { }

  public interface IMembershipHandler : IOrderHandler { }

  public interface IMembershipChangeHandler : IOrderHandler { }

  public interface INotificationHandler : IOrderHandler { }
}
