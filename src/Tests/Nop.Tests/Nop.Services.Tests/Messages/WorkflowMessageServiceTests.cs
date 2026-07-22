using FluentAssertions;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Vendors;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Messages;

[TestFixture]
public class WorkflowMessageServiceTests : ServiceTest
{
    private readonly IWorkflowMessageService _workflowMessageService;

    private readonly List<int> _notActiveTempletes = new();
    private readonly IMessageTemplateService _messageTemplateService;
    private Customer _customer;
    private readonly IRepository<QueuedEmail> _queuedEmailRepository;
    private Order _order;
    private Vendor _vendor;
    private Shipment _shipment;
    private IList<MessageTemplate> _allMessageTemplates;
    private OrderNote _orderNote;
    private RecurringPayment _recurringPayment;
    private NewsLetterSubscription _subscription;
    private Product _product;
    private OrderItem _orderItem;
    private ReturnRequest _returnRequest;
    private Forum _forum;
    private ForumTopic _forumTopic;
    private ForumPost _forumPost;
    private PrivateMessage _privateMessage;
    private ProductReview _productReview;
    private GiftCard _giftCard;
    private BlogComment _blogComment;
    private NewsComment _newsComment;
    private BackInStockSubscription _backInStockSubscription;
    private readonly IForumService _forumService;

    public WorkflowMessageServiceTests()
    {
		logger.LogDebug(">>> Entering WorkflowMessageServiceTests()");
        _workflowMessageService = GetService<IWorkflowMessageService>();
        _messageTemplateService = GetService<IMessageTemplateService>();
		logger.LogDebug("WorkflowMessageServiceTests(): _workflowMessageService → {_workflowMessageService}", _workflowMessageService);
		logger.LogDebug("WorkflowMessageServiceTests(): _messageTemplateService → {_messageTemplateService}", _messageTemplateService);
        _queuedEmailRepository = GetService<IRepository<QueuedEmail>>();
        _forumService = GetService<IForumService>();
		logger.LogDebug("<<< Exiting WorkflowMessageServiceTests()");
		logger.LogDebug("WorkflowMessageServiceTests(): _queuedEmailRepository → {_queuedEmailRepository}", _queuedEmailRepository);
		logger.LogDebug("WorkflowMessageServiceTests(): _forumService → {_forumService}", _forumService);
    }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var customerService = GetService<ICustomerService>();
        var orderService = GetService<IOrderService>();
		logger.LogDebug("OneTimeSetUp(): customerService → {CustomerService}", customerService);
		logger.LogDebug("OneTimeSetUp(): orderService → {OrderService}", orderService);
        var vendorService = GetService<IVendorService>();
        var shipmentService = GetService<IShipmentService>();
		logger.LogDebug("OneTimeSetUp(): vendorService → {VendorService}", vendorService);
		logger.LogDebug("OneTimeSetUp(): shipmentService → {ShipmentService}", shipmentService);
        var productService = GetService<IProductService>();
        var productReviewService = GetService<IProductReviewService>();
		logger.LogDebug("OneTimeSetUp(): productService → {ProductService}", productService);
		logger.LogDebug("OneTimeSetUp(): productReviewService → {ProductReviewService}", productReviewService);
        var giftCardService = GetService<IGiftCardService>();
        var blogService = GetService<IBlogService>();
		logger.LogDebug("OneTimeSetUp(): giftCardService → {GiftCardService}", giftCardService);
		logger.LogDebug("OneTimeSetUp(): blogService → {BlogService}", blogService);
        var newsService = GetService<INewsService>();

		logger.LogDebug("OneTimeSetUp(): newsService → {NewsService}", newsService);
        _order = await orderService.GetOrderByIdAsync(1);
		logger.LogDebug("OneTimeSetUp(): _order → {_order}", _order);
        _orderItem = (await orderService.GetOrderItemsAsync(1)).First();
        _customer = await customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
		logger.LogDebug("OneTimeSetUp(): _orderItem → {_orderItem}", _orderItem);
		logger.LogDebug("OneTimeSetUp(): _customer → {_customer}", _customer);
        _vendor = await vendorService.GetVendorByIdAsync(1);
        _shipment = await shipmentService.GetShipmentByIdAsync(1);
		logger.LogDebug("OneTimeSetUp(): _vendor → {_vendor}", _vendor);
		logger.LogDebug("OneTimeSetUp(): _shipment → {_shipment}", _shipment);
        _orderNote = await orderService.GetOrderNoteByIdAsync(1);
        _recurringPayment = new RecurringPayment { InitialOrderId = _order.Id, IsActive = true };
		logger.LogDebug("OneTimeSetUp(): _orderNote → {_orderNote}", _orderNote);
        _subscription = new NewsLetterSubscription { Active = true, Email = NopTestsDefaults.AdminEmail, LanguageId = 1 };
        _product = await productService.GetProductByIdAsync(1);
        _returnRequest = new ReturnRequest { CustomerId = _customer.Id, OrderItemId = _orderItem.Id };
		logger.LogDebug("OneTimeSetUp(): _product → {_product}", _product);
		logger.LogDebug("OneTimeSetUp(): _recurringPayment → {_recurringPayment}", _recurringPayment);
		logger.LogDebug("OneTimeSetUp(): _subscription → {_subscription}", _subscription);
		logger.LogDebug("OneTimeSetUp(): _returnRequest → {_returnRequest}", _returnRequest);
        _forum = await _forumService.GetForumByIdAsync(1);
        _forumTopic = new ForumTopic { CustomerId = _customer.Id, ForumId = _forum.Id, Subject = "Subject" };
		logger.LogDebug("OneTimeSetUp(): _forum → {_forum}", _forum);
		logger.LogDebug("OneTimeSetUp(): _forumTopic → {_forumTopic}", _forumTopic);
        await _forumService.InsertTopicAsync(_forumTopic, false);
        _forumPost = new ForumPost { CustomerId = _customer.Id, TopicId = _forumTopic.Id, Text = "Text" };
        await _forumService.InsertPostAsync(_forumPost, false);
		logger.LogDebug("OneTimeSetUp(): _forumPost → {_forumPost}", _forumPost);

        _privateMessage = new PrivateMessage
        {
            FromCustomerId = 1,
            ToCustomerId = 2,
            Subject = string.Empty,
            Text = string.Empty
			logger.LogDebug("OneTimeSetUp(): _privateMessage → {_privateMessage}", _privateMessage);
        };
        _productReview = (await productReviewService.GetAllProductReviewsAsync()).FirstOrDefault();
        _giftCard = await GetService<INopDataProvider>().InsertEntityAsync(new GiftCard
        {
		logger.LogDebug("OneTimeSetUp(): _productReview → {_productReview}", _productReview);
		logger.LogDebug("OneTimeSetUp(): _giftCard → {_giftCard}", _giftCard);
            GiftCardType = GiftCardType.Virtual,
            PurchasedWithOrderItemId = 3,
            Amount = 25M,
            IsGiftCardActivated = false,
            GiftCardCouponCode = string.Empty,
            RecipientName = "Brenda Lindgren",
            RecipientEmail = "brenda_lindgren@nopCommerce.com",
            SenderName = "Steve Gates",
            SenderEmail = "steve_gates@nopCommerce.com",
            Message = string.Empty,
            IsRecipientNotified = false,
            CreatedOnUtc = DateTime.UtcNow
        });

        _blogComment = await blogService.GetBlogCommentByIdAsync(1);
		logger.LogDebug("OneTimeSetUp(): _blogComment → {_blogComment}", _blogComment);
        _newsComment = await newsService.GetNewsCommentByIdAsync(1);
        _backInStockSubscription = new BackInStockSubscription { ProductId = _product.Id, CustomerId = _customer.Id };
		logger.LogDebug("OneTimeSetUp(): _newsComment → {_newsComment}", _newsComment);
		logger.LogDebug("OneTimeSetUp(): _backInStockSubscription → {_backInStockSubscription}", _backInStockSubscription);

        _allMessageTemplates = await _messageTemplateService.GetAllMessageTemplatesAsync(0);

		logger.LogDebug("OneTimeSetUp(): _allMessageTemplates → {_allMessageTemplates}", _allMessageTemplates);
        foreach (var template in _allMessageTemplates.Where(t => !t.IsActive))
        {
            template.IsActive = true;
			logger.LogDebug("OneTimeSetUp(): template.IsActive → {Arg0}", template.IsActive);
            _notActiveTempletes.Add(template.Id);
            await _messageTemplateService.UpdateMessageTemplateAsync(template);
        }
		logger.LogDebug("<<< Exiting OneTimeSetUp()");
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
		logger.LogDebug(">>> Entering OneTimeTearDown()");
        foreach (var template in _allMessageTemplates.Where(t => _notActiveTempletes.Contains(t.Id)))
        {
            template.IsActive = false;
			logger.LogDebug("OneTimeTearDown(): template.IsActive → {Arg0}", template.IsActive);
            await _messageTemplateService.UpdateMessageTemplateAsync(template);
        }

        await _forumService.DeletePostAsync(_forumPost);
        await _forumService.DeleteTopicAsync(_forumTopic);
		logger.LogDebug("<<< Exiting OneTimeTearDown()");
    }

    [SetUp]
    public async Task SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _queuedEmailRepository.TruncateAsync();
		logger.LogDebug("<<< Exiting SetUp()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("SetUp(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("SetUp(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
    protected async Task CheckData(Func<Task<IList<int>>> func)
    {
		logger.LogDebug(">>> Entering CheckData(func={Func})", func);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
        queuedEmails.Count.Should().Be(0);

		logger.LogDebug("CheckData(func={Func}): queuedEmails → {QueuedEmails}", func, queuedEmails);
		logger.LogInformation("CheckData(func)={Func}: find query executed in {Arg1} ms", func, __logStart.ElapsedMilliseconds);
        var emailIds = await func();

		logger.LogDebug("CheckData(func={Func}): emailIds → {EmailIds}", func, emailIds);
        emailIds.Count.Should().BeGreaterThan(0);

        queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
        queuedEmails.Count.Should().Be(emailIds.Count);
		logger.LogDebug("<<< Exiting CheckData(func={Func})", func);
    }

    #region Customer workflow

    [Test]
    public async Task CanSendCustomerRegisteredNotificationMessage()
    {
		logger.LogDebug(">>> Entering CanSendCustomerRegisteredNotificationMessage()");
        await CheckData(async () =>
            await _workflowMessageService.SendCustomerRegisteredStoreOwnerNotificationMessageAsync(_customer, 1));
			logger.LogDebug("<<< Exiting CanSendCustomerRegisteredNotificationMessage()");
    }

    [Test]
    public async Task CanSendCustomerWelcomeMessage()
    {
		logger.LogDebug(">>> Entering CanSendCustomerWelcomeMessage()");
        await CheckData(async () =>
            await _workflowMessageService.SendCustomerWelcomeMessageAsync(_customer, 1));
			logger.LogDebug("<<< Exiting CanSendCustomerWelcomeMessage()");
    }

    [Test]
    public async Task CanSendCustomerEmailValidationMessage()
    {
		logger.LogDebug(">>> Entering CanSendCustomerEmailValidationMessage()");
        await CheckData(async () =>
            await _workflowMessageService.SendCustomerEmailValidationMessageAsync(_customer, 1));
			logger.LogDebug("<<< Exiting CanSendCustomerEmailValidationMessage()");
    }

    [Test]
    public async Task CanSendCustomerEmailRevalidationMessage()
    {
		logger.LogDebug(">>> Entering CanSendCustomerEmailRevalidationMessage()");
        _customer.EmailToRevalidate = NopTestsDefaults.AdminEmail;
        await CheckData(async () =>
            await _workflowMessageService.SendCustomerEmailRevalidationMessageAsync(_customer, 1));
			logger.LogDebug("CanSendCustomerEmailRevalidationMessage(): _customer.EmailToRevalidate → {Arg0}", _customer.EmailToRevalidate);
    }

    [Test]
    public async Task CanSendCustomerPasswordRecoveryMessage()
    {
		logger.LogDebug(">>> Entering CanSendCustomerPasswordRecoveryMessage()");
        await CheckData(async () =>
            await _workflowMessageService.SendCustomerPasswordRecoveryMessageAsync(_customer, 1));
			logger.LogDebug("<<< Exiting CanSendCustomerPasswordRecoveryMessage()");
    }

    #endregion

    #region Order workflow

    [Test]
    public async Task CanSendOrderPlacedVendorNotification()
    {
		logger.LogDebug(">>> Entering CanSendOrderPlacedVendorNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendOrderPlacedVendorNotificationAsync(_order, _vendor, 1));
			logger.LogDebug("<<< Exiting CanSendOrderPlacedVendorNotification()");
    }

    [Test]
    public async Task CanSendOrderPlacedStoreOwnerNotification()
    {
		logger.LogDebug(">>> Entering CanSendOrderPlacedStoreOwnerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendOrderPlacedStoreOwnerNotificationAsync(_order, 1));
			logger.LogDebug("<<< Exiting CanSendOrderPlacedStoreOwnerNotification()");
    }

    [Test]
    public async Task CanSendOrderPlacedAffiliateNotification()
    {
		logger.LogDebug(">>> Entering CanSendOrderPlacedAffiliateNotification()");
        _order.AffiliateId = 1;
        await CheckData(async () =>
            await _workflowMessageService.SendOrderPlacedAffiliateNotificationAsync(_order, 1));
			logger.LogDebug("CanSendOrderPlacedAffiliateNotification(): _order.AffiliateId → {Arg0}", _order.AffiliateId);
    }

    [Test]
    public async Task CanSendOrderPaidStoreOwnerNotification()
    {
		logger.LogDebug(">>> Entering CanSendOrderPaidStoreOwnerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendOrderPaidStoreOwnerNotificationAsync(_order, 1));
			logger.LogDebug("<<< Exiting CanSendOrderPaidStoreOwnerNotification()");
    }

    [Test]
    public async Task CanSendOrderPaidCustomerNotification()
    {
		logger.LogDebug(">>> Entering CanSendOrderPaidCustomerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendOrderPaidCustomerNotificationAsync(_order, 1));
			logger.LogDebug("<<< Exiting CanSendOrderPaidCustomerNotification()");
    }

    [Test]
    public async Task CanSendOrderPaidVendorNotification()
    {
		logger.LogDebug(">>> Entering CanSendOrderPaidVendorNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendOrderPaidVendorNotificationAsync(_order, _vendor, 1));
			logger.LogDebug("<<< Exiting CanSendOrderPaidVendorNotification()");
    }

    [Test]
    public async Task CanSendOrderPaidAffiliateNotification()
    {
		logger.LogDebug(">>> Entering CanSendOrderPaidAffiliateNotification()");
        _order.AffiliateId = 1;
        await CheckData(async () =>
            await _workflowMessageService.SendOrderPaidAffiliateNotificationAsync(_order, 1));
			logger.LogDebug("CanSendOrderPaidAffiliateNotification(): _order.AffiliateId → {Arg0}", _order.AffiliateId);
    }

    [Test]
    public async Task CanSendOrderPlacedCustomerNotification()
    {
		logger.LogDebug(">>> Entering CanSendOrderPlacedCustomerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendOrderPlacedCustomerNotificationAsync(_order, 1));
			logger.LogDebug("<<< Exiting CanSendOrderPlacedCustomerNotification()");
    }

    [Test]
    public async Task CanSendShipmentSentCustomerNotification()
    {
		logger.LogDebug(">>> Entering CanSendShipmentSentCustomerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendShipmentSentCustomerNotificationAsync(_shipment, 1));
			logger.LogDebug("<<< Exiting CanSendShipmentSentCustomerNotification()");
    }
    [Test]
    public async Task CanSendShipmentDeliveredCustomerNotification()
    {
		logger.LogDebug(">>> Entering CanSendShipmentDeliveredCustomerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendShipmentDeliveredCustomerNotificationAsync(_shipment, 1));
			logger.LogDebug("<<< Exiting CanSendShipmentDeliveredCustomerNotification()");
    }

    [Test]
    public async Task CanSendOrderCompletedCustomerNotification()
    {
		logger.LogDebug(">>> Entering CanSendOrderCompletedCustomerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendOrderCompletedCustomerNotificationAsync(_order, 1));
			logger.LogDebug("<<< Exiting CanSendOrderCompletedCustomerNotification()");
    }

    [Test]
    public async Task CanSendOrderCompletedStoreOwnerNotification()
    {
		logger.LogDebug(">>> Entering CanSendOrderCompletedStoreOwnerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendOrderCompletedStoreOwnerNotificationAsync(_order, 1));
			logger.LogDebug("<<< Exiting CanSendOrderCompletedStoreOwnerNotification()");
    }

    [Test]
    public async Task CanSendOrderCancelledCustomerNotification()
    {
		logger.LogDebug(">>> Entering CanSendOrderCancelledCustomerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendOrderCancelledCustomerNotificationAsync(_order, 1));
			logger.LogDebug("<<< Exiting CanSendOrderCancelledCustomerNotification()");
    }

    [Test]
    public async Task CanSendOrderRefundedStoreOwnerNotification()
    {
		logger.LogDebug(">>> Entering CanSendOrderRefundedStoreOwnerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendOrderRefundedStoreOwnerNotificationAsync(_order, 1M, 1));
			logger.LogDebug("<<< Exiting CanSendOrderRefundedStoreOwnerNotification()");
    }

    [Test]
    public async Task CanSendOrderRefundedCustomerNotification()
    {
		logger.LogDebug(">>> Entering CanSendOrderRefundedCustomerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendOrderRefundedCustomerNotificationAsync(_order, 1M, 1));
			logger.LogDebug("<<< Exiting CanSendOrderRefundedCustomerNotification()");
    }

    [Test]
    public async Task CanSendNewOrderNoteAddedCustomerNotification()
    {
		logger.LogDebug(">>> Entering CanSendNewOrderNoteAddedCustomerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendNewOrderNoteAddedCustomerNotificationAsync(_orderNote, 1));
			logger.LogDebug("<<< Exiting CanSendNewOrderNoteAddedCustomerNotification()");
    }

    [Test]
    public async Task CanSendRecurringPaymentCancelledStoreOwnerNotification()
    {
		logger.LogDebug(">>> Entering CanSendRecurringPaymentCancelledStoreOwnerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendRecurringPaymentCancelledStoreOwnerNotificationAsync(_recurringPayment, 1));
			logger.LogDebug("<<< Exiting CanSendRecurringPaymentCancelledStoreOwnerNotification()");
    }

    [Test]
    public async Task CanSendRecurringPaymentCancelledCustomerNotification()
    {
		logger.LogDebug(">>> Entering CanSendRecurringPaymentCancelledCustomerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendRecurringPaymentCancelledCustomerNotificationAsync(_recurringPayment, 1));
			logger.LogDebug("<<< Exiting CanSendRecurringPaymentCancelledCustomerNotification()");
    }

    [Test]
    public async Task CanSendRecurringPaymentFailedCustomerNotification()
    {
		logger.LogDebug(">>> Entering CanSendRecurringPaymentFailedCustomerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendRecurringPaymentFailedCustomerNotificationAsync(_recurringPayment, 1));
			logger.LogDebug("<<< Exiting CanSendRecurringPaymentFailedCustomerNotification()");
    }

    #endregion

    #region Newsletter workflow

    [Test]
    public async Task CanSendNewsLetterSubscriptionActivationMessage()
    {
		logger.LogDebug(">>> Entering CanSendNewsLetterSubscriptionActivationMessage()");
        await CheckData(async () =>
            await _workflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(_subscription));
			logger.LogDebug("<<< Exiting CanSendNewsLetterSubscriptionActivationMessage()");
    }

    [Test]
    public async Task CanSendNewsLetterSubscriptionDeactivationMessage()
    {
		logger.LogDebug(">>> Entering CanSendNewsLetterSubscriptionDeactivationMessage()");
        await CheckData(async () =>
            await _workflowMessageService.SendNewsLetterSubscriptionDeactivationMessageAsync(_subscription));
			logger.LogDebug("<<< Exiting CanSendNewsLetterSubscriptionDeactivationMessage()");
    }

    #endregion

    #region Send a message to a friend

    [Test]
    public async Task CanSendProductEmailAFriendMessage()
    {
		logger.LogDebug(">>> Entering CanSendProductEmailAFriendMessage()");
        await CheckData(async () =>
            await _workflowMessageService.SendProductEmailAFriendMessageAsync(_customer, 1, _product, NopTestsDefaults.AdminEmail, NopTestsDefaults.AdminEmail, string.Empty));
			logger.LogDebug("<<< Exiting CanSendProductEmailAFriendMessage()");
    }

    [Test]
    public async Task CanSendWishlistEmailAFriendMessage()
    {
		logger.LogDebug(">>> Entering CanSendWishlistEmailAFriendMessage()");
        await CheckData(async () =>
            await _workflowMessageService.SendWishlistEmailAFriendMessageAsync(_customer, 1, NopTestsDefaults.AdminEmail, NopTestsDefaults.AdminEmail, string.Empty, string.Empty));
			logger.LogDebug("<<< Exiting CanSendWishlistEmailAFriendMessage()");
    }

    #endregion

    #region Return requests

    [Test]
    public async Task CanSendNewReturnRequestStoreOwnerNotification()
    {
		logger.LogDebug(">>> Entering CanSendNewReturnRequestStoreOwnerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendNewReturnRequestStoreOwnerNotificationAsync(_returnRequest, _orderItem, _order, 1));
			logger.LogDebug("<<< Exiting CanSendNewReturnRequestStoreOwnerNotification()");
    }

    [Test]
    public async Task CanSendNewReturnRequestCustomerNotification()
    {
		logger.LogDebug(">>> Entering CanSendNewReturnRequestCustomerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendNewReturnRequestCustomerNotificationAsync(_returnRequest, _orderItem, _order));
			logger.LogDebug("<<< Exiting CanSendNewReturnRequestCustomerNotification()");
    }

    [Test]
    public async Task CanSendReturnRequestStatusChangedCustomerNotification()
    {
		logger.LogDebug(">>> Entering CanSendReturnRequestStatusChangedCustomerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendReturnRequestStatusChangedCustomerNotificationAsync(_returnRequest, _orderItem, _order));
			logger.LogDebug("<<< Exiting CanSendReturnRequestStatusChangedCustomerNotification()");
    }

    #endregion

    #region Forum Notifications

    [Test]
    public async Task CanSendNewForumTopicMessage()
    {
		logger.LogDebug(">>> Entering CanSendNewForumTopicMessage()");
        await CheckData(async () =>
            await _workflowMessageService.SendNewForumTopicMessageAsync(_customer, _forumTopic, _forum, 1));
			logger.LogDebug("<<< Exiting CanSendNewForumTopicMessage()");
    }

    [Test]
    public async Task CanSendNewForumPostMessage()
    {
		logger.LogDebug(">>> Entering CanSendNewForumPostMessage()");
        await CheckData(async () =>
            await _workflowMessageService.SendNewForumPostMessageAsync(_customer, _forumPost, _forumTopic, _forum, 1, 1));
			logger.LogDebug("<<< Exiting CanSendNewForumPostMessage()");
    }

    [Test]
    public async Task CanSendPrivateMessageNotification()
    {
		logger.LogDebug(">>> Entering CanSendPrivateMessageNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendPrivateMessageNotificationAsync(_privateMessage, 1));
			logger.LogDebug("<<< Exiting CanSendPrivateMessageNotification()");
    }

    #endregion

    #region Misc

    [Test]
    public async Task CanSendNewVendorAccountApplyStoreOwnerNotification()
    {
		logger.LogDebug(">>> Entering CanSendNewVendorAccountApplyStoreOwnerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendNewVendorAccountApplyStoreOwnerNotificationAsync(_customer, _vendor, 1));
			logger.LogDebug("<<< Exiting CanSendNewVendorAccountApplyStoreOwnerNotification()");
    }

    [Test]
    public async Task CanSendVendorInformationChangeNotification()
    {
		logger.LogDebug(">>> Entering CanSendVendorInformationChangeNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendVendorInformationChangeStoreOwnerNotificationAsync(_vendor, 1));
			logger.LogDebug("<<< Exiting CanSendVendorInformationChangeNotification()");
    }

    [Test]
    public async Task CanSendProductReviewNotificationMessage()
    {
		logger.LogDebug(">>> Entering CanSendProductReviewNotificationMessage()");
        await CheckData(async () =>
            await _workflowMessageService.SendProductReviewStoreOwnerNotificationMessageAsync(_productReview, 1));
			logger.LogDebug("<<< Exiting CanSendProductReviewNotificationMessage()");
    }

    [Test]
    public async Task CanSendProductReviewReplyCustomerNotificationMessage()
    {
		logger.LogDebug(">>> Entering CanSendProductReviewReplyCustomerNotificationMessage()");
        await CheckData(async () =>
            await _workflowMessageService.SendProductReviewReplyCustomerNotificationMessageAsync(_productReview, 1));
			logger.LogDebug("<<< Exiting CanSendProductReviewReplyCustomerNotificationMessage()");
    }

    [Test]
    public async Task CanSendGiftCardNotification()
    {
		logger.LogDebug(">>> Entering CanSendGiftCardNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendGiftCardNotificationAsync(_giftCard, 1));
			logger.LogDebug("<<< Exiting CanSendGiftCardNotification()");
    }

    [Test]
    public async Task CanSendQuantityBelowStoreOwnerNotification()
    {
		logger.LogDebug(">>> Entering CanSendQuantityBelowStoreOwnerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendQuantityBelowStoreOwnerNotificationAsync(_product, 1));
			logger.LogDebug("<<< Exiting CanSendQuantityBelowStoreOwnerNotification()");
    }

    [Test]
    public async Task CanSendNewVatSubmittedStoreOwnerNotification()
    {
		logger.LogDebug(">>> Entering CanSendNewVatSubmittedStoreOwnerNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendNewVatSubmittedStoreOwnerNotificationAsync(_customer, "vat name", "vat address", 1));
			logger.LogDebug("<<< Exiting CanSendNewVatSubmittedStoreOwnerNotification()");
    }

    [Test]
    public async Task CanSendBlogCommentNotificationMessage()
    {
		logger.LogDebug(">>> Entering CanSendBlogCommentNotificationMessage()");
        await CheckData(async () =>
            await _workflowMessageService.SendBlogCommentStoreOwnerNotificationMessageAsync(_blogComment, 1));
			logger.LogDebug("<<< Exiting CanSendBlogCommentNotificationMessage()");
    }

    [Test]
    public async Task CanSendNewsCommentNotificationMessage()
    {
		logger.LogDebug(">>> Entering CanSendNewsCommentNotificationMessage()");
        await CheckData(async () =>
            await _workflowMessageService.SendNewsCommentStoreOwnerNotificationMessageAsync(_newsComment, 1));
			logger.LogDebug("<<< Exiting CanSendNewsCommentNotificationMessage()");
    }

    [Test]
    public async Task CanSendBackInStockNotification()
    {
		logger.LogDebug(">>> Entering CanSendBackInStockNotification()");
        await CheckData(async () =>
            await _workflowMessageService.SendBackInStockNotificationAsync(_backInStockSubscription, 1));
			logger.LogDebug("<<< Exiting CanSendBackInStockNotification()");
    }

    [Test]
    public async Task CanSendContactUsMessage()
    {
		logger.LogDebug(">>> Entering CanSendContactUsMessage()");
        await CheckData(async () =>
            await _workflowMessageService.SendContactUsMessageAsync(1, NopTestsDefaults.AdminEmail, "sender name", "subject", "body"));
			logger.LogDebug("<<< Exiting CanSendContactUsMessage()");
    }

    [Test]
    public async Task CanSendContactVendorMessage()
    {
		logger.LogDebug(">>> Entering CanSendContactVendorMessage()");
        await CheckData(async () =>
            await _workflowMessageService.SendContactVendorMessageAsync(_vendor, 1, NopTestsDefaults.AdminEmail, "sender name", "subject", "body"));
			logger.LogDebug("<<< Exiting CanSendContactVendorMessage()");
    }

    #endregion
}
