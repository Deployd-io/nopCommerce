using FluentMigrator;
using LinqToDB;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Data.Mapping;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Migrations.UpgradeTo470;

[NopUpdateMigration("2023-01-01 00:00:00", "4.70", UpdateMigrationType.Data)]
public class DataMigration : Migration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private readonly INopDataProvider _dataProvider;

    public DataMigration(INopDataProvider dataProvider)
    {
		_logger.LogDebug(">>> Entering DataMigration(dataProvider={DataProvider})", dataProvider);
        _dataProvider = dataProvider;
		_logger.LogDebug("DataMigration(dataProvider={DataProvider}): _dataProvider → {_dataProvider}", dataProvider, _dataProvider);
		_logger.LogDebug("<<< Exiting DataMigration(dataProvider={DataProvider})", dataProvider);
    }

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        //#5312 new activity log type
        var activityLogTypeTable = _dataProvider.GetTable<ActivityLogType>();
		_logger.LogDebug("Up(): activityLogTypeTable → {ActivityLogTypeTable}", activityLogTypeTable);

        if (!activityLogTypeTable.Any(alt =>
                string.Compare(alt.SystemKeyword, "ImportCustomers", StringComparison.InvariantCultureIgnoreCase) ==
                0))
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "ImportCustomers",
                    Enabled = true,
                    Name = "Customers were imported"
                }
            );

        //6660 new activity log type for update plugin
        if (!activityLogTypeTable.Any(alt =>
                string.Compare(alt.SystemKeyword, "UpdatePlugin", StringComparison.InvariantCultureIgnoreCase) ==
                0))
            _dataProvider.InsertEntity(
                new ActivityLogType { SystemKeyword = "UpdatePlugin", Enabled = true, Name = "Update a plugin" }
            );

        //1934
        int pageIndex;
        var pageSize = 500;
        var productAttributeCombinationTableName = nameof(ProductAttributeCombination);
		_logger.LogDebug("Up(): productAttributeCombinationTableName → {ProductAttributeCombinationTableName}", productAttributeCombinationTableName);

        var pac = Schema.Table(productAttributeCombinationTableName);
		_logger.LogDebug("Up(): pac → {Pac}", pac);
        var columnName = "PictureId";
		_logger.LogDebug("Up(): columnName → {ColumnName}", columnName);

        if (pac.Column(columnName).Exists())
		_logger.LogDebug("<<< Exiting Up()");
        {
#pragma warning disable CS0618
            var combinationQuery =
                from c in _dataProvider.GetTable<ProductAttributeCombination>()
                join p in _dataProvider.GetTable<Picture>() on c.PictureId equals p.Id
                select c;
#pragma warning restore CS0618

            pageIndex = 0;

			_logger.LogDebug("Up(): pageIndex → {PageIndex}", pageIndex);
            while (true)
            {
                var combinations = combinationQuery.ToPagedListAsync(pageIndex, pageSize).Result;
				_logger.LogDebug("Up(): combinations → {Combinations}", combinations);

                if (!combinations.Any())
                    break;

#pragma warning disable CS0618
                foreach (var combination in combinations)
                {
                    if (!combination.PictureId.HasValue)
                        continue;

                    _dataProvider.InsertEntity(new ProductAttributeCombinationPicture
                    {
                        PictureId = combination.PictureId.Value,
                        ProductAttributeCombinationId = combination.Id
                    });

                    combination.PictureId = null;
					_logger.LogDebug("Up(): combination.PictureId → {Arg0}", combination.PictureId);
                }
#pragma warning restore CS0618

                _dataProvider.UpdateEntitiesAsync(combinations);

                pageIndex++;
            }
        }

        var productAttributeValueTableName = nameof(ProductAttributeValue);
		_logger.LogDebug("Up(): productAttributeValueTableName → {ProductAttributeValueTableName}", productAttributeValueTableName);
        var pav = Schema.Table(productAttributeValueTableName);
		_logger.LogDebug("Up(): pav → {Pav}", pav);

        if (pav.Column(columnName).Exists())
        {
#pragma warning disable CS0618
            var valueQuery =
                from c in _dataProvider.GetTable<ProductAttributeValue>()
                join p in _dataProvider.GetTable<Picture>() on c.PictureId equals p.Id
                select c;
#pragma warning restore CS0618

            pageIndex = 0;

            while (true)
            {
                var values = valueQuery.ToPagedListAsync(pageIndex, pageSize).Result;
				_logger.LogDebug("Up(): values → {Values}", values);

                if (!values.Any())
                    break;

#pragma warning disable CS0618
                foreach (var value in values)
                {
                    if (!value.PictureId.HasValue)
                        continue;

                    _dataProvider.InsertEntity(new ProductAttributeValuePicture
                    {
                        PictureId = value.PictureId.Value,
                        ProductAttributeValueId = value.Id
                    });

                    value.PictureId = null;
					_logger.LogDebug("Up(): value.PictureId → {Arg0}", value.PictureId);
                }
#pragma warning restore CS0618

                _dataProvider.UpdateEntitiesAsync(values);

                pageIndex++;
            }
        }

        // new permission
        if (_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "AccessProfiling", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.BulkDeleteEntitiesAsync<PermissionRecord>(pr => pr.SystemName == "AccessProfiling");
        }

        //#6890
        var productTableName = NameCompatibilityManager.GetTableName(typeof(Product));
		_logger.LogDebug("Up(): productTableName → {ProductTableName}", productTableName);

        //remove column
        var isTelecommunicationsOrBroadcastingOrElectronicServicesColumnName = "IsTelecommunicationsOrBroadcastingOrElectronicServices";
		_logger.LogDebug("Up(): isTelecommunicationsOrBroadcastingOrElectronicServicesColumnName → {IsTelecommunicationsOrBroadcastingOrElectronicServicesColumnName}", isTelecommunicationsOrBroadcastingOrElectronicServicesColumnName);
        if (Schema.Table(productTableName).Column(isTelecommunicationsOrBroadcastingOrElectronicServicesColumnName).Exists())
            Delete.Column(isTelecommunicationsOrBroadcastingOrElectronicServicesColumnName).FromTable(productTableName);

        //New message template
        if (!_dataProvider.GetTable<MessageTemplate>().Any(st => string.Compare(st.Name, MessageTemplateSystemNames.DELETE_CUSTOMER_REQUEST_STORE_OWNER_NOTIFICATION, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            var eaGeneral = _dataProvider.GetTable<EmailAccount>().FirstOrDefault() ?? throw new Exception("Default email account cannot be loaded");
			_logger.LogDebug("Up(): eaGeneral → {EaGeneral}", eaGeneral);
            _dataProvider.InsertEntity(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.DELETE_CUSTOMER_REQUEST_STORE_OWNER_NOTIFICATION,
                Subject = "%Store.Name%. New request to delete customer (GDPR)",
                Body = $"%Customer.Email% has requested account deletion. You can consider this in the admin area.",
                IsActive = true,
                EmailAccountId = eaGeneral.Id
            });
        }

        //#7031
        var emailAccountTableName = nameof(EmailAccount);
		_logger.LogDebug("Up(): emailAccountTableName → {EmailAccountTableName}", emailAccountTableName);
        var credentialsColumnName = "UseDefaultCredentials";
		_logger.LogDebug("Up(): credentialsColumnName → {CredentialsColumnName}", credentialsColumnName);

        if (Schema.Table(emailAccountTableName).Column(credentialsColumnName).Exists())
        {
            var emailAccounts = _dataProvider.GetTable<EmailAccount>().ToList();
			_logger.LogDebug("Up(): emailAccounts → {EmailAccounts}", emailAccounts);
            foreach (var item in emailAccounts)
            {
                if (!string.IsNullOrEmpty(item.Username))
                    item.EmailAuthenticationMethod = EmailAuthenticationMethod.Login;
					_logger.LogDebug("Up(): item.EmailAuthenticationMethod → {Arg0}", item.EmailAuthenticationMethod);
            }

            _dataProvider.UpdateEntities(emailAccounts);

            //remove column
            Delete.Column(credentialsColumnName).FromTable(emailAccountTableName);
        }

        //#6978
        var newsLetterSubscriptionTableName = nameof(NewsLetterSubscription);
		_logger.LogDebug("Up(): newsLetterSubscriptionTableName → {NewsLetterSubscriptionTableName}", newsLetterSubscriptionTableName);
        var languageIdColumnName = nameof(NewsLetterSubscription.LanguageId);
		_logger.LogDebug("Up(): languageIdColumnName → {LanguageIdColumnName}", languageIdColumnName);
        if (Schema.Table(newsLetterSubscriptionTableName).Column(languageIdColumnName).Exists())
        {
            var defaultLanguageId = _dataProvider.GetTable<Language>().FirstOrDefault()?.Id ?? 0;
			_logger.LogDebug("Up(): defaultLanguageId → {DefaultLanguageId}", defaultLanguageId);

            _dataProvider.GetTable<NewsLetterSubscription>()
                .Set(p => p.LanguageId, defaultLanguageId)
                .Update();
        }
    }

    public override void Down()
    {
		_logger.LogDebug(">>> Entering Down()");
        //add the downgrade logic if necessary 
		_logger.LogDebug("<<< Exiting Down()");
    }
}