using FluentAssertions;
using Nop.Core.Domain.Security;
using Nop.Services.Configuration;
using Nop.Services.Security;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Security;

[TestFixture]
public class EncryptionServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IEncryptionService _encryptionService;
    private SecuritySettings _securitySettings;
    private ISettingService _settingService;
    private string _defaultEncryptionKey;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _securitySettings = GetService<SecuritySettings>();
        _settingService = GetService<ISettingService>();
		_logger.LogDebug("SetUp(): _securitySettings → {_securitySettings}", _securitySettings);
		_logger.LogDebug("SetUp(): _settingService → {_settingService}", _settingService);
		_logger.LogDebug("SetUp(): _securitySettings.EncryptionKey → {Arg0}", _securitySettings.EncryptionKey);

        _defaultEncryptionKey = _securitySettings.EncryptionKey;
		_logger.LogDebug("SetUp(): _defaultEncryptionKey → {_defaultEncryptionKey}", _defaultEncryptionKey);
        _securitySettings.EncryptionKey = "273ece6f97dd844d";
        await _settingService.SaveSettingAsync(_securitySettings);

        _encryptionService = GetService<IEncryptionService>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _encryptionService → {_encryptionService}", _encryptionService);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        _securitySettings.EncryptionKey = _defaultEncryptionKey;
        await _settingService.SaveSettingAsync(_securitySettings);
		_logger.LogDebug("<<< Exiting TearDown()");
		_logger.LogDebug("TearDown(): _securitySettings.EncryptionKey → {Arg0}", _securitySettings.EncryptionKey);
    }

    [Test]
    public void CanHashSha1()
    {
		_logger.LogDebug(">>> Entering CanHashSha1()");
        var password = "MyLittleSecret";
        var saltKey = "salt1";
		_logger.LogWarning("CanHashSha1(): password → {Password}", password);
		_logger.LogDebug("CanHashSha1(): saltKey → {SaltKey}", saltKey);
        var hashedPassword = _encryptionService.CreatePasswordHash(password, saltKey, "SHA1");
        hashedPassword.Should().Be("A07A9638CCE93E48E3F26B37EF7BDF979B8124D6");
		_logger.LogDebug("<<< Exiting CanHashSha1()");
		_logger.LogDebug("CanHashSha1(): hashedPassword → {HashedPassword}", hashedPassword);
    }

    [Test]
    public void CanHashSha512()
    {
		_logger.LogDebug(">>> Entering CanHashSha512()");
        var password = "MyLittleSecret";
        var saltKey = "salt1";
		_logger.LogWarning("CanHashSha512(): password → {Password}", password);
		_logger.LogDebug("CanHashSha512(): saltKey → {SaltKey}", saltKey);
        var hashedPassword = _encryptionService.CreatePasswordHash(password, saltKey, "SHA512");
        hashedPassword.Should().Be("4506D65FDB6F3A8CF97278AB7C5C62DEC35EADD474BE1E6243776691D56E1B27F71C1D9085B26BD7513BED89822204D6B8FCBD6E665D46558C48F56D21B2A293");
		_logger.LogDebug("<<< Exiting CanHashSha512()");
		_logger.LogDebug("CanHashSha512(): hashedPassword → {HashedPassword}", hashedPassword);
    }

    [Test]
    public void CanEncryptAndDecrypt()
    {
		_logger.LogDebug(">>> Entering CanEncryptAndDecrypt()");
        var password = "MyLittleSecret";
        var encryptedPassword = _encryptionService.EncryptText(password);
		_logger.LogWarning("CanEncryptAndDecrypt(): password → {Password}", password);
		_logger.LogDebug("CanEncryptAndDecrypt(): encryptedPassword → {EncryptedPassword}", encryptedPassword);
        var decryptedPassword = _encryptionService.DecryptText(encryptedPassword);
        decryptedPassword.Should().Be(password);
		_logger.LogDebug("<<< Exiting CanEncryptAndDecrypt()");
		_logger.LogDebug("CanEncryptAndDecrypt(): decryptedPassword → {DecryptedPassword}", decryptedPassword);
    }
}