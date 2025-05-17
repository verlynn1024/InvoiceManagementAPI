using System;
using System.Threading;
using System.Threading.Tasks;
using InvoiceManagementAPI.Application.Common.Behaviours;
using InvoiceManagementAPI.Application.Common.Interfaces;
using InvoiceManagementAPI.Application.Invoices.Commands.CreateInvoice;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace InvoiceManagementAPI.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
    private Mock<ILogger<CreateInvoiceCommand>> _logger = null!;
    private Mock<IUser> _user = null!;
    private Mock<IIdentityService> _identityService = null!;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<CreateInvoiceCommand>>();
        _user = new Mock<IUser>();
        _identityService = new Mock<IIdentityService>();
    }

    [Test]
    public async Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
    {
        _user.Setup(x => x.Id).Returns(Guid.NewGuid().ToString());

        var requestLogger = new LoggingBehaviour<CreateInvoiceCommand>(_logger.Object, _user.Object, _identityService.Object);

        await requestLogger.Process(new CreateInvoiceCommand(), new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
        var requestLogger = new LoggingBehaviour<CreateInvoiceCommand>(_logger.Object, _user.Object, _identityService.Object);

        await requestLogger.Process(new CreateInvoiceCommand(), new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Never);
    }
}
