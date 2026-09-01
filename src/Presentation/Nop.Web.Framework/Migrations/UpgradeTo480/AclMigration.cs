using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Security;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Migrations.UpgradeTo480;

[NopMigration("2023-11-07 12:00:00", "ACL. Added advanced permissions")]
public class AclMigration : Migration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IRepository<CustomerRole> _customerRoleRepository;
    protected readonly IRepository<Language> _languageRepository;
    protected readonly IRepository<LocaleStringResource> _localeStringRepository;
    protected readonly IRepository<PermissionRecord> _permissionRepository;
    protected readonly IRepository<PermissionRecordCustomerRoleMapping> _permissionRecordCustomerRoleMappingRepository;

    public AclMigration(IRepository<CustomerRole> customerRoleRepository,
        IRepository<Language> languageRepository,
        IRepository<LocaleStringResource> localeStringRepository,
        IRepository<PermissionRecord> permissionRepository,
        IRepository<PermissionRecordCustomerRoleMapping> permissionRecordCustomerRoleMappingRepository)
    {
		_logger.LogDebug(">>> Entering AclMigration(customerRoleRepository={CustomerRoleRepository},languageRepository={LanguageRepository},localeStringRepository={LocaleStringRepository},permissionRepository={PermissionRepository},permissionRecordCustomerRoleMappingRepository=<redacted>)", customerRoleRepository, languageRepository, localeStringRepository, permissionRepository);
        _customerRoleRepository = customerRoleRepository;
		_logger.LogDebug("AclMigration(customerRoleRepository={CustomerRoleRepository},languageRepository={LanguageRepository},localeStringRepository={LocaleStringRepository},permissionRepository={PermissionRepository},permissionRecordCustomerRoleMappingRepository=<redacted>): _customerRoleRepository → {_customerRoleRepository}", customerRoleRepository, languageRepository, localeStringRepository, permissionRepository, _customerRoleRepository);
        _languageRepository = languageRepository;
		_logger.LogDebug("AclMigration(customerRoleRepository={CustomerRoleRepository},languageRepository={LanguageRepository},localeStringRepository={LocaleStringRepository},permissionRepository={PermissionRepository},permissionRecordCustomerRoleMappingRepository=<redacted>): _languageRepository → {_languageRepository}", customerRoleRepository, languageRepository, localeStringRepository, permissionRepository, _languageRepository);
        _localeStringRepository = localeStringRepository;
		_logger.LogDebug("AclMigration(customerRoleRepository={CustomerRoleRepository},languageRepository={LanguageRepository},localeStringRepository={LocaleStringRepository},permissionRepository={PermissionRepository},permissionRecordCustomerRoleMappingRepository=<redacted>): _localeStringRepository → {_localeStringRepository}", customerRoleRepository, languageRepository, localeStringRepository, permissionRepository, _localeStringRepository);
        _permissionRepository = permissionRepository;
		_logger.LogDebug("AclMigration(customerRoleRepository={CustomerRoleRepository},languageRepository={LanguageRepository},localeStringRepository={LocaleStringRepository},permissionRepository={PermissionRepository},permissionRecordCustomerRoleMappingRepository=<redacted>): _permissionRepository → {_permissionRepository}", customerRoleRepository, languageRepository, localeStringRepository, permissionRepository, _permissionRepository);
        _permissionRecordCustomerRoleMappingRepository = permissionRecordCustomerRoleMappingRepository;
		_logger.LogDebug("AclMigration(customerRoleRepository={CustomerRoleRepository},languageRepository={LanguageRepository},localeStringRepository={LocaleStringRepository},permissionRepository={PermissionRepository},permissionRecordCustomerRoleMappingRepository=<redacted>): _permissionRecordCustomerRoleMappingRepository → {_permissionRecordCustomerRoleMappingRepository}", customerRoleRepository, languageRepository, localeStringRepository, permissionRepository, _permissionRecordCustomerRoleMappingRepository);
    }

    /// <summary>
    /// Gets a permission record-customer role mapping
    /// </summary>
    /// <param name="permissionId">Permission identifier</param>
    /// <returns>Permission record-customer role mapping</returns>
    protected virtual IList<PermissionRecordCustomerRoleMapping> GetMappingByPermissionRecordId(int permissionId)
    {
		_logger.LogDebug(">>> Entering GetMappingByPermissionRecordId(permissionId={PermissionId})", permissionId);
        var records = _permissionRecordCustomerRoleMappingRepository.Table
            .Where(x => x.PermissionRecordId == permissionId);

		_logger.LogDebug("<<< Exiting GetMappingByPermissionRecordId(permissionId={PermissionId})", permissionId);
        return records.ToList();
    }

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        var dbPermissions = _permissionRepository.Table
            .OrderBy(pr => pr.Id)
            .ToList();
        
        PermissionRecord getPermissionRecord(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var permissionRecord = dbPermissions
                .FirstOrDefault(pr => pr.SystemName == systemName);

            return permissionRecord;
        }
        
        void insertMappings(string oldPermissionSystemName, params string[] newPermissionSystemNames)
        {
            var record = getPermissionRecord(oldPermissionSystemName);
			_logger.LogDebug("Up(): record → {Record}", record);

            if (record == null) 
                return;

            var roles = GetMappingByPermissionRecordId(record.Id)
                .Select(p => p.CustomerRoleId)
                .ToList();

            foreach (var systemName in newPermissionSystemNames)
            {
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
                var newPermissionRecord = getPermissionRecord(systemName);
				_logger.LogDebug("Up(): newPermissionRecord → {NewPermissionRecord}", newPermissionRecord);

                if (newPermissionRecord == null)
                    continue;

                foreach (var role in roles)
                    try
                    {
                        _permissionRecordCustomerRoleMappingRepository.Insert(
                            new PermissionRecordCustomerRoleMapping
                            {
                                CustomerRoleId = role,
                                PermissionRecordId = newPermissionRecord.Id
                            });
							_logger.LogInformation("Up(): save query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
                    }
                    catch
                    {
						_logger.LogError("Exception in Up(): {Arg0}", ex.Message);
                        //ignore
                    }
            }

            _permissionRepository.Delete(record);
			_logger.LogInformation("Up(): delete query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }
        
        insertMappings("AccessAdminPanel", StandardPermission.Security.ACCESS_ADMIN_PANEL);
        insertMappings("AllowCustomerImpersonation", StandardPermission.Customers.CUSTOMERS_IMPERSONATION);
        insertMappings("ManageProducts", StandardPermission.Catalog.PRODUCTS_VIEW, StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE, StandardPermission.Catalog.PRODUCTS_IMPORT_EXPORT, StandardPermission.Reports.LOW_STOCK);
        insertMappings("ManageCategories", StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE, StandardPermission.Catalog.CATEGORIES_IMPORT_EXPORT, StandardPermission.Catalog.CATEGORIES_VIEW);
        insertMappings("ManageManufacturers", StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE, StandardPermission.Catalog.MANUFACTURER_IMPORT_EXPORT, StandardPermission.Catalog.MANUFACTURER_VIEW);
        insertMappings("ManageProductReviews", StandardPermission.Catalog.PRODUCT_REVIEWS_CREATE_EDIT_DELETE, StandardPermission.Catalog.PRODUCT_REVIEWS_VIEW);
        insertMappings("ManageProductTags", StandardPermission.Catalog.PRODUCT_TAGS_CREATE_EDIT_DELETE, StandardPermission.Catalog.PRODUCT_TAGS_VIEW);
        insertMappings("ManageAttributes", StandardPermission.Catalog.PRODUCT_ATTRIBUTES_CREATE_EDIT_DELETE, StandardPermission.Catalog.PRODUCT_ATTRIBUTES_VIEW, StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_CREATE_EDIT_DELETE, StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_VIEW, StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE, StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW);
        insertMappings("ManageCustomers", StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE, StandardPermission.Customers.CUSTOMERS_IMPORT_EXPORT, StandardPermission.Customers.CUSTOMERS_VIEW, StandardPermission.Customers.CUSTOMER_ROLES_VIEW, StandardPermission.Customers.CUSTOMER_ROLES_CREATE_EDIT_DELETE, StandardPermission.Customers.GDPR_MANAGE, StandardPermission.Reports.REGISTERED_CUSTOMERS, StandardPermission.Reports.CUSTOMERS_BY_NUMBER_OF_ORDERS, StandardPermission.Reports.CUSTOMERS_BY_ORDER_TOTAL);
        insertMappings("ManageVendors", StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE, StandardPermission.Customers.VENDORS_VIEW);
        insertMappings("ManageCurrentCarts", StandardPermission.Orders.CURRENT_CARTS_MANAGE);
        insertMappings("ManageOrders", StandardPermission.Orders.ORDERS_VIEW, StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE, StandardPermission.Orders.ORDERS_IMPORT_EXPORT, StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE, StandardPermission.Orders.SHIPMENTS_VIEW, StandardPermission.Reports.BESTSELLERS, StandardPermission.Reports.PRODUCTS_NEVER_PURCHASED);
        insertMappings("SalesSummaryReport", StandardPermission.Reports.SALES_SUMMARY);
        insertMappings("ManageRecurringPayments", StandardPermission.Orders.RECURRING_PAYMENTS_CREATE_EDIT_DELETE, StandardPermission.Orders.RECURRING_PAYMENTS_VIEW);
        insertMappings("ManageGiftCards", StandardPermission.Orders.GIFT_CARDS_CREATE_EDIT_DELETE, StandardPermission.Orders.GIFT_CARDS_VIEW);
        insertMappings("ManageReturnRequests", StandardPermission.Orders.RETURN_REQUESTS_CREATE_EDIT_DELETE, StandardPermission.Orders.RETURN_REQUESTS_VIEW);
        insertMappings("OrderCountryReport", StandardPermission.Reports.COUNTRY_SALES);
        insertMappings("ManageAffiliates", StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE, StandardPermission.Promotions.AFFILIATES_VIEW);
        insertMappings("ManageCampaigns", StandardPermission.Promotions.CAMPAIGNS_CREATE_EDIT, StandardPermission.Promotions.CAMPAIGNS_DELETE, StandardPermission.Promotions.CAMPAIGNS_SEND_EMAILS, StandardPermission.Promotions.CAMPAIGNS_VIEW);
        insertMappings("ManageDiscounts", StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE, StandardPermission.Promotions.DISCOUNTS_VIEW);
        insertMappings("ManageNewsletterSubscribers", StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE, StandardPermission.Promotions.SUBSCRIBERS_IMPORT_EXPORT, StandardPermission.Promotions.SUBSCRIBERS_VIEW);
        insertMappings("ManagePolls", StandardPermission.ContentManagement.POLLS_CREATE_EDIT_DELETE, StandardPermission.ContentManagement.POLLS_VIEW);
        insertMappings("ManageNews", StandardPermission.ContentManagement.NEWS_COMMENTS_CREATE_EDIT_DELETE, StandardPermission.ContentManagement.NEWS_COMMENTS_VIEW, StandardPermission.ContentManagement.NEWS_CREATE_EDIT_DELETE, StandardPermission.ContentManagement.NEWS_VIEW);
        insertMappings("ManageBlog", StandardPermission.ContentManagement.BLOG_COMMENTS_CREATE_EDIT_DELETE, StandardPermission.ContentManagement.BLOG_COMMENTS_VIEW, StandardPermission.ContentManagement.BLOG_CREATE_EDIT_DELETE, StandardPermission.ContentManagement.BLOG_VIEW);
        insertMappings("ManageWidgets", StandardPermission.Configuration.MANAGE_WIDGETS);
        insertMappings("ManageTopics", StandardPermission.ContentManagement.TOPICS_CREATE_EDIT_DELETE, StandardPermission.ContentManagement.TOPICS_VIEW);
        insertMappings("ManageForums", StandardPermission.ContentManagement.FORUMS_CREATE_EDIT_DELETE, StandardPermission.ContentManagement.FORUMS_VIEW);
        insertMappings("ManageMessageTemplates", StandardPermission.ContentManagement.MESSAGE_TEMPLATES_CREATE_EDIT_DELETE, StandardPermission.ContentManagement.MESSAGE_TEMPLATES_VIEW);
        insertMappings("ManageCountries", StandardPermission.Configuration.MANAGE_COUNTRIES);
        insertMappings("ManageLanguages", StandardPermission.Configuration.MANAGE_LANGUAGES);
        insertMappings("ManageSettings", StandardPermission.Configuration.MANAGE_SETTINGS);
        insertMappings("ManagePaymentMethods", StandardPermission.Configuration.MANAGE_PAYMENT_METHODS);
        insertMappings("ManageExternalAuthenticationMethods", StandardPermission.Configuration.MANAGE_EXTERNAL_AUTHENTICATION_METHODS);
        insertMappings("ManageMultifactorAuthenticationMethods", StandardPermission.Configuration.MANAGE_MULTIFACTOR_AUTHENTICATION_METHODS);
        insertMappings("ManageTaxSettings", StandardPermission.Configuration.MANAGE_TAX_SETTINGS);
        insertMappings("ManageShippingSettings", StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS);
        insertMappings("ManageCurrencies", StandardPermission.Configuration.MANAGE_CURRENCIES);
        insertMappings("ManageActivityLog", StandardPermission.Customers.ACTIVITY_LOG_DELETE, StandardPermission.Customers.ACTIVITY_LOG_MANAGE_TYPES, StandardPermission.Customers.ACTIVITY_LOG_VIEW);
        insertMappings("ManageACL", StandardPermission.Configuration.MANAGE_ACL);
        insertMappings("ManageEmailAccounts", StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS);
        insertMappings("ManageStores", StandardPermission.Configuration.MANAGE_STORES);
        insertMappings("ManagePlugins", StandardPermission.Configuration.MANAGE_PLUGINS);
        insertMappings("ManageSystemLog", StandardPermission.System.MANAGE_SYSTEM_LOG);
        insertMappings("ManageMessageQueue", StandardPermission.System.MANAGE_MESSAGE_QUEUE);
        insertMappings("ManageMaintenance", StandardPermission.System.MANAGE_MAINTENANCE);
        insertMappings("HtmlEditor.ManagePictures", StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES);
        insertMappings("ManageScheduleTasks", StandardPermission.System.MANAGE_SCHEDULE_TASKS);
        insertMappings("ManageAppSettings", StandardPermission.System.MANAGE_APP_SETTINGS);

        insertMappings("DisplayPrices", StandardPermission.PublicStore.DISPLAY_PRICES);
        insertMappings("EnableShoppingCart", StandardPermission.PublicStore.ENABLE_SHOPPING_CART);
        insertMappings("EnableWishlist", StandardPermission.PublicStore.ENABLE_WISHLIST);
        insertMappings("PublicStoreAllowNavigation", StandardPermission.PublicStore.PUBLIC_STORE_ALLOW_NAVIGATION);
        insertMappings("AccessClosedStore", StandardPermission.PublicStore.ACCESS_CLOSED_STORE);

        insertMappings("EnableMultiFactorAuthentication", StandardPermission.Security.ENABLE_MULTI_FACTOR_AUTHENTICATION);

        //add new permissions for vendors
        var vendorRole = _customerRoleRepository.Table
            .FirstOrDefault(x => x.IsSystemRole && x.SystemName == NopCustomerDefaults.VendorsRoleName);

        if (vendorRole is not null)
        {
            var viewDiscountsPermission = _permissionRepository.Table.FirstOrDefault(x => x.SystemName == StandardPermission.Promotions.DISCOUNTS_VIEW);
			_logger.LogDebug("Up(): viewDiscountsPermission → {ViewDiscountsPermission}", viewDiscountsPermission);
            if (viewDiscountsPermission is not null && !_permissionRecordCustomerRoleMappingRepository.Table.Any(x => x.CustomerRoleId == vendorRole.Id && x.PermissionRecordId == viewDiscountsPermission.Id))
            {
                _permissionRecordCustomerRoleMappingRepository.Insert(
                        new PermissionRecordCustomerRoleMapping
                        {
                            CustomerRoleId = vendorRole.Id,
                            PermissionRecordId = viewDiscountsPermission.Id
                        });
            }

            var crudDiscountsPermission = _permissionRepository.Table.FirstOrDefault(x => x.SystemName == StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE);
			_logger.LogDebug("Up(): crudDiscountsPermission → {CrudDiscountsPermission}", crudDiscountsPermission);
            if (crudDiscountsPermission is not null && !_permissionRecordCustomerRoleMappingRepository.Table.Any(x => x.CustomerRoleId == vendorRole.Id && x.PermissionRecordId == crudDiscountsPermission.Id))
            {
                _permissionRecordCustomerRoleMappingRepository.Insert(
                        new PermissionRecordCustomerRoleMapping
                        {
                            CustomerRoleId = vendorRole.Id,
                            PermissionRecordId = crudDiscountsPermission.Id
                        });
            }

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