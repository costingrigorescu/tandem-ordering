using NUnit.Framework;
using Tandem.Model.Enum;

namespace Tandem.Model.UnitTests
{
  [TestFixture]
  public class MembershipChangeTests
  {
    [Test]
    public void MembershipChange_ProductType_ReturnsExpectedValue()
    {
      // Arrange
      var model = new MembershipChange();

      // Act
      var productType = model.ProductType;

      // Arrange
      Assert.AreEqual(EnumProductType.Digital, productType);
    }
  }
}
