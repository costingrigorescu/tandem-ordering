using Tandem.Model.Enum;

namespace Tandem.Model
{
  public class Book : Product
  {
    public override EnumProductType ProductType => EnumProductType.Physical;
  }
}
