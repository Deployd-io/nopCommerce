const logger = console;
﻿/*
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
		logger.debug(`>>> Entering init(usepopupnotifications=${usepopupnotifications},topcartselector=${topcartselector},topwishlistselector=${topwishlistselector},flyoutcartselector=${flyoutcartselector},localized_data=${localized_data})`);
        this.loadWaiting = false;
        this.usepopupnotifications = usepopupnotifications;
        this.topcartselector = topcartselector;
		logger.debug(`init(usepopupnotifications=${usepopupnotifications},topcartselector=${topcartselector},topwishlistselector=${topwishlistselector},flyoutcartselector=${flyoutcartselector},localized_data=${localized_data}): this.loadWaiting → ${this.loadWaiting}`);
		logger.debug(`init(usepopupnotifications=${usepopupnotifications},topcartselector=${topcartselector},topwishlistselector=${topwishlistselector},flyoutcartselector=${flyoutcartselector},localized_data=${localized_data}): usepopupnotifications → ${usepopupnotifications}`);
		logger.debug(`init(usepopupnotifications=${usepopupnotifications},topcartselector=${topcartselector},topwishlistselector=${topwishlistselector},flyoutcartselector=${flyoutcartselector},localized_data=${localized_data}): topcartselector → ${topcartselector}`);
		logger.debug(`init(usepopupnotifications=${usepopupnotifications},topcartselector=${topcartselector},topwishlistselector=${topwishlistselector},flyoutcartselector=${flyoutcartselector},localized_data=${localized_data}): this.localized_data → ${this.localized_data}`);
		logger.debug(`init(usepopupnotifications=${usepopupnotifications},topcartselector=${topcartselector},topwishlistselector=${topwishlistselector},flyoutcartselector=${flyoutcartselector},localized_data=${localized_data}): flyoutcartselector → ${flyoutcartselector}`);
		logger.debug(`init(usepopupnotifications=${usepopupnotifications},topcartselector=${topcartselector},topwishlistselector=${topwishlistselector},flyoutcartselector=${flyoutcartselector},localized_data=${localized_data}): localized_data → ${localized_data}`);
        this.topwishlistselector = topwishlistselector;
        this.flyoutcartselector = flyoutcartselector;
        this.localized_data = localized_data;
		logger.debug(`<<< Exiting init(usepopupnotifications=${usepopupnotifications},topcartselector=${topcartselector},topwishlistselector=${topwishlistselector},flyoutcartselector=${flyoutcartselector},localized_data=${localized_data})`);
    },

    setLoadWaiting: function (display) {
        displayAjaxLoading(display);
		logger.debug(`>>> Entering setLoadWaiting(display=${display})`);
        this.loadWaiting = display;
		logger.debug(`<<< Exiting setLoadWaiting(display=${display})`);
  },

    //move a shopping cart item to the custom wishlist
    moveToCustomWishlist: function (urlmove, itemId, wishlistId) {
		logger.debug(`>>> Entering moveToCustomWishlist(urlmove=${urlmove},itemId=${itemId},wishlistId=${wishlistId})`);
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
		logger.debug(`<<< Exiting moveToCustomWishlist(urlmove=${urlmove},itemId=${itemId},wishlistId=${wishlistId})`);
    },

    //create custom wishlist
    createCustomWishlist: function (urlcreate, wishlistName, productId) {
	logger.debug(`>>> Entering createCustomWishlist(urlcreate=${urlcreate},wishlistName=${wishlistName},productId=${productId})`);
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
	logger.debug(`<<< Exiting createCustomWishlist(urlcreate=${urlcreate},wishlistName=${wishlistName},productId=${productId})`);
    },

    //delete custom wishlist
    deleteCustomWishlist: function (urldelete) {
		logger.debug(`>>> Entering deleteCustomWishlist(urldelete=${urldelete})`);
        if (this.loadWaiting !== false) {
          return;
        }
        this.setLoadWaiting(true);

        var postData = {};
        addAntiForgeryToken(postData);

        this.send_ajax(urldelete, postData);
		logger.debug(`<<< Exiting deleteCustomWishlist(urldelete=${urldelete})`);
    },

    //move a product to the custom wishlist from the default wishlist
    moveproducttowishlist: function (urlmove, wishlistid) {
		logger.debug(`>>> Entering moveproducttowishlist(urlmove=${urlmove},wishlistid=${wishlistid})`);
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);
        var postData = {
            wishlistId: wishlistid 
        };
        addAntiForgeryToken(postData);

        this.send_ajax(urlmove, postData);
		logger.debug(`<<< Exiting moveproducttowishlist(urlmove=${urlmove},wishlistid=${wishlistid})`);
    },

    //add a product to the cart/wishlist from the catalog pages
    addproducttocart_catalog: function (urladd) {
		logger.debug(`>>> Entering addproducttocart_catalog(urladd=${urladd})`);
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);

        var postData = {};
        addAntiForgeryToken(postData);

        this.send_ajax(urladd, postData);
		logger.debug(`<<< Exiting addproducttocart_catalog(urladd=${urladd})`);
    },

    //add a product to the cart/wishlist from the product details page
    addproducttocart_details: function (urladd, formselector) {
		logger.debug(`>>> Entering addproducttocart_details(urladd=${urladd},formselector=${formselector})`);
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);

        this.send_ajax(urladd, $(formselector).serialize());
		logger.debug(`<<< Exiting addproducttocart_details(urladd=${urladd},formselector=${formselector})`);
    },

    //add a product to compare list
    addproducttocomparelist: function (urladd) {
		logger.debug(`>>> Entering addproducttocomparelist(urladd=${urladd})`);
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);

        var postData = {};
        addAntiForgeryToken(postData);

        this.send_ajax(urladd, postData);
		logger.debug(`<<< Exiting addproducttocomparelist(urladd=${urladd})`);
    },

    send_ajax: function (requestUrl, postData) {
      $.ajax({
        cache: false,
        url: requestUrl,
        type: "POST",
        data: postData,
        success: this.success_process,
        complete: this.resetLoadWaiting,
        error: this.ajaxFailure
      });
    },

    success_process: function (response) {
		logger.debug(`>>> Entering success_process(response=${response})`);
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
			logger.debug(`success_process(response=${response}): location.href → ${location.href}`);
            return true;
        }
        return false;
    },

    resetLoadWaiting: function () {
		logger.debug('>>> Entering resetLoadWaiting()');
        AjaxCart.setLoadWaiting(false);
		logger.debug('<<< Exiting resetLoadWaiting()');
    },

    ajaxFailure: function () {
	logger.debug('>>> Entering ajaxFailure()');
      alert(this.localized_data.AjaxCartFailure);
	logger.debug('<<< Exiting ajaxFailure()');
    }
};