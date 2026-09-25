using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Migrations.UpgradeTo470;

[NopSchemaMigration("2024-04-20 00:00:00", "SchemaMigration for 4.70.0")]
public class SchemaMigration : ForwardOnlyMigration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogError(">>> Entering Up()");
        //#6167
        var messageTemplateTableName = nameof(MessageTemplate);
		_logger.LogError("Up(): messageTemplateTableName → {MessageTemplateTableName}", messageTemplateTableName);
        var allowDirectReplyColumnName = nameof(MessageTemplate.AllowDirectReply);
		_logger.LogError("Up(): allowDirectReplyColumnName → {AllowDirectReplyColumnName}", allowDirectReplyColumnName);

        //add column
        if (!Schema.Table(messageTemplateTableName).Column(allowDirectReplyColumnName).Exists())
            Alter.Table(messageTemplateTableName)
                .AddColumn(allowDirectReplyColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);

        //1934
        if (!Schema.Table(nameof(ProductAttributeCombinationPicture)).Exists())
            Create.TableFor<ProductAttributeCombinationPicture>();

        if (!Schema.Table(nameof(ProductAttributeValuePicture)).Exists())
            Create.TableFor<ProductAttributeValuePicture>();

        var productTableName = nameof(Product);
		_logger.LogError("Up(): productTableName → {ProductTableName}", productTableName);
        var displayAttributeCombinationImagesOnlyColumnName = nameof(Product.DisplayAttributeCombinationImagesOnly);
		_logger.LogError("Up(): displayAttributeCombinationImagesOnlyColumnName → {DisplayAttributeCombinationImagesOnlyColumnName}", displayAttributeCombinationImagesOnlyColumnName);

        if (!Schema.Table(productTableName).Column(displayAttributeCombinationImagesOnlyColumnName).Exists())
            Alter.Table(productTableName)
                .AddColumn(displayAttributeCombinationImagesOnlyColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);

        //#6710
        var productAttributeCombinationTableName = nameof(ProductAttributeCombination);
		_logger.LogError("Up(): productAttributeCombinationTableName → {ProductAttributeCombinationTableName}", productAttributeCombinationTableName);
        var pac = Schema.Table(productAttributeCombinationTableName);
		_logger.LogError("Up(): pac → {Pac}", pac);
        var columnName = "PictureId";
		_logger.LogError("Up(): columnName → {ColumnName}", columnName);
        var description = "The field is not used since 4.70 and is left only for the update process use the ProductAttributeCombinationPicture instead";
		_logger.LogError("Up(): description → {Description}", description);

        if (pac.Column(columnName).Exists())
            Alter.Table(productAttributeCombinationTableName)
                .AlterColumn(columnName).AsInt32().Nullable().WithColumnDescription(description);
        else
            Alter.Table(productAttributeCombinationTableName)
                .AddColumn(columnName).AsInt32().Nullable().SetExistingRowsTo(null).WithColumnDescription(description);

        var productAttributeValueTableName = nameof(ProductAttributeValue);
		_logger.LogError("Up(): productAttributeValueTableName → {ProductAttributeValueTableName}", productAttributeValueTableName);
        var pav = Schema.Table(productAttributeValueTableName);
		_logger.LogError("Up(): pav → {Pav}", pav);

        if (pav.Column(columnName).Exists())
            Alter.Table(productAttributeValueTableName)
                .AlterColumn(columnName).AsInt32().Nullable().WithColumnDescription(description);
        else
            Alter.Table(productAttributeValueTableName)
                .AddColumn(columnName).AsInt32().Nullable().SetExistingRowsTo(null).WithColumnDescription(description);

        // 6771
        Alter.Table(nameof(Customer)).AlterColumn(nameof(Customer.LastIpAddress)).AsString(100).Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumPost))).AlterColumn(nameof(ForumPost.IPAddress)).AsString(100).Nullable();
        Alter.Table(nameof(ActivityLog)).AlterColumn(nameof(ActivityLog.IpAddress)).AsString(100).Nullable();
        Alter.Table(nameof(Log)).AlterColumn(nameof(Log.IpAddress)).AsString(100).Nullable();
        Alter.Table(nameof(Order)).AlterColumn(nameof(Order.CustomerIp)).AsString(100).Nullable();

        //#6958
        //add column
        var emailAccountTableName = nameof(EmailAccount);
		_logger.LogError("Up(): emailAccountTableName → {EmailAccountTableName}", emailAccountTableName);
        var maxNumberOfEmailsColumnName = nameof(EmailAccount.MaxNumberOfEmails);
		_logger.LogError("Up(): maxNumberOfEmailsColumnName → {MaxNumberOfEmailsColumnName}", maxNumberOfEmailsColumnName);

        if (!Schema.Table(emailAccountTableName).Column(maxNumberOfEmailsColumnName).Exists())
            Alter.Table(emailAccountTableName)
                .AddColumn(maxNumberOfEmailsColumnName).AsInt32().NotNullable().SetExistingRowsTo(50);

        //#7031
        if (!Schema.Table(emailAccountTableName).Column(nameof(EmailAccount.EmailAuthenticationMethodId)).Exists())
            Alter.Table(emailAccountTableName)
                .AddColumn(nameof(EmailAccount.EmailAuthenticationMethodId)).AsInt32().NotNullable().SetExistingRowsTo(0);

        if (!Schema.Table(emailAccountTableName).Column(nameof(EmailAccount.ClientId)).Exists())
            Alter.Table(emailAccountTableName)
                .AddColumn(nameof(EmailAccount.ClientId)).AsString().Nullable();

        if (!Schema.Table(emailAccountTableName).Column(nameof(EmailAccount.ClientSecret)).Exists())
            Alter.Table(emailAccountTableName)
                .AddColumn(nameof(EmailAccount.ClientSecret)).AsString().Nullable();

        if (!Schema.Table(emailAccountTableName).Column(nameof(EmailAccount.TenantId)).Exists())
            Alter.Table(emailAccountTableName)
                .AddColumn(nameof(EmailAccount.TenantId)).AsString().Nullable();

        //#6978
        var newsLetterSubscriptionTableName = nameof(NewsLetterSubscription);
		_logger.LogError("Up(): newsLetterSubscriptionTableName → {NewsLetterSubscriptionTableName}", newsLetterSubscriptionTableName);
        if (!Schema.Table(newsLetterSubscriptionTableName).Column(nameof(NewsLetterSubscription.LanguageId)).Exists())
            Alter.Table(newsLetterSubscriptionTableName)
                .AddColumn(nameof(NewsLetterSubscription.LanguageId)).AsInt32().NotNullable().SetExistingRowsTo(1);
				_logger.LogWarning("<<< Exiting Up()");
    }
}