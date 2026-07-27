using Nop.Core.Domain.Gdpr;
using Nop.Services.Gdpr;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Gdpr;

[TestFixture]
public class GdprServiceTests : ServiceTest<GdprConsent>
{
    private IGdprService _gdprService;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _gdprService = GetService<IGdprService>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _gdprService → {_gdprService}", _gdprService);
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