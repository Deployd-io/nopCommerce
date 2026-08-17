using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Http;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Extensions;
using Nop.Web.Models.Common;
using Nop.Web.Models.Profile;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the profile model factory
/// </summary>
public partial class ProfileModelFactory : IProfileModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CustomerSettings _customerSettings;
    protected readonly ForumSettings _forumSettings;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IForumService _forumService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPictureService _pictureService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;

    #endregion

    #region Ctor

    public ProfileModelFactory(CustomerSettings customerSettings,
        ForumSettings forumSettings,
        ICountryService countryService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IForumService forumService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        IPictureService pictureService,
        IWorkContext workContext,
        MediaSettings mediaSettings)
    {
		_logger.LogDebug(">>> Entering ProfileModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings})", customerSettings, forumSettings, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings);
        _customerSettings = customerSettings;
		_logger.LogDebug("ProfileModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings}): _customerSettings → {_customerSettings}", customerSettings, forumSettings, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, _customerSettings);
        _forumSettings = forumSettings;
		_logger.LogDebug("ProfileModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings}): _forumSettings → {_forumSettings}", customerSettings, forumSettings, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, _forumSettings);
        _countryService = countryService;
		_logger.LogDebug("ProfileModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings}): _countryService → {_countryService}", customerSettings, forumSettings, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, _countryService);
        _customerService = customerService;
		_logger.LogDebug("ProfileModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings}): _customerService → {_customerService}", customerSettings, forumSettings, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, _customerService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("ProfileModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings}): _dateTimeHelper → {_dateTimeHelper}", customerSettings, forumSettings, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, _dateTimeHelper);
        _forumService = forumService;
		_logger.LogDebug("ProfileModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings}): _forumService → {_forumService}", customerSettings, forumSettings, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, _forumService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("ProfileModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings}): _genericAttributeService → {_genericAttributeService}", customerSettings, forumSettings, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, _genericAttributeService);
        _localizationService = localizationService;
		_logger.LogDebug("ProfileModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings}): _localizationService → {_localizationService}", customerSettings, forumSettings, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, _localizationService);
        _pictureService = pictureService;
		_logger.LogDebug("ProfileModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings}): _pictureService → {_pictureService}", customerSettings, forumSettings, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, _pictureService);
        _workContext = workContext;
		_logger.LogDebug("ProfileModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings}): _workContext → {_workContext}", customerSettings, forumSettings, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, _workContext);
        _mediaSettings = mediaSettings;
		_logger.LogDebug("<<< Exiting ProfileModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings})", customerSettings, forumSettings, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings);
		_logger.LogDebug("ProfileModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings}): _mediaSettings → {_mediaSettings}", customerSettings, forumSettings, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, _mediaSettings);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the profile index model
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="page">Number of posts page; pass null to disable paging</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the profile index model
    /// </returns>
    public virtual async Task<ProfileIndexModel> PrepareProfileIndexModelAsync(Customer customer, int? page)
    {
		_logger.LogDebug(">>> Entering PrepareProfileIndexModelAsync(customer={Customer},page={Page})", customer, page);
        ArgumentNullException.ThrowIfNull(customer);

        var pagingPosts = false;
		_logger.LogDebug("PrepareProfileIndexModelAsync(customer={Customer},page={Page}): pagingPosts → {PagingPosts}", customer, page, pagingPosts);
        var postsPage = 0;
		_logger.LogDebug("PrepareProfileIndexModelAsync(customer={Customer},page={Page}): postsPage → {PostsPage}", customer, page, postsPage);

        if (page.HasValue)
        {
            postsPage = page.Value;
            pagingPosts = true;
        }

        var name = await _customerService.FormatUsernameAsync(customer);
		_logger.LogDebug("PrepareProfileIndexModelAsync(customer={Customer},page={Page}): name → {Name}", customer, page, name);
        var title = string.Format(await _localizationService.GetResourceAsync("Profile.ProfileOf"), name);
		_logger.LogDebug("PrepareProfileIndexModelAsync(customer={Customer},page={Page}): title → {Title}", customer, page, title);

        var model = new ProfileIndexModel
        {
            ProfileTitle = title,
            PostsPage = postsPage,
            PagingPosts = pagingPosts,
            CustomerProfileId = customer.Id,
            ForumsEnabled = _forumSettings.ForumsEnabled
        };
		_logger.LogDebug("<<< Exiting PrepareProfileIndexModelAsync(customer={Customer},page={Page})", customer, page);
        return model;
    }

    /// <summary>
    /// Prepare the profile info model
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the profile info model
    /// </returns>
    public virtual async Task<ProfileInfoModel> PrepareProfileInfoModelAsync(Customer customer)
    {
		_logger.LogDebug(">>> Entering PrepareProfileInfoModelAsync(customer={Customer})", customer);
        ArgumentNullException.ThrowIfNull(customer);

        //avatar
        var avatarUrl = "";
		_logger.LogDebug("PrepareProfileInfoModelAsync(customer={Customer}): avatarUrl → {AvatarUrl}", customer, avatarUrl);
        if (_customerSettings.AllowCustomersToUploadAvatars)
        {
            avatarUrl = await _pictureService.GetPictureUrlAsync(
                await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                _mediaSettings.AvatarPictureSize,
                _customerSettings.DefaultAvatarEnabled,
                defaultPictureType: PictureType.Avatar);
        }

        //location
        var locationEnabled = false;
		_logger.LogDebug("PrepareProfileInfoModelAsync(customer={Customer}): locationEnabled → {LocationEnabled}", customer, locationEnabled);
        var location = string.Empty;
		_logger.LogDebug("PrepareProfileInfoModelAsync(customer={Customer}): location → {Location}", customer, location);
        if (_customerSettings.ShowCustomersLocation)
        {
            locationEnabled = true;

            var countryId = customer.CountryId;
            var country = await _countryService.GetCountryByIdAsync(countryId);
			_logger.LogDebug("PrepareProfileInfoModelAsync(customer={Customer}): country → {Country}", customer, country);
            if (country != null)
            {
                location = await _localizationService.GetLocalizedAsync(country, x => x.Name);
            }
            else
            {
                locationEnabled = false;
            }
        }

        //private message
        var pmEnabled = _forumSettings.AllowPrivateMessages && !await _customerService.IsGuestAsync(customer);
		_logger.LogDebug("PrepareProfileInfoModelAsync(customer={Customer}): pmEnabled → {PmEnabled}", customer, pmEnabled);

        //total forum posts
        var totalPostsEnabled = false;
		_logger.LogDebug("PrepareProfileInfoModelAsync(customer={Customer}): totalPostsEnabled → {TotalPostsEnabled}", customer, totalPostsEnabled);
        var totalPosts = 0;
		_logger.LogDebug("PrepareProfileInfoModelAsync(customer={Customer}): totalPosts → {TotalPosts}", customer, totalPosts);
        if (_forumSettings.ForumsEnabled && _forumSettings.ShowCustomersPostCount)
        {
            totalPostsEnabled = true;
            totalPosts = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.ForumPostCountAttribute);
        }

        //registration date
        var joinDateEnabled = false;
		_logger.LogDebug("PrepareProfileInfoModelAsync(customer={Customer}): joinDateEnabled → {JoinDateEnabled}", customer, joinDateEnabled);
        var joinDate = string.Empty;
		_logger.LogDebug("PrepareProfileInfoModelAsync(customer={Customer}): joinDate → {JoinDate}", customer, joinDate);

        if (_customerSettings.ShowCustomersJoinDate)
        {
            joinDateEnabled = true;
            joinDate = (await _dateTimeHelper.ConvertToUserTimeAsync(customer.CreatedOnUtc, DateTimeKind.Utc)).ToString("f");
        }

        //birth date
        var dateOfBirthEnabled = false;
		_logger.LogDebug("PrepareProfileInfoModelAsync(customer={Customer}): dateOfBirthEnabled → {DateOfBirthEnabled}", customer, dateOfBirthEnabled);
        var dateOfBirth = string.Empty;
		_logger.LogDebug("PrepareProfileInfoModelAsync(customer={Customer}): dateOfBirth → {DateOfBirth}", customer, dateOfBirth);
        if (_customerSettings.DateOfBirthEnabled)
        {
            if (customer.DateOfBirth.HasValue)
            {
                dateOfBirthEnabled = true;
                dateOfBirth = customer.DateOfBirth.Value.ToString("D");
            }
        }

        var model = new ProfileInfoModel
        {
            CustomerProfileId = customer.Id,
            AvatarUrl = avatarUrl,
            LocationEnabled = locationEnabled,
            Location = location,
            PMEnabled = pmEnabled,
            TotalPostsEnabled = totalPostsEnabled,
            TotalPosts = totalPosts.ToString(),
            JoinDateEnabled = joinDateEnabled,
            JoinDate = joinDate,
            DateOfBirthEnabled = dateOfBirthEnabled,
            DateOfBirth = dateOfBirth,
        };

		_logger.LogDebug("<<< Exiting PrepareProfileInfoModelAsync(customer={Customer})", customer);
        return model;
    }

    /// <summary>
    /// Prepare the profile posts model
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="page">Number of posts page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the profile posts model
    /// </returns>
    public virtual async Task<ProfilePostsModel> PrepareProfilePostsModelAsync(Customer customer, int page)
    {
		_logger.LogDebug(">>> Entering PrepareProfilePostsModelAsync(customer={Customer},page={Page})", customer, page);
        ArgumentNullException.ThrowIfNull(customer);

        if (page > 0)
        {
            page -= 1;
        }

        var pageSize = _forumSettings.LatestCustomerPostsPageSize;

        var list = await _forumService.GetAllPostsAsync(0, customer.Id, string.Empty, false, page, pageSize);
		_logger.LogDebug("PrepareProfilePostsModelAsync(customer={Customer},page={Page}): list → {List}", customer, page, list);

        var latestPosts = new List<PostsModel>();
		_logger.LogDebug("PrepareProfilePostsModelAsync(customer={Customer},page={Page}): latestPosts → {LatestPosts}", customer, page, latestPosts);

        foreach (var forumPost in list)
        {
            string posted;
            if (_forumSettings.RelativeDateTimeFormattingEnabled)
            {
                var languageCode = (await _workContext.GetWorkingLanguageAsync()).LanguageCulture;
				_logger.LogDebug("PrepareProfilePostsModelAsync(customer={Customer},page={Page}): languageCode → {LanguageCode}", customer, page, languageCode);
                var postedAgo = forumPost.CreatedOnUtc.RelativeFormat(languageCode);
				_logger.LogDebug("PrepareProfilePostsModelAsync(customer={Customer},page={Page}): postedAgo → {PostedAgo}", customer, page, postedAgo);
                posted = string.Format(await _localizationService.GetResourceAsync("Common.RelativeDateTime.Past"), postedAgo);
				_logger.LogDebug("PrepareProfilePostsModelAsync(customer={Customer},page={Page}): posted → {Posted}", customer, page, posted);
            }
            else
            {
                posted = (await _dateTimeHelper.ConvertToUserTimeAsync(forumPost.CreatedOnUtc, DateTimeKind.Utc)).ToString("f");
            }

            var topic = await _forumService.GetTopicByIdAsync(forumPost.TopicId);
			_logger.LogDebug("PrepareProfilePostsModelAsync(customer={Customer},page={Page}): topic → {Topic}", customer, page, topic);

            latestPosts.Add(new PostsModel
            {
                ForumTopicId = topic.Id,
                ForumTopicTitle = topic.Subject,
                ForumTopicSlug = await _forumService.GetTopicSeNameAsync(topic),
                ForumPostText = _forumService.FormatPostText(forumPost),
                Posted = posted
            });
        }

        var pagerModel = new PagerModel(_localizationService)
        {
            PageSize = list.PageSize,
            TotalRecords = list.TotalCount,
            PageIndex = list.PageIndex,
            ShowTotalSummary = false,
            RouteActionName = NopRouteNames.Standard.CUSTOMER_PROFILE_PAGED,
            UseRouteLinks = true,
            RouteValues = new SlugRouteValues { PageNumber = page, Id = customer.Id }
        };

        var model = new ProfilePostsModel
        {
            PagerModel = pagerModel,
            Posts = latestPosts,
        };

		_logger.LogDebug("<<< Exiting PrepareProfilePostsModelAsync(customer={Customer},page={Page})", customer, page);
        return model;
    }

    #endregion
}