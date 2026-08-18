using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Models.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Order;

public partial record CustomerRewardPointsModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CustomerRewardPointsModel()
    {
		_logger.LogDebug(">>> Entering CustomerRewardPointsModel()");
        RewardPoints = new List<RewardPointsHistoryModel>();
		_logger.LogDebug("CustomerRewardPointsModel(): RewardPoints → {RewardPoints}", RewardPoints);
		_logger.LogDebug("<<< Exiting CustomerRewardPointsModel()");
    }

    public IList<RewardPointsHistoryModel> RewardPoints { get; set; }
    public PagerModel PagerModel { get; set; }
    public int RewardPointsBalance { get; set; }
    public string RewardPointsAmount { get; set; }
    public int MinimumRewardPointsBalance { get; set; }
    public string MinimumRewardPointsAmount { get; set; }

    #region Nested classes

    public partial record RewardPointsHistoryModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("RewardPoints.Fields.Points")]
        public int Points { get; set; }

        [NopResourceDisplayName("RewardPoints.Fields.PointsBalance")]
        public string PointsBalance { get; set; }

        [NopResourceDisplayName("RewardPoints.Fields.Message")]
        public string Message { get; set; }

        [NopResourceDisplayName("RewardPoints.Fields.CreatedDate")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("RewardPoints.Fields.EndDate")]
        public DateTime? EndDate { get; set; }
    }

    #endregion
}