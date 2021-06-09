using System.Threading;
using System.Threading.Tasks;
using Tandem.Model;

namespace Tandem.Service.OrderService
{
  public interface IOrderService
  {
    Task ProcessOrderAsync(Order order, CancellationToken ct);
  }
}
