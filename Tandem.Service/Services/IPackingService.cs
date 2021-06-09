using System.Threading;
using System.Threading.Tasks;
using Tandem.Model;

namespace Tandem.Service.Services
{
  public interface IPackingService
  {
    Task GenerateShippingPackingSlipAsync(PhysicalProduct product, CancellationToken ct);
    Task GenerateRoyaltyPackingSlipAsync(Book book, CancellationToken ct);
  }
}
