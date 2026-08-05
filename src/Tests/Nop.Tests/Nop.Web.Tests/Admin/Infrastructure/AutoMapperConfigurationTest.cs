using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Web.Areas.Admin.Infrastructure.Mapper;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Admin.Infrastructure;

[TestFixture]
public class AutoMapperConfigurationTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    [Test]
    public void ConfigurationIsValid()
    {
		_logger.LogDebug(">>> Entering ConfigurationIsValid()");
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(typeof(AdminMapperConfiguration));
        });

        AutoMapperConfiguration.Init(config);
        AutoMapperConfiguration.MapperConfiguration.AssertConfigurationIsValid();
		_logger.LogDebug("<<< Exiting ConfigurationIsValid()");
    }
}