using Tandem.Model.Enum;

namespace Tandem.Model
{
  public abstract class Product : BaseModel
  {
    public abstract EnumProductType ProductType { get; }
  }
}
