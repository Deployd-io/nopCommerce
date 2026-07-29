using FluentMigrator;
using LinqToDB;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.UpgradeTo470;

[NopUpdateMigration("2023-01-01 00:00:00", "4.70", UpdateMigrationType.Data)]
public class DataMigration : Migration
{
    private readonly INopDataProvider _dataProvider;

    public DataMigration(INopDataProvider dataProvider)
    {
		logger.LogDebug(">>> Entering DataMigration(dataProvider={DataProvider})", dataProvider);
        _dataProvider = dataProvider;
		logger.LogDebug("<<< Exiting DataMigration(dataProvider={DataProvider})", dataProvider);
		logger.LogDebug("DataMigration(dataProvider={DataProvider}): _dataProvider → {_dataProvider}", dataProvider, _dataProvider);
    }

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		logger.LogDebug(">>> Entering Up()");
        //#5312 new activity log type
        var activityLogTypeTable = _dataProvider.GetTable<ActivityLogType>();

		logger.LogDebug("Up(): activityLogTypeTable → {ActivityLogTypeTable}", activityLogTypeTable);
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

		logger.LogDebug("Up(): productAttributeCombinationTableName → {ProductAttributeCombinationTableName}", productAttributeCombinationTableName);
        var pac = Schema.Table(productAttributeCombinationTableName);
        var columnName = "PictureId";
		logger.LogDebug("Up(): pac → {Pac}", pac);
		logger.LogDebug("Up(): columnName → {ColumnName}", columnName);

        if (pac.Column(columnName).Exists())
        {
#pragma warning disable CS0618
            var combinationQuery =
                from c in _dataProvider.GetTable<ProductAttributeCombination>()
                join p in _dataProvider.GetTable<Picture>() on c.PictureId equals p.Id
                select c;
#pragma warning restore CS0618

            pageIndex = 0;
			logger.LogDebug("Up(): pageIndex → {PageIndex}", pageIndex);

            while (true)
            {
                var combinations = combinationQuery.ToPagedListAsync(pageIndex, pageSize).Result;

				logger.LogDebug("Up(): combinations → {Combinations}", combinations);
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
					logger.LogDebug("Up(): combination.PictureId → {Arg0}", combination.PictureId);
                }
#pragma warning restore CS0618

                _dataProvider.UpdateEntitiesAsync(combinations);

                pageIndex++;
            }
        }

        var productAttributeValueTableName = nameof(ProductAttributeValue);
        var pav = Schema.Table(productAttributeValueTableName);
		logger.LogDebug("Up(): productAttributeValueTableName → {ProductAttributeValueTableName}", productAttributeValueTableName);
		logger.LogDebug("Up(): pav → {Pav}", pav);

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

				logger.LogDebug("Up(): values → {Values}", values);
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
					logger.LogDebug("Up(): value.PictureId → {Arg0}", value.PictureId);
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

		logger.LogDebug("Up(): productTableName → {ProductTableName}", productTableName);
        //remove column
        var isTelecommunicationsOrBroadcastingOrElectronicServicesColumnName = "IsTelecommunicationsOrBroadcastingOrElectronicServices";
        if (Schema.Table(productTableName).Column(isTelecommunicationsOrBroadcastingOrElectronicServicesColumnName).Exists())
			logger.LogDebug("Up(): isTelecommunicationsOrBroadcastingOrElectronicServicesColumnName → {IsTelecommunicationsOrBroadcastingOrElectronicServicesColumnName}", isTelecommunicationsOrBroadcastingOrElectronicServicesColumnName);
            Delete.Column(isTelecommunicationsOrBroadcastingOrElectronicServicesColumnName).FromTable(productTableName);

        //New message template
        if (!_dataProvider.GetTable<MessageTemplate>().Any(st => string.Compare(st.Name, MessageTemplateSystemNames.DELETE_CUSTOMER_REQUEST_STORE_OWNER_NOTIFICATION, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            var eaGeneral = _dataProvider.GetTable<EmailAccount>().FirstOrDefault() ?? throw new Exception("Default email account cannot be loaded");
            _dataProvider.InsertEntity(new MessageTemplate()
            {
			logger.LogDebug("Up(): eaGeneral → {EaGeneral}", eaGeneral);
                Name = MessageTemplateSystemNames.DELETE_CUSTOMER_REQUEST_STORE_OWNER_NOTIFICATION,
                Subject = "%Store.Name%. New request to delete customer (GDPR)",
                Body = $"%Customer.Email% has requested account deletion. You can consider this in the admin area.",
                IsActive = true,
                EmailAccountId = eaGeneral.Id
            });
        }

        //#7031
        var emailAccountTableName = nameof(EmailAccount);
        var credentialsColumnName = "UseDefaultCredentials";
		logger.LogDebug("Up(): emailAccountTableName → {EmailAccountTableName}", emailAccountTableName);
		logger.LogDebug("Up(): credentialsColumnName → {CredentialsColumnName}", credentialsColumnName);

        if (Schema.Table(emailAccountTableName).Column(credentialsColumnName).Exists())
        {
            var emailAccounts = _dataProvider.GetTable<EmailAccount>().ToList();
            foreach (var item in emailAccounts)
            {
			logger.LogDebug("Up(): emailAccounts → {EmailAccounts}", emailAccounts);
                if (!string.IsNullOrEmpty(item.Username))
                    item.EmailAuthenticationMethod = EmailAuthenticationMethod.Login;
					logger.LogDebug("Up(): item.EmailAuthenticationMethod → {Arg0}", item.EmailAuthenticationMethod);
            }

            _dataProvider.UpdateEntities(emailAccounts);

            //remove column
            Delete.Column(credentialsColumnName).FromTable(emailAccountTableName);
        }

        //#6978
        var newsLetterSubscriptionTableName = nameof(NewsLetterSubscription);
        var languageIdColumnName = nameof(NewsLetterSubscription.LanguageId);
		logger.LogDebug("Up(): newsLetterSubscriptionTableName → {NewsLetterSubscriptionTableName}", newsLetterSubscriptionTableName);
		logger.LogDebug("Up(): languageIdColumnName → {LanguageIdColumnName}", languageIdColumnName);
        if (Schema.Table(newsLetterSubscriptionTableName).Column(languageIdColumnName).Exists())
        {
            var defaultLanguageId = _dataProvider.GetTable<Language>().FirstOrDefault()?.Id ?? 0;

			logger.LogDebug("Up(): defaultLanguageId → {DefaultLanguageId}", defaultLanguageId);
            _dataProvider.GetTable<NewsLetterSubscription>()
                .Set(p => p.LanguageId, defaultLanguageId)
                .Update();
        }
		logger.LogDebug("<<< Exiting Up()");
    }

    public override void Down()
    {
		logger.LogDebug(">>> Entering Down()");
        //add the downgrade logic if necessary 
		logger.LogDebug("<<< Exiting Down()");
    }
}