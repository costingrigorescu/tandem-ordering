using Tandem.Model.Enum;

namespace Tandem.Model
{
  public class Newspaper : Product
  {
    public override EnumProductType ProductType => EnumProductType.Physical;
  }
}
