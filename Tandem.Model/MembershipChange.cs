using System;
using Tandem.Model.Enum;

namespace Tandem.Model
{
  public class MembershipChange : Product
  {
    public override EnumProductType ProductType => EnumProductType.Digital;
    public Guid MembershipId { get; set; }
    public EnumMembershipType MembershipType { get; set; }
    public DateTime? Start { get; set; }
  }
}
