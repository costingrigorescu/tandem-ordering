using System;
using Tandem.Model.Enum;

namespace Tandem.Model
{
  public class Membership : Product
  {
    public override EnumProductType ProductType => EnumProductType.Digital;
    public EnumMembershipType MembershipType { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
  }
}
