using Microsoft.Extensions.Logging;
﻿using AutoMapper;

namespace Nop.Core.Infrastructure.Mapper;

/// <summary>
/// AutoMapper configuration
/// </summary>
public static class AutoMapperConfiguration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Mapper
    /// </summary>
    public static IMapper Mapper { get; private set; }

    /// <summary>
    /// Mapper configuration
    /// </summary>
    public static MapperConfiguration MapperConfiguration { get; private set; }

    /// <summary>
    /// Initialize mapper
    /// </summary>
    /// <param name="config">Mapper configuration</param>
    public static void Init(MapperConfiguration config)
    {
		_logger.LogDebug(">>> Entering Init(config={Config})", config);
        MapperConfiguration = config;
        Mapper = config.CreateMapper();
		_logger.LogDebug("<<< Exiting Init(config={Config})", config);
    }
}
_logger.LogDebug("Init(config={Config}): Mapper → {Mapper}", config, Mapper);