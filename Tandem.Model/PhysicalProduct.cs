using Tandem.Model.Enum;

namespace Tandem.Model
{
  public abstract class PhysicalProduct : Product
  {
    public override EnumProductType ProductType => EnumProductType.Physical;
  }
}
