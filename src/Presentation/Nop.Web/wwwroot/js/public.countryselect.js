const logger = console;
/*
** nopCommerce country select js functions
*/
+function ($) {
	logger.debug('>>> Entering function()');
    'use strict';
    if ('undefined' == typeof (jQuery)) {
		logger.debug('<<< Exiting function()');
        throw new Error('jQuery JS required');
    }
    function countrySelectHandler() {
		logger.debug('>>> Entering countrySelectHandler()');
        var $this = $(this);
        var selectedItem = $this.val();
		logger.debug(`countrySelectHandler(): selectedItem → ${selectedItem}`);
        var stateProvince = $($this.data('stateprovince'));
		logger.debug(`countrySelectHandler(): stateProvince → ${stateProvince}`);

        if(stateProvince.length == 0)
          return;

        var loading = $($this.data('loading'));
        loading.show();
        $.ajax({
            cache: false,
            type: "GET",
            url: $this.data('url'),
            data: { 
              'countryId': selectedItem,
              'addSelectStateItem': "true" 
            },
            success: function (data, textStatus, jqXHR) {
				logger.debug(`>>> Entering success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
                stateProvince.html('');
                $.each(data,
                    function (id, option) {
						logger.debug(`>>> Entering function(id=${id},option=${option})`);
                        stateProvince.append($('<option></option>').val(option.id).html(option.name));
                    });
            },
            error: function (jqXHR, textStatus, errorThrown) {
				logger.debug(`>>> Entering error(jqXHR=${jqXHR},textStatus=${textStatus},errorThrown=${errorThrown})`);
                alert('Failed to retrieve states.');
				logger.debug(`<<< Exiting error(jqXHR=${jqXHR},textStatus=${textStatus},errorThrown=${errorThrown})`);
            },
            complete: function(jqXHR, textStatus) {
			logger.debug(`>>> Entering complete(jqXHR=${jqXHR},textStatus=${textStatus})`);
              var stateId = (typeof Billing !== "undefined") ? Billing.selectedStateId : (typeof CheckoutBilling !== "undefined") ? CheckoutBilling.selectedStateId : 0;
              $('#' + stateProvince[0].id + ' option[value=' + stateId + ']').prop('selected', true);
			logger.debug(`complete(jqXHR=${jqXHR},textStatus=${textStatus}): stateId → ${stateId}`);

              loading.hide();
			logger.debug(`<<< Exiting complete(jqXHR=${jqXHR},textStatus=${textStatus})`);
            }
        });
		logger.debug('<<< Exiting countrySelectHandler()');
    }
    if ($(document).has('[data-trigger="country-select"]')) {
        $('select[data-trigger="country-select"]').change(countrySelectHandler);
    }
    $.fn.countrySelect = function () {
        this.each(function () {
			logger.debug('>>> Entering each()');
            $(this).change(countrySelectHandler);
        });
    }
}(jQuery); 