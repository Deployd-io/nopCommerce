using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Common;

public partial record MaintenanceModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public MaintenanceModel()
    {
		_logger.LogDebug(">>> Entering MaintenanceModel()");
        DeleteGuests = new();
        DeleteAbandonedCarts = new();
		_logger.LogDebug("MaintenanceModel(): DeleteAbandonedCarts → {DeleteAbandonedCarts}", DeleteAbandonedCarts);
		_logger.LogDebug("MaintenanceModel(): DeleteGuests → {DeleteGuests}", DeleteGuests);
        DeleteExportedFiles = new();
		_logger.LogDebug("MaintenanceModel(): DeleteExportedFiles → {DeleteExportedFiles}", DeleteExportedFiles);
        BackupFileSearchModel = new();
		_logger.LogDebug("MaintenanceModel(): BackupFileSearchModel → {BackupFileSearchModel}", BackupFileSearchModel);
        DeleteAlreadySentQueuedEmails = new();
		_logger.LogDebug("MaintenanceModel(): DeleteAlreadySentQueuedEmails → {DeleteAlreadySentQueuedEmails}", DeleteAlreadySentQueuedEmails);
        DeleteMinificationFiles = new();
		_logger.LogDebug("MaintenanceModel(): DeleteMinificationFiles → {DeleteMinificationFiles}", DeleteMinificationFiles);
        DeleteThumbsFiles = new();
		_logger.LogDebug("MaintenanceModel(): DeleteThumbsFiles → {DeleteThumbsFiles}", DeleteThumbsFiles);
		_logger.LogDebug("<<< Exiting MaintenanceModel()");
    }

    public DeleteGuestsModel DeleteGuests { get; set; }

    public DeleteAbandonedCartsModel DeleteAbandonedCarts { get; set; }

    public DeleteExportedFilesModel DeleteExportedFiles { get; set; }

    public BackupFileSearchModel BackupFileSearchModel { get; set; }

    public DeleteAlreadySentQueuedEmailsModel DeleteAlreadySentQueuedEmails { get; set; }

    public DeleteMinificationFilesModel DeleteMinificationFiles { get; set; }

    public DeleteThumbFilesModel DeleteThumbsFiles { get; set; }

    public bool BackupSupported { get; set; }

    #region Nested classes

    public partial record DeleteGuestsModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.System.Maintenance.DeleteGuests.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.System.Maintenance.DeleteGuests.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [NopResourceDisplayName("Admin.System.Maintenance.DeleteGuests.OnlyWithoutShoppingCart")]
        public bool OnlyWithoutShoppingCart { get; set; }

        public int? NumberOfDeletedCustomers { get; set; }
    }

    public partial record DeleteAbandonedCartsModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.System.Maintenance.DeleteAbandonedCarts.OlderThan")]
        [UIHint("Date")]
        public DateTime OlderThan { get; set; }

        public int? NumberOfDeletedItems { get; set; }
    }

    public partial record DeleteExportedFilesModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.System.Maintenance.DeleteExportedFiles.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.System.Maintenance.DeleteExportedFiles.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        public int? NumberOfDeletedFiles { get; set; }
    }

    public partial record DeleteAlreadySentQueuedEmailsModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.System.Maintenance.DeleteAlreadySentQueuedEmails.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.System.Maintenance.DeleteAlreadySentQueuedEmails.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        public int? NumberOfDeletedEmails { get; set; }
    }

    public partial record DeleteMinificationFilesModel : BaseNopModel
    {
        public int? NumberOfDeletedFiles { get; set; }
    }

    public partial record DeleteThumbFilesModel : BaseNopModel
    {
        public bool IsDeleteThumbsSupported { get; set; }
        
        public string FilesCountText { get; set; }
        
        public string FilesSizeText { get; set; }
    }

    #endregion
}