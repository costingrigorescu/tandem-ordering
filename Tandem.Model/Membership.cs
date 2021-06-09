using System;
using Tandem.Model.Enum;

namespace Tandem.Model
{
  public class Membership : DigitalProduct
  {
    public EnumMembershipType MembershipType { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
  }
}
