using NUnit.Framework;
using Tandem.Model.Enum;

namespace Tandem.Model.UnitTests
{
  [TestFixture]
  public class NewspaperTests
  {
    [Test]
    public void Newspaper_ProductType_ReturnsExpectedValue()
    {
      // Arrange
      var model = new Newspaper();

      // Act
      var productType = model.ProductType;

      // Arrange
      Assert.AreEqual(EnumProductType.Physical, productType);
    }
  }
}
