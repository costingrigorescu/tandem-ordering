using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tandem.Service.OrderHandlers;

namespace Tandem.Service.UnitTests.OrderHandlers
{
  [TestFixture]
  public class BaseOrderHandlerTests
  {
    [TestCase(true, true)]
    [TestCase(false, false)]
    public void BaseOrderHandler_Enable_SetsEnabled(bool enabled, bool expected)
    {
      // Arrange
      var handler = new TestHandler();
      handler.Enable(enabled);

      // Act
      var actual = handler.Enabled;

      // Assert
      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void BaseOrderHandler_SetNext_ReturnsExpectedHandler()
    {
      // Arrange
      var handler = new TestHandler();
      var next = new TestNextHandler();

      // Act
      var actual = handler.SetNext(next);

      // Assert
      Assert.AreEqual(next, actual);
    }

    [Test]
    public async Task BaseOrderHandler_HandleAsync_WithNext_AdvancesTheChain()
    {
      // Arrange
      var handler = new TestHandler();
      var next = new Mock<IOrderHandler>();
      handler.SetNext(next.Object);

      // Act
      await handler.HandleAsync(default, CancellationToken.None);

      // Assert
      next.Verify(m => m.HandleAsync(default, CancellationToken.None), Times.Once());
    }

    [Test]
    public async Task BaseOrderHandler_HandleAsync_WithoutNext_DoesNothing()
    {
      // Arrange
      var handler = new TestHandler();

      // Act
      await handler.HandleAsync(default, CancellationToken.None);
    }

    private class TestHandler : BaseOrderHandler
    {
    }

    private class TestNextHandler : BaseOrderHandler
    {
    }
  }
}
