using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tandem.Model;

namespace Tandem.Service.Services
{
  public interface INotificationService
  {
    Task GenerateProductsNotificationAsync(List<Product> products, CancellationToken ct);
  }
}
