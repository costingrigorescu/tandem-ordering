using System.Threading;
using System.Threading.Tasks;
using Tandem.Model;

namespace Tandem.Service.Services
{
  public interface IMembershipService
  {
    Task ActivateMembershipAsync(Membership membership, CancellationToken ct);
    Task ApplyMembershipChangeAsync(MembershipChange membershipChange, CancellationToken ct);
  }
}
