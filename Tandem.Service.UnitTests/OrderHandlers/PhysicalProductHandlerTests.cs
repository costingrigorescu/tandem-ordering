using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tandem.Model;
using Tandem.Model.Enum;
using Tandem.Service.OrderHandlers;
using Tandem.Service.Services;

namespace Tandem.Service.UnitTests.OrderHandlers
{
  [TestFixture]
  public class PhysicalProductHandlerTests
  {
    private IPhysicalProductHandler _handler;

    private Mock<IPackingService> _mockPackagingService;
    private Mock<ILogger<PhysicalProductHandler>> _mockLogger;

    private Mock<IOrderHandler> _mockNextHandler;

    [SetUp]
    public void SetUp()
    {
      _mockPackagingService = new Mock<IPackingService>();
      _mockLogger = new Mock<ILogger<PhysicalProductHandler>>();

      _mockNextHandler = new Mock<IOrderHandler>();

      _handler = new PhysicalProductHandler(_mockPackagingService.Object, _mockLogger.Object);
      _handler.Enable(true);
      _handler.SetNext(_mockNextHandler.Object);
    }

    [Test]
    public void PhysicalProductHandler_Constructor_NoPackingService_ThrowsException()
    {
      // Act + Assert
      var ex = Assert.Throws<ArgumentNullException>(() => new PhysicalProductHandler(null, _mockLogger.Object));

      Assert.IsNotNull(ex);
      Assert.AreEqual("packingService", ex.ParamName);
    }

    [Test]
    public void PhysicalProductHandler_Constructor_NoLogger_ThrowsException()
    {
      // Act + Assert
      var ex = Assert.Throws<ArgumentNullException>(() => new PhysicalProductHandler(_mockPackagingService.Object, null));

      Assert.IsNotNull(ex);
      Assert.AreEqual("logger", ex.ParamName);
    }

    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public async Task PhysicalProductHandler_HandleAsync_AdvancesTheChain(bool enabled, bool hasProducts)
    {
      // Arrange
      _handler.Enable(enabled);

      var order = new Order
      {
        Id = Guid.NewGuid(),
        ClientId = Guid.NewGuid(),
        Products = hasProducts
          ? new List<Product>
            {
              new Newspaper
              {
                Id = Guid.NewGuid()
              }
            }
          : new List<Product>()
      };

      // Act
      await _handler.HandleAsync(order, CancellationToken.None);

      // Assert
      _mockNextHandler.Verify(m => m.HandleAsync(order, CancellationToken.None), Times.Once());
    }

    [Test]
    public async Task PhysicalProductHandler_HandleAsync_NotEnabled_DoesNotCallGenerateShippingPackingSlipAsync()
    {
      // Arrange
      _handler.Enable(false);

      var order = new Order
      {
        Id = Guid.NewGuid(),
        ClientId = Guid.NewGuid(),
        Products = new List<Product>
        {
          new Newspaper
          {
            Id = Guid.NewGuid()
          }
        }
      };

      // Act
      await _handler.HandleAsync(order, CancellationToken.None);

      // Assert
      _mockPackagingService.Verify(m => m.GenerateShippingPackingSlipAsync(It.IsAny<PhysicalProduct>(), CancellationToken.None), Times.Never());
    }

    [Test]
    public async Task PhysicalProductHandler_HandleAsync_NoPhysicalProducts_DoesNotCallGenerateShippingPackingSlipAsync()
    {
      // Arrange
      _handler.Enable(true);

      var order = new Order
      {
        Id = Guid.NewGuid(),
        ClientId = Guid.NewGuid(),
        Products = new List<Product>
        {
          new Membership
          {
            Id = Guid.NewGuid(),
            MembershipType = EnumMembershipType.Bronze,
            Start = DateTime.Today
          }
        }
      };

      // Act
      await _handler.HandleAsync(order, CancellationToken.None);

      // Assert
      _mockPackagingService.Verify(m => m.GenerateShippingPackingSlipAsync(It.IsAny<PhysicalProduct>(), CancellationToken.None), Times.Never());
    }

    [Test]
    public async Task PhysicalProductHandler_HandleAsync_WithPhysicalProducts_CallsGenerateShippingPackingSlipAsync()
    {
      // Arrange
      var order = new Order
      {
        Id = Guid.NewGuid(),
        ClientId = Guid.NewGuid(),
        Products = new List<Product>
        {
          new Newspaper
          {
            Id = Guid.NewGuid()
          },
          new Membership
          {
            Id = Guid.NewGuid(),
            MembershipType = EnumMembershipType.Bronze,
            Start = DateTime.Today
          },
          new Book
          {
            Id = Guid.NewGuid()
          }
        }
      };

      // Act
      await _handler.HandleAsync(order, CancellationToken.None);

      // Assert
      _mockPackagingService.Verify(
        m =>
          m.GenerateShippingPackingSlipAsync(
            It.IsAny<PhysicalProduct>(),
            CancellationToken.None),
        Times.Exactly(2));

      _mockPackagingService.Verify(
        m =>
          m.GenerateShippingPackingSlipAsync(
            It.Is<PhysicalProduct>(
              x =>
                x is Newspaper &&
                x.Id == order.Products[0].Id),
            CancellationToken.None),
        Times.Once());

      _mockPackagingService.Verify(
        m =>
          m.GenerateShippingPackingSlipAsync(
            It.Is<PhysicalProduct>(
              x =>
                x is Book &&
                x.Id == order.Products[2].Id),
            CancellationToken.None),
        Times.Once());
    }

    [Test]
    public void PhysicalProductHandler_HandleAsync_GenerateShippingPackingSlipAsyncFails_ThrowsException()
    {
      // Arrange
      _mockPackagingService
        .Setup(m => m.GenerateShippingPackingSlipAsync(It.IsAny<Book>(), CancellationToken.None))
        .ThrowsAsync(new Exception("Exception generating packing slip."));

      var order = new Order
      {
        Id = Guid.NewGuid(),
        ClientId = Guid.NewGuid(),
        Products = new List<Product>
        {
          new Newspaper
          {
            Id = Guid.NewGuid()
          },
          new Membership
          {
            Id = Guid.NewGuid(),
            MembershipType = EnumMembershipType.Bronze,
            Start = DateTime.Today
          },
          new Book
          {
            Id = Guid.NewGuid()
          }
        }
      };

      // Act + Assert
      var ex = Assert.ThrowsAsync<Exception>(async () => await _handler.HandleAsync(order, CancellationToken.None));

      Assert.NotNull(ex);
      Assert.AreEqual("Exception generating packing slip.", ex.Message);
    }
  }
}
