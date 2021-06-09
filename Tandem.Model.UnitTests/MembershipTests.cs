using NUnit.Framework;
using Tandem.Model.Enum;

namespace Tandem.Model.UnitTests
{
  [TestFixture]
  public class MembershipTests
  {
    [Test]
    public void Membership_ProductType_ReturnsExpectedValue()
    {
      // Arrange
      var model = new Membership();

      // Act
      var productType = model.ProductType;

      // Arrange
      Assert.AreEqual(EnumProductType.Digital, productType);
    }
  }
}
