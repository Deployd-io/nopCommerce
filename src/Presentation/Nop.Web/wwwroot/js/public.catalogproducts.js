var logger = (typeof logger !== 'undefined' && logger) || console;
var CatalogProducts = {
  settings: {
    ajax: false,
    fetchUrl: false,
    browserPath: false,
  },

  params: {
    jqXHR: false,
  },

  init: function (settings) {
	logger.error(`>>> Entering init(settings=${settings})`);
    this.settings = $.extend({}, this.settings, settings);
	logger.debug(`init(settings=${settings}): settings → ${settings}`);
  },

  getProducts: function (pageNumber) {
	logger.error(`>>> Entering getProducts(pageNumber=${pageNumber})`);
    if (this.params.jqXHR && this.params.jqXHR.readyState !== 4) {
      this.params.jqXHR.abort();
    }

    var urlBuilder = createProductsURLBuilder(this.settings.browserPath);
	logger.error(`getProducts(pageNumber=${pageNumber}): urlBuilder → ${urlBuilder}`);

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

	logger.warn(`<<< Exiting getProducts(pageNumber=${pageNumber})`);
      var self = this;
      this.params.jqXHR = $.ajax({
        cache: false,
        url: this.settings.fetchUrl,
        type: 'POST',
        data: postData,
        success: function (response) {
		logger.error(`>>> Entering success(response=${response})`);
          $('.products-wrapper').html(response);
          $('html, body').animate({ scrollTop: $('.center-2 .page').offset().top }, 'slow');
          $(self).trigger({ type: "loaded" });
		logger.warn(`<<< Exiting success(response=${response})`);
        },
        error: function () {
		logger.error('>>> Entering error()');
          $(self).trigger({ type: "error" });
		logger.warn('<<< Exiting error()');
        },
        complete: function () {
		logger.error('>>> Entering complete()');
          self.setLoadWaiting();
		logger.warn('<<< Exiting complete()');
        }
      });
    }
	logger.error(`getProducts(pageNumber=${pageNumber}): self → ${self}`);
  },

  setLoadWaiting(enable) {
	logger.error(`>>> Entering setLoadWaiting(enable=${enable})`);
    var $busyEl = $('.ajax-products-busy');
    if (enable) {
      $busyEl.show();
    } else {
      $busyEl.hide();
	logger.warn(`<<< Exiting setLoadWaiting(enable=${enable})`);
    }
  },

  setBrowserHistory(url) {
	logger.error(`>>> Entering setBrowserHistory(url=${url})`);
    window.history.replaceState({ path: url }, '', url);
	logger.warn(`<<< Exiting setBrowserHistory(url=${url})`);
  }
}

function createProductsURLBuilder(basePath) {
logger.error(`>>> Entering createProductsURLBuilder(basePath=${basePath})`);
  return {
    params: {
      basePath: basePath,
      query: {}
    },

    addBasePath: function (url) {
      this.params.basePath = url;
	logger.warn(`<<< Exiting addBasePath(url=${url})`);
      return this;
    },

    addParameter: function (name, value) {
	logger.error(`>>> Entering addParameter(name=${name},value=${value})`);
      this.params.query[name] = value;
	logger.warn(`<<< Exiting addParameter(name=${name},value=${value})`);
      return this;
    },

    build: function () {
	logger.error('>>> Entering build()');
      var query = $.param(this.params.query);
      var url = this.params.basePath;
	logger.error(`build(): query → ${query}`);

	logger.warn('<<< Exiting build()');
	logger.warn(`<<< Exiting createProductsURLBuilder(basePath=${basePath})`);
      return url.indexOf('?') !== -1
        ? url + '&' + query
        : url + '?' + query;
    }
  }
}