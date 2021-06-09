using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tandem.Model;
using Tandem.Model.Enum;
using Tandem.Service.Config;
using Tandem.Service.OrderHandlers;
using Tandem.Service.OrderService;

namespace Tandem.Service.UnitTests.OrderService
{
  [TestFixture]
  public class OrderServiceTests
  {
    private IOrderService _service;

    private Mock<IOptionsSnapshot<OrderServiceConfig>> _mockConfig;
    private Mock<IServiceProvider> _mockServiceProvider;
    private Mock<ILogger<Service.OrderService.OrderService>> _mockLogger;

    private Mock<IPhysicalProductHandler> _mockPhysicalProductHandler;
    private Mock<IBookHandler> _mockBookHandler;
    private Mock<IMembershipHandler> _mockMembershipHandler;
    private Mock<IMembershipChangeHandler> _mockMembershipChangeHandler;
    private Mock<INotificationHandler> _mockNotificationHandler;

    [SetUp]
    public void SetUp()
    {
      _mockConfig = new Mock<IOptionsSnapshot<OrderServiceConfig>>();
      _mockConfig
        .SetupGet(x => x.Value)
        .Returns(new OrderServiceConfig
        {
          GenerateShippingPackingSlip = true,
          GenerateRoyaltyPackingSlip = true,
          ActivateMembership = true,
          ApplyMembershipChange = true,
          NotifyClient = true
        });

      _mockPhysicalProductHandler = new Mock<IPhysicalProductHandler>();
      _mockBookHandler = new Mock<IBookHandler>();
      _mockMembershipHandler = new Mock<IMembershipHandler>();
      _mockMembershipChangeHandler = new Mock<IMembershipChangeHandler>();
      _mockNotificationHandler = new Mock<INotificationHandler>();

      _mockPhysicalProductHandler
        .Setup(m => m.SetNext(It.IsAny<IBookHandler>()))
        .Returns<IBookHandler>(x => x);

      _mockBookHandler
        .Setup(m => m.SetNext(It.IsAny<IMembershipHandler>()))
        .Returns<IMembershipHandler>(x => x);

      _mockMembershipHandler
        .Setup(m => m.SetNext(It.IsAny<IMembershipChangeHandler>()))
        .Returns<IMembershipChangeHandler>(x => x);

      _mockMembershipChangeHandler
        .Setup(m => m.SetNext(It.IsAny<INotificationHandler>()))
        .Returns<INotificationHandler>(x => x);

      _mockNotificationHandler
        .Setup(m => m.SetNext(It.IsAny<IOrderHandler>()))
        .Throws(new Exception("Invalid chain configuration."));

      _mockServiceProvider = new Mock<IServiceProvider>();
      _mockServiceProvider
        .Setup(x => x.GetService(typeof(IPhysicalProductHandler)))
        .Returns(_mockPhysicalProductHandler.Object);
      _mockServiceProvider
        .Setup(x => x.GetService(typeof(IBookHandler)))
        .Returns(_mockBookHandler.Object);
      _mockServiceProvider
        .Setup(x => x.GetService(typeof(IMembershipHandler)))
        .Returns(_mockMembershipHandler.Object);
      _mockServiceProvider
        .Setup(x => x.GetService(typeof(IMembershipChangeHandler)))
        .Returns(_mockMembershipChangeHandler.Object);
      _mockServiceProvider
        .Setup(x => x.GetService(typeof(INotificationHandler)))
        .Returns(_mockNotificationHandler.Object);

      _mockLogger = new Mock<ILogger<Service.OrderService.OrderService>>();

      _service = new Service.OrderService.OrderService(_mockConfig.Object, _mockServiceProvider.Object, _mockLogger.Object);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void OrderService_Constructor_NoOrderServiceConfig_ThrowsException(bool nullValue)
    {
      // Arrange
      var mockConfig = new Mock<IOptionsSnapshot<OrderServiceConfig>>();
      mockConfig
        .SetupGet(x => x.Value)
        .Returns(default(OrderServiceConfig));

      // Act + Assert
      var ex = Assert.Throws<ArgumentNullException>(() => new Service.OrderService.OrderService(nullValue ? null : mockConfig.Object, _mockServiceProvider.Object, _mockLogger.Object));

      Assert.IsNotNull(ex);
      Assert.AreEqual("config", ex.ParamName);
    }

    [Test]
    public void OrderService_Constructor_NoServiceProvider_ThrowsException()
    {
      // Act + Assert
      var ex = Assert.Throws<ArgumentNullException>(() => new Service.OrderService.OrderService(_mockConfig.Object, null, _mockLogger.Object));

      Assert.IsNotNull(ex);
      Assert.AreEqual("serviceProvider", ex.ParamName);
    }

    [Test]
    public void OrderService_Constructor_NoLogger_ThrowsException()
    {
      // Act + Assert
      var ex = Assert.Throws<ArgumentNullException>(() => new Service.OrderService.OrderService(_mockConfig.Object, _mockServiceProvider.Object, null));

      Assert.IsNotNull(ex);
      Assert.AreEqual("logger", ex.ParamName);
    }

    [Test]
    public void OrderService_Constructor_ConfiguresChain()
    {
      // Arrange
      _mockPhysicalProductHandler = new Mock<IPhysicalProductHandler>();
      _mockBookHandler = new Mock<IBookHandler>();
      _mockMembershipHandler = new Mock<IMembershipHandler>();
      _mockMembershipChangeHandler = new Mock<IMembershipChangeHandler>();
      _mockNotificationHandler = new Mock<INotificationHandler>();

      _mockPhysicalProductHandler
        .Setup(m => m.SetNext(It.IsAny<IBookHandler>()))
        .Returns<IBookHandler>(x => x);

      _mockBookHandler
        .Setup(m => m.SetNext(It.IsAny<IMembershipHandler>()))
        .Returns<IMembershipHandler>(x => x);

      _mockMembershipHandler
        .Setup(m => m.SetNext(It.IsAny<IMembershipChangeHandler>()))
        .Returns<IMembershipChangeHandler>(x => x);

      _mockMembershipChangeHandler
        .Setup(m => m.SetNext(It.IsAny<INotificationHandler>()))
        .Returns<INotificationHandler>(x => x);

      _mockNotificationHandler
        .Setup(m => m.SetNext(It.IsAny<IOrderHandler>()))
        .Throws(new Exception("Invalid chain configuration."));

      _mockServiceProvider = new Mock<IServiceProvider>();
      _mockServiceProvider
        .Setup(x => x.GetService(typeof(IPhysicalProductHandler)))
        .Returns(_mockPhysicalProductHandler.Object);
      _mockServiceProvider
        .Setup(x => x.GetService(typeof(IBookHandler)))
        .Returns(_mockBookHandler.Object);
      _mockServiceProvider
        .Setup(x => x.GetService(typeof(IMembershipHandler)))
        .Returns(_mockMembershipHandler.Object);
      _mockServiceProvider
        .Setup(x => x.GetService(typeof(IMembershipChangeHandler)))
        .Returns(_mockMembershipChangeHandler.Object);
      _mockServiceProvider
        .Setup(x => x.GetService(typeof(INotificationHandler)))
        .Returns(_mockNotificationHandler.Object);

      // Act
      var service = new Service.OrderService.OrderService(_mockConfig.Object, _mockServiceProvider.Object, _mockLogger.Object);

      // Assert
      _mockServiceProvider.Verify(m => m.GetService(typeof(IPhysicalProductHandler)), Times.Once());
      _mockServiceProvider.Verify(m => m.GetService(typeof(IBookHandler)), Times.Once());
      _mockServiceProvider.Verify(m => m.GetService(typeof(IMembershipHandler)), Times.Once());
      _mockServiceProvider.Verify(m => m.GetService(typeof(IMembershipChangeHandler)), Times.Once());
      _mockServiceProvider.Verify(m => m.GetService(typeof(INotificationHandler)), Times.Once());

      _mockPhysicalProductHandler.Verify(m => m.Enable(true), Times.Once());
      _mockBookHandler.Verify(m => m.Enable(true), Times.Once());
      _mockMembershipHandler.Verify(m => m.Enable(true), Times.Once());
      _mockMembershipChangeHandler.Verify(m => m.Enable(true), Times.Once());
      _mockNotificationHandler.Verify(m => m.Enable(true), Times.Once());

      _mockPhysicalProductHandler.Verify(m => m.SetNext(_mockBookHandler.Object), Times.Once());
      _mockBookHandler.Verify(m => m.SetNext(_mockMembershipHandler.Object), Times.Once());
      _mockMembershipHandler.Verify(m => m.SetNext(_mockMembershipChangeHandler.Object), Times.Once());
      _mockMembershipChangeHandler.Verify(m => m.SetNext(_mockNotificationHandler.Object), Times.Once());
      _mockNotificationHandler.Verify(m => m.SetNext(It.IsAny<IOrderHandler>()), Times.Never());
    }

    [TestCase(true, false, false, false, false)]
    [TestCase(false, true, false, false, false)]
    [TestCase(false, false, true, false, false)]
    [TestCase(false, false, false, true, false)]
    [TestCase(false, false, false, false, true)]
    public void OrderService_Constructor_EnablesHandlers(
      bool generateShippingPackingSlip,
      bool generateRoyaltyPackingSlip,
      bool activateMembership,
      bool applyMembershipChange,
      bool notifyClient)
    {
      // Arrange
      _mockConfig = new Mock<IOptionsSnapshot<OrderServiceConfig>>();
      _mockConfig
        .SetupGet(x => x.Value)
        .Returns(new OrderServiceConfig
        {
          GenerateShippingPackingSlip = generateShippingPackingSlip,
          GenerateRoyaltyPackingSlip = generateRoyaltyPackingSlip,
          ActivateMembership = activateMembership,
          ApplyMembershipChange = applyMembershipChange,
          NotifyClient = notifyClient
        });

      _mockPhysicalProductHandler = new Mock<IPhysicalProductHandler>();
      _mockBookHandler = new Mock<IBookHandler>();
      _mockMembershipHandler = new Mock<IMembershipHandler>();
      _mockMembershipChangeHandler = new Mock<IMembershipChangeHandler>();
      _mockNotificationHandler = new Mock<INotificationHandler>();

      _mockPhysicalProductHandler
        .Setup(m => m.SetNext(It.IsAny<IBookHandler>()))
        .Returns<IBookHandler>(x => x);

      _mockBookHandler
        .Setup(m => m.SetNext(It.IsAny<IMembershipHandler>()))
        .Returns<IMembershipHandler>(x => x);

      _mockMembershipHandler
        .Setup(m => m.SetNext(It.IsAny<IMembershipChangeHandler>()))
        .Returns<IMembershipChangeHandler>(x => x);

      _mockMembershipChangeHandler
        .Setup(m => m.SetNext(It.IsAny<INotificationHandler>()))
        .Returns<INotificationHandler>(x => x);

      _mockNotificationHandler
        .Setup(m => m.SetNext(It.IsAny<IOrderHandler>()))
        .Throws(new Exception("Invalid chain configuration."));

      _mockServiceProvider = new Mock<IServiceProvider>();
      _mockServiceProvider
        .Setup(x => x.GetService(typeof(IPhysicalProductHandler)))
        .Returns(_mockPhysicalProductHandler.Object);
      _mockServiceProvider
        .Setup(x => x.GetService(typeof(IBookHandler)))
        .Returns(_mockBookHandler.Object);
      _mockServiceProvider
        .Setup(x => x.GetService(typeof(IMembershipHandler)))
        .Returns(_mockMembershipHandler.Object);
      _mockServiceProvider
        .Setup(x => x.GetService(typeof(IMembershipChangeHandler)))
        .Returns(_mockMembershipChangeHandler.Object);
      _mockServiceProvider
        .Setup(x => x.GetService(typeof(INotificationHandler)))
        .Returns(_mockNotificationHandler.Object);

      // Act
      var service = new Service.OrderService.OrderService(_mockConfig.Object, _mockServiceProvider.Object, _mockLogger.Object);

      // Assert
      _mockPhysicalProductHandler.Verify(m => m.Enable(generateShippingPackingSlip), Times.Once());
      _mockBookHandler.Verify(m => m.Enable(generateRoyaltyPackingSlip), Times.Once());
      _mockMembershipHandler.Verify(m => m.Enable(activateMembership), Times.Once());
      _mockMembershipChangeHandler.Verify(m => m.Enable(applyMembershipChange), Times.Once());
      _mockNotificationHandler.Verify(m => m.Enable(notifyClient), Times.Once());
    }

    [Test]
    public void OrderService_ProcessOrderAsync_NoOrder_ThrowsException()
    {
      // Act + Assert
      var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.ProcessOrderAsync(null, CancellationToken.None));

      Assert.IsNotNull(ex);
      Assert.AreEqual("order", ex.ParamName);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void OrderService_ProcessOrderAsync_NoOrderProducts_ThrowsException(bool nullProducts)
    {
      // Arrange
      var order = new Order
      {
        Id = Guid.NewGuid(),
        ClientId = Guid.NewGuid(),
        Products = nullProducts ? default : new List<Product>()
      };

      // Act + Assert
      var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _service.ProcessOrderAsync(order, CancellationToken.None));

      Assert.IsNotNull(ex);
      Assert.AreEqual("Missing products.", ex.Message);
    }

    [Test]
    public async Task OrderService_ProcessOrderAsync_CallsHandleAsync_WithExpectedOrder()
    {
      // Arrange
      var order = new Order
      {
        Id = Guid.NewGuid(),
        ClientId = Guid.NewGuid(),
        Products = new List<Product>
        {
          new Book { Id = Guid.NewGuid() },
          new Membership { Id = Guid.NewGuid(), MembershipType = EnumMembershipType.Gold, Start = DateTime.Today }
        }
      };

      // Act
      await _service.ProcessOrderAsync(order, CancellationToken.None);

      // Assert
      _mockPhysicalProductHandler
        .Verify(
          m => m.HandleAsync(
            It.Is<Order>(
              o =>
                o.Id == order.Id &&
                o.ClientId == order.ClientId &&
                o.Products != null &&
                o.Products.Count == 2 &&
                o.Products.Count(x => x is Book) == 1 &&
                o.Products.Count(x => x is Membership) == 1 &&
                (o.Products.Single(x => x is Book) as Book).Id == order.Products[0].Id &&
                (o.Products.Single(x => x is Book) as Book).ProductType == order.Products[0].ProductType &&
                (o.Products.Single(x => x is Membership) as Membership).Id == order.Products[1].Id &&
                (o.Products.Single(x => x is Membership) as Membership).ProductType == order.Products[1].ProductType &&
                (o.Products.Single(x => x is Membership) as Membership).MembershipType == (order.Products[1] as Membership).MembershipType &&
                (o.Products.Single(x => x is Membership) as Membership).Start == (order.Products[1] as Membership).Start &&
                (o.Products.Single(x => x is Membership) as Membership).End == (order.Products[1] as Membership).End),
            CancellationToken.None),
          Times.Once()
        );
    }

    [Test]
    public void OrderService_ProcessOrderAsync_HandleAsyncFails_ThrowsException()
    {
      // Arrange
      _mockPhysicalProductHandler
        .Setup(m => m.HandleAsync(It.IsAny<Order>(), CancellationToken.None))
        .ThrowsAsync(new Exception("Chain exception."));

      var order = new Order
      {
        Id = Guid.NewGuid(),
        ClientId = Guid.NewGuid(),
        Products = new List<Product>
        {
          new Book { Id = Guid.NewGuid() },
          new Membership { Id = Guid.NewGuid(), MembershipType = EnumMembershipType.Gold, Start = DateTime.Today }
        }
      };

      // Act + Assert
      var ex = Assert.ThrowsAsync<Exception>(async () => await _service.ProcessOrderAsync(order, CancellationToken.None));

      Assert.IsNotNull(ex);
      Assert.AreEqual("Chain exception.", ex.Message);
    }
  }
}
