using NUnit.Framework;
using Tandem.Model.Enum;

namespace Tandem.Model.UnitTests
{
  [TestFixture]
  public class BookTests
  {
    [Test]
    public void Book_ProductType_ReturnsExpectedValue()
    {
      // Arrange
      var model = new Book();

      // Act
      var productType = model.ProductType;

      // Arrange
      Assert.AreEqual(EnumProductType.Physical, productType);
    }
  }
}
