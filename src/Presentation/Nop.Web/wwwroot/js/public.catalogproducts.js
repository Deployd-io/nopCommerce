const logger = console;
﻿var CatalogProducts = {
  settings: {
    ajax: false,
    fetchUrl: false,
    browserPath: false,
  },

  params: {
    jqXHR: false,
  },

  init: function (settings) {
	logger.debug(`>>> Entering init(settings=${settings})`);
    this.settings = $.extend({}, this.settings, settings);
	logger.debug(`init(settings=${settings}): settings → ${settings}`);
	logger.debug(`<<< Exiting init(settings=${settings})`);
  },

  getProducts: function (pageNumber) {
	logger.debug(`>>> Entering getProducts(pageNumber=${pageNumber})`);
    if (this.params.jqXHR && this.params.jqXHR.readyState !== 4) {
      this.params.jqXHR.abort();
    }

    var urlBuilder = createProductsURLBuilder(this.settings.browserPath);
	logger.debug(`getProducts(pageNumber=${pageNumber}): urlBuilder → ${urlBuilder}`);

    if (pageNumber) {
      urlBuilder.addParameter('pagenumber', pageNumber);
    }

    var beforePayload = {
      urlBuilder
    };
    $(this).trigger({ type: "before", payload: beforePayload });

    this.setBrowserHistory(urlBuilder.build());

    if (!this.settings.ajax) {
      setLocation(urlBuilder.build());
    } else {
      this.setLoadWaiting(1);

      var postData = { ...beforePayload.urlBuilder.params.query };
      addAntiForgeryToken(postData)

	logger.debug(`<<< Exiting getProducts(pageNumber=${pageNumber})`);
      var self = this;
      this.params.jqXHR = $.ajax({
        cache: false,
        url: this.settings.fetchUrl,
        type: 'POST',
        data: postData,
        success: function (response) {
		logger.debug(`>>> Entering success(response=${response})`);
          $('.products-wrapper').html(response);
          $('html, body').animate({ scrollTop: $('.center-2 .page').offset().top }, 'slow');
		logger.debug(`getProducts(pageNumber=${pageNumber}): self → ${self}`);
          $(self).trigger({ type: "loaded" });
		logger.debug(`<<< Exiting success(response=${response})`);
        },
        error: function () {
		logger.debug('>>> Entering error()');
          $(self).trigger({ type: "error" });
		logger.debug('<<< Exiting error()');
        },
        complete: function () {
		logger.debug('>>> Entering complete()');
          self.setLoadWaiting();
		logger.debug('<<< Exiting complete()');
        }
      });
    }
  },

  setLoadWaiting(enable) {
	logger.debug(`>>> Entering setLoadWaiting(enable=${enable})`);
    var $busyEl = $('.ajax-products-busy');
    if (enable) {
      $busyEl.show();
    } else {
      $busyEl.hide();
	logger.debug(`<<< Exiting setLoadWaiting(enable=${enable})`);
    }
  },

  setBrowserHistory(url) {
	logger.debug(`>>> Entering setBrowserHistory(url=${url})`);
    window.history.replaceState({ path: url }, '', url);
	logger.debug(`<<< Exiting setBrowserHistory(url=${url})`);
  }
}

function createProductsURLBuilder(basePath) {
logger.debug(`>>> Entering createProductsURLBuilder(basePath=${basePath})`);
  return {
    params: {
      basePath: basePath,
      query: {}
    },

    addBasePath: function (url) {
      this.params.basePath = url;
	logger.debug(`<<< Exiting addBasePath(url=${url})`);
      return this;
    },

    addParameter: function (name, value) {
	logger.debug(`>>> Entering addParameter(name=${name},value=${value})`);
      this.params.query[name] = value;
	logger.debug(`<<< Exiting addParameter(name=${name},value=${value})`);
      return this;
    },

    build: function () {
	logger.debug('>>> Entering build()');
      var query = $.param(this.params.query);
      var url = this.params.basePath;
	logger.debug(`build(): query → ${query}`);

	logger.debug('<<< Exiting build()');
	logger.debug(`<<< Exiting createProductsURLBuilder(basePath=${basePath})`);
      return url.indexOf('?') !== -1
        ? url + '&' + query
        : url + '?' + query;
    }
  }
}