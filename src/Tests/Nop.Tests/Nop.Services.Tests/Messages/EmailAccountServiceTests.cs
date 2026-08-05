using Nop.Core.Domain.Messages;
using Nop.Services.Messages;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Messages;

[TestFixture]
public class EmailAccountServiceTests : ServiceTest<EmailAccount>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IEmailAccountService _emailAccountService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _emailAccountService = GetService<IEmailAccountService>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _emailAccountService → {_emailAccountService}", _emailAccountService);
    }

    protected override CrudData<EmailAccount> CrudData
    {
        get
        {
            var insertItem = new EmailAccount
            {
                Email = "test@test.com",
                DisplayName = "Test name",
                Host = "smtp.test.com",
                Port = 25,
                Username = "test_user",
                Password = "test_password",
                EnableSsl = false
            };

            var updateItem = new EmailAccount
            {
                Email = "test@test.com",
                DisplayName = "Test name",
                Host = "smtp.test.com",
                Port = 430,
                Username = "test_user",
                Password = "test_password",
                EnableSsl = true
            };

            return new CrudData<EmailAccount>
            {
                BaseEntity = insertItem,
                UpdatedEntity = updateItem,
                Insert = _emailAccountService.InsertEmailAccountAsync,
                Update = _emailAccountService.UpdateEmailAccountAsync,
                GetById = _emailAccountService.GetEmailAccountByIdAsync,
                IsEqual = (item, other) => item.Port.Equals(other.Port) && item.EnableSsl.Equals(other.EnableSsl),
                Delete = _emailAccountService.DeleteEmailAccountAsync
            };
        }
    }

}