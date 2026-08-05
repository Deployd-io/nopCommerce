using FluentMigrator;
using Nop.Core.Domain.Messages;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Migrations.UpgradeTo480;

[NopUpdateMigration("2023-10-30 12:00:00", "4.80", UpdateMigrationType.Data)]
public class DataMigration : Migration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private readonly INopDataProvider _dataProvider;

    public DataMigration(INopDataProvider dataProvider)
    {
		_logger.LogDebug(">>> Entering DataMigration(dataProvider={DataProvider})", dataProvider);
        _dataProvider = dataProvider;
		_logger.LogDebug("<<< Exiting DataMigration(dataProvider={DataProvider})", dataProvider);
		_logger.LogDebug("DataMigration(dataProvider={DataProvider}): _dataProvider → {_dataProvider}", dataProvider, _dataProvider);
    }

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        //#7108 New message template
        if (!_dataProvider.GetTable<MessageTemplate>().Any(st => string.Compare(st.Name, MessageTemplateSystemNames.ORDER_CANCELLED_VENDOR_NOTIFICATION, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            var eaGeneral = _dataProvider.GetTable<EmailAccount>().FirstOrDefault() ?? throw new Exception("Default email account cannot be loaded");
            _dataProvider.InsertEntity(new MessageTemplate()
            {
			_logger.LogDebug("Up(): eaGeneral → {EaGeneral}", eaGeneral);
                Name = MessageTemplateSystemNames.ORDER_CANCELLED_VENDOR_NOTIFICATION,
                Subject = "%Store.Name%. Order #%Order.OrderNumber% cancelled",
                Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order #%Order.OrderNumber% has been cancelled.{Environment.NewLine}<br />{Environment.NewLine}Customer: %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id
            });
        }

        //#5898
        if (!_dataProvider.GetTable<MessageTemplate>().Any(st => string.Compare(st.Name, MessageTemplateSystemNames.QUANTITY_BELOW_VENDOR_NOTIFICATION, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            var eaGeneral = _dataProvider.GetTable<EmailAccount>().FirstOrDefault() ?? throw new Exception("Default email account cannot be loaded");
            _dataProvider.InsertEntity(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.QUANTITY_BELOW_VENDOR_NOTIFICATION,
                Subject = "%Store.Name%. Quantity below notification. %Product.Name%\"",
                Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Product.Name% (ID: %Product.ID%) low quantity.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Quantity: %Product.StockQuantity%{Environment.NewLine}<br />{Environment.NewLine}</p>{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id
            });
        }

        if (!_dataProvider.GetTable<MessageTemplate>().Any(st => string.Compare(st.Name, MessageTemplateSystemNames.QUANTITY_BELOW_ATTRIBUTE_COMBINATION_VENDOR_NOTIFICATION, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            var eaGeneral = _dataProvider.GetTable<EmailAccount>().FirstOrDefault() ?? throw new Exception("Default email account cannot be loaded");
            _dataProvider.InsertEntity(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.QUANTITY_BELOW_ATTRIBUTE_COMBINATION_VENDOR_NOTIFICATION,
                Subject = "%Store.Name%. Quantity below notification. %Product.Name%\"",
                Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Product.Name% (ID: %Product.ID%) low quantity.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Quantity: %Product.StockQuantity%{Environment.NewLine}<br />{Environment.NewLine}</p>{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id
            });
        }

	_logger.LogDebug("<<< Exiting Up()");
    }

    public override void Down()
    {
		_logger.LogDebug(">>> Entering Down()");
        //add the downgrade logic if necessary 
		_logger.LogDebug("<<< Exiting Down()");
    }
}