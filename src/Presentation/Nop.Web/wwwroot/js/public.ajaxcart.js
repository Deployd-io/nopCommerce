var logger = (typeof logger !== 'undefined' && logger) || console;
/*
** nopCommerce ajax cart implementation
*/


var AjaxCart = {
    loadWaiting: false,
    usepopupnotifications: false,
    topcartselector: '',
    topwishlistselector: '',
    flyoutcartselector: '',
    localized_data: false,

    init: function (usepopupnotifications, topcartselector, topwishlistselector, flyoutcartselector, localized_data) {
		logger.error(`>>> Entering init(usepopupnotifications=${usepopupnotifications},topcartselector=${topcartselector},topwishlistselector=${topwishlistselector},flyoutcartselector=${flyoutcartselector},localized_data=${localized_data})`);
        this.loadWaiting = false;
        this.usepopupnotifications = usepopupnotifications;
        this.topcartselector = topcartselector;
		logger.debug(`init(usepopupnotifications=${usepopupnotifications},topcartselector=${topcartselector},topwishlistselector=${topwishlistselector},flyoutcartselector=${flyoutcartselector},localized_data=${localized_data}): this.loadWaiting → ${this.loadWaiting}`);
		logger.debug(`init(usepopupnotifications=${usepopupnotifications},topcartselector=${topcartselector},topwishlistselector=${topwishlistselector},flyoutcartselector=${flyoutcartselector},localized_data=${localized_data}): usepopupnotifications → ${usepopupnotifications}`);
		logger.debug(`init(usepopupnotifications=${usepopupnotifications},topcartselector=${topcartselector},topwishlistselector=${topwishlistselector},flyoutcartselector=${flyoutcartselector},localized_data=${localized_data}): topcartselector → ${topcartselector}`);
		logger.debug(`init(usepopupnotifications=${usepopupnotifications},topcartselector=${topcartselector},topwishlistselector=${topwishlistselector},flyoutcartselector=${flyoutcartselector},localized_data=${localized_data}): this.usepopupnotifications → ${this.usepopupnotifications}`);
        this.topwishlistselector = topwishlistselector;
        this.flyoutcartselector = flyoutcartselector;
        this.localized_data = localized_data;
    },

    setLoadWaiting: function (display) {
		logger.error(`>>> Entering setLoadWaiting(display=${display})`);
        displayAjaxLoading(display);
        this.loadWaiting = display;
		logger.warn(`<<< Exiting setLoadWaiting(display=${display})`);
  },

    //move a shopping cart item to the custom wishlist
    moveToCustomWishlist: function (urlmove, itemId, wishlistId) {
		logger.error(`>>> Entering moveToCustomWishlist(urlmove=${urlmove},itemId=${itemId},wishlistId=${wishlistId})`);
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);

        var postData = {
          shoppingCartItemId: itemId,
          customWishlistId: wishlistId
        };
        addAntiForgeryToken(postData);

        this.send_ajax(urlmove, postData);
		logger.warn(`<<< Exiting moveToCustomWishlist(urlmove=${urlmove},itemId=${itemId},wishlistId=${wishlistId})`);
    },

    //create custom wishlist
    createCustomWishlist: function (urlcreate, wishlistName, productId) {
	logger.error(`>>> Entering createCustomWishlist(urlcreate=${urlcreate},wishlistName=${wishlistName},productId=${productId})`);
      if (this.loadWaiting !== false) {
        return;
      }
      this.setLoadWaiting(true);

      var postData = {
        name: wishlistName,
        productId: productId
      };
      addAntiForgeryToken(postData);

      this.send_ajax(urlcreate, postData);
	logger.warn(`<<< Exiting createCustomWishlist(urlcreate=${urlcreate},wishlistName=${wishlistName},productId=${productId})`);
    },

    //delete custom wishlist
    deleteCustomWishlist: function (urldelete) {
		logger.error(`>>> Entering deleteCustomWishlist(urldelete=${urldelete})`);
        if (this.loadWaiting !== false) {
          return;
        }
        this.setLoadWaiting(true);

        var postData = {};
        addAntiForgeryToken(postData);

        this.send_ajax(urldelete, postData);
		logger.warn(`<<< Exiting deleteCustomWishlist(urldelete=${urldelete})`);
    },

    //move a product to the custom wishlist from the default wishlist
    moveproducttowishlist: function (urlmove, wishlistid) {
		logger.error(`>>> Entering moveproducttowishlist(urlmove=${urlmove},wishlistid=${wishlistid})`);
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);
        var postData = {
            wishlistId: wishlistid 
        };
        addAntiForgeryToken(postData);

        this.send_ajax(urlmove, postData);
		logger.warn(`<<< Exiting moveproducttowishlist(urlmove=${urlmove},wishlistid=${wishlistid})`);
    },

    //add a product to the cart/wishlist from the catalog pages
    addproducttocart_catalog: function (urladd) {
		logger.error(`>>> Entering addproducttocart_catalog(urladd=${urladd})`);
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);

        var postData = {};
        addAntiForgeryToken(postData);

        this.send_ajax(urladd, postData);
		logger.warn(`<<< Exiting addproducttocart_catalog(urladd=${urladd})`);
    },

    //add a product to the cart/wishlist from the product details page
    addproducttocart_details: function (urladd, formselector) {
		logger.error(`>>> Entering addproducttocart_details(urladd=${urladd},formselector=${formselector})`);
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);

        this.send_ajax(urladd, $(formselector).serialize());
		logger.warn(`<<< Exiting addproducttocart_details(urladd=${urladd},formselector=${formselector})`);
    },

    //add a product to compare list
    addproducttocomparelist: function (urladd) {
		logger.error(`>>> Entering addproducttocomparelist(urladd=${urladd})`);
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);

        var postData = {};
        addAntiForgeryToken(postData);

        this.send_ajax(urladd, postData);
		logger.warn(`<<< Exiting addproducttocomparelist(urladd=${urladd})`);
    },

    send_ajax: function (requestUrl, postData) {
	logger.error(`>>> Entering send_ajax(requestUrl=${requestUrl},postData=${postData})`);
      $.ajax({
        cache: false,
        url: requestUrl,
        type: "POST",
        data: postData,
        success: this.success_process,
        complete: this.resetLoadWaiting,
        error: this.ajaxFailure
      });
	logger.warn(`<<< Exiting send_ajax(requestUrl=${requestUrl},postData=${postData})`);
    },

    success_process: function (response) {
		logger.error(`>>> Entering success_process(response=${response})`);
        if (response.updatetopcartsectionhtml) {
            $(AjaxCart.topcartselector).html(response.updatetopcartsectionhtml);
        }
        if (response.updatetopwishlistsectionhtml) {
            $(AjaxCart.topwishlistselector).html(response.updatetopwishlistsectionhtml);
        }
        if (response.updateflyoutcartsectionhtml) {
            $(AjaxCart.flyoutcartselector).replaceWith(response.updateflyoutcartsectionhtml);
        }
        if (response.message) {
            //display notification
            if (response.success === true) {
                //success
                if (AjaxCart.usepopupnotifications === true) {
                    displayPopupNotification(response.message, 'success', true);
                }
                else {
                    //specify timeout for success messages
                    displayBarNotification(response.message, 'success', 3500);
                }
            }
            else {
                //error
                if (AjaxCart.usepopupnotifications === true) {
                    displayPopupNotification(response.message, 'error', true);
                }
                else {
                    //no timeout for errors
                    displayBarNotification(response.message, 'error', 0);
                }
            }
            return false;
        }
        if (response.redirect) {
            location.href = response.redirect;
			logger.warn(`<<< Exiting success_process(response=${response})`);
            return true;
        }
        return false;
    },

    resetLoadWaiting: function () {
		logger.error('>>> Entering resetLoadWaiting()');
        AjaxCart.setLoadWaiting(false);
		logger.warn('<<< Exiting resetLoadWaiting()');
    },

    ajaxFailure: function () {
	logger.error('>>> Entering ajaxFailure()');
      alert(this.localized_data.AjaxCartFailure);
	logger.warn('<<< Exiting ajaxFailure()');
    }
};