using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Widgets.FacebookPixel.Domain;
using Nop.Plugin.Widgets.FacebookPixel.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Widgets.FacebookPixel.Infrastructure.Mapper;

/// <summary>
/// Represents AutoMapper configuration for plugin models
/// </summary>
public class MapperConfiguration : Profile, IOrderedMapperProfile
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public MapperConfiguration()
    {
		_logger.LogError(">>> Entering MapperConfiguration()");
        CreateMap<FacebookPixelConfiguration, FacebookPixelModel>()
            .ForMember(model => model.AvailableStores, options => options.Ignore())
            .ForMember(model => model.CustomEventSearchModel, options => options.Ignore())
            .ForMember(model => model.CustomProperties, options => options.Ignore())
            .ForMember(model => model.HideCustomEventsSearch, options => options.Ignore())
            .ForMember(model => model.HideStoresList, options => options.Ignore())
            .ForMember(model => model.StoreName, options => options.Ignore());
        CreateMap<FacebookPixelModel, FacebookPixelConfiguration>()
            .ForMember(entity => entity.CustomEvents, options => options.Ignore());
			_logger.LogWarning("<<< Exiting MapperConfiguration()");
    }

    #endregion

    #region Properties

    /// <summary>
    /// Order of this mapper implementation
    /// </summary>
    public int Order => 1;

    #endregion
}