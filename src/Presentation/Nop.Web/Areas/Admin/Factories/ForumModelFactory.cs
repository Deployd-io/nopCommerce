using Nop.Core.Domain.Forums;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Forums;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the forum model factory implementation
/// </summary>
public partial class ForumModelFactory : IForumModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IForumService _forumService;

    #endregion

    #region Ctor

    public ForumModelFactory(IDateTimeHelper dateTimeHelper, IForumService forumService)
    {
		_logger.LogDebug(">>> Entering ForumModelFactory(dateTimeHelper={DateTimeHelper},forumService={ForumService})", dateTimeHelper, forumService);
        _dateTimeHelper = dateTimeHelper;
        _forumService = forumService;
		_logger.LogDebug("<<< Exiting ForumModelFactory(dateTimeHelper={DateTimeHelper},forumService={ForumService})", dateTimeHelper, forumService);
		_logger.LogDebug("ForumModelFactory(dateTimeHelper={DateTimeHelper},forumService={ForumService}): _forumService → {_forumService}", dateTimeHelper, forumService, _forumService);
		_logger.LogDebug("ForumModelFactory(dateTimeHelper={DateTimeHelper},forumService={ForumService}): _dateTimeHelper → {_dateTimeHelper}", dateTimeHelper, forumService, _dateTimeHelper);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare forum search model
    /// </summary>
    /// <param name="searchModel">Forum search model</param>
    /// <returns>Forum search model</returns>
    protected virtual ForumSearchModel PrepareForumSearchModel(ForumSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareForumSearchModel(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareForumSearchModel(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare forum group search model
    /// </summary>
    /// <param name="searchModel">Forum group search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum group search model
    /// </returns>
    public virtual Task<ForumGroupSearchModel> PrepareForumGroupSearchModelAsync(ForumGroupSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareForumGroupSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare nested search model
        PrepareForumSearchModel(searchModel.ForumSearch);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareForumGroupSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged forum group list model
    /// </summary>
    /// <param name="searchModel">Forum group search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum group list model
    /// </returns>
    public virtual async Task<ForumGroupListModel> PrepareForumGroupListModelAsync(ForumGroupSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareForumGroupListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get forum groups
        var forumGroups = (await _forumService.GetAllForumGroupsAsync()).ToPagedList(searchModel);

		_logger.LogDebug("PrepareForumGroupListModelAsync(searchModel={SearchModel}): forumGroups → {ForumGroups}", searchModel, forumGroups);
        //prepare list model
        var model = await new ForumGroupListModel().PrepareToGridAsync(searchModel, forumGroups, () =>
        {
            return forumGroups.SelectAwait(async forumGroup =>
            {
                //fill in model values from the entity
                var forumGroupModel = forumGroup.ToModel<ForumGroupModel>();

				_logger.LogDebug("PrepareForumGroupListModelAsync(searchModel={SearchModel}): forumGroupModel → {ForumGroupModel}", searchModel, forumGroupModel);
                //convert dates to the user time
                forumGroupModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(forumGroup.CreatedOnUtc, DateTimeKind.Utc);

                return forumGroupModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareForumGroupListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare forum group model
    /// </summary>
    /// <param name="model">Forum group model</param>
    /// <param name="forumGroup">Forum group</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum group model
    /// </returns>
    public virtual Task<ForumGroupModel> PrepareForumGroupModelAsync(ForumGroupModel model, ForumGroup forumGroup, bool excludeProperties = false)
    {
        //fill in model values from the entity
        if (forumGroup != null)
            model ??= forumGroup.ToModel<ForumGroupModel>();

        //set default values for the new model
        if (forumGroup == null)
            model.DisplayOrder = 1;

        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare paged forum list model
    /// </summary>
    /// <param name="searchModel">Forum search model</param>
    /// <param name="forumGroup">Forum group</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum list model
    /// </returns>
    public virtual async Task<ForumListModel> PrepareForumListModelAsync(ForumSearchModel searchModel, ForumGroup forumGroup)
    {
		_logger.LogDebug(">>> Entering PrepareForumListModelAsync(searchModel={SearchModel},forumGroup={ForumGroup})", searchModel, forumGroup);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(forumGroup);

        //get forums
        var forums = (await _forumService.GetAllForumsByGroupIdAsync(forumGroup.Id)).ToPagedList(searchModel);

		_logger.LogDebug("PrepareForumListModelAsync(searchModel={SearchModel},forumGroup={ForumGroup}): forums → {Forums}", searchModel, forumGroup, forums);
        //prepare list model
        var model = await new ForumListModel().PrepareToGridAsync(searchModel, forums, () =>
        {
            return forums.SelectAwait(async forum =>
            {
                //fill in model values from the entity
                var forumModel = forum.ToModel<ForumModel>();

				_logger.LogDebug("PrepareForumListModelAsync(searchModel={SearchModel},forumGroup={ForumGroup}): forumModel → {ForumModel}", searchModel, forumGroup, forumModel);
                //convert dates to the user time
                forumModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(forum.CreatedOnUtc, DateTimeKind.Utc);

                return forumModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareForumListModelAsync(searchModel={SearchModel},forumGroup={ForumGroup})", searchModel, forumGroup);
        return model;
    }

    /// <summary>
    /// Prepare forum model
    /// </summary>
    /// <param name="model">Forum model</param>
    /// <param name="forum">Forum</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum model
    /// </returns>
    public virtual async Task<ForumModel> PrepareForumModelAsync(ForumModel model, Forum forum, bool excludeProperties = false)
    {
        //fill in model values from the entity
        if (forum != null)
            model ??= forum.ToModel<ForumModel>();

        //set default values for the new model
        if (forum == null)
            model.DisplayOrder = 1;

        //prepare available forum groups
        foreach (var forumGroup in await _forumService.GetAllForumGroupsAsync())
        {
            model.ForumGroups.Add(forumGroup.ToModel<ForumGroupModel>());
        }

        return model;
    }

    #endregion
}