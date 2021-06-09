using Tandem.Model.Enum;

namespace Tandem.Model
{
  public abstract class DigitalProduct : Product
  {
    public override EnumProductType ProductType => EnumProductType.Digital;
  }
}
