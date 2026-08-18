using Nop.Core.Domain.Gdpr;
using Nop.Services.Gdpr;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Gdpr;

[TestFixture]
public class GdprServiceTests : ServiceTest<GdprConsent>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IGdprService _gdprService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _gdprService = GetService<IGdprService>();
		_logger.LogDebug("SetUp(): _gdprService → {_gdprService}", _gdprService);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    protected override CrudData<GdprConsent> CrudData
    {
        get
        {
            var insertItem = new GdprConsent
            {
                Message = "Test message"
            };

            var updateItem = new GdprConsent
            {
                Message = "Update test message"
            };

            return new CrudData<GdprConsent>
            {
                BaseEntity = insertItem,
                UpdatedEntity = updateItem,
                Insert = _gdprService.InsertConsentAsync,
                Update = _gdprService.UpdateConsentAsync,
                GetById = _gdprService.GetConsentByIdAsync,
                IsEqual = (item, other) => item.Message.Equals(other.Message),
                Delete = _gdprService.DeleteConsentAsync
            };
        }
    }
}