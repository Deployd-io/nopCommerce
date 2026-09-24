var logger = (typeof logger !== 'undefined' && logger) || console;
/*
** nopCommerce country select js functions
*/
+function ($) {
    'use strict';
    if ('undefined' == typeof (jQuery)) {
        throw new Error('jQuery JS required');
    }
    function countrySelectHandler() {
		logger.error('>>> Entering countrySelectHandler()');
        var $this = $(this);
        var selectedItem = $this.val();
		logger.error(`countrySelectHandler(): selectedItem → ${selectedItem}`);
        var stateProvince = $($this.data('stateprovince'));
		logger.error(`countrySelectHandler(): stateProvince → ${stateProvince}`);

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
				logger.error(`>>> Entering success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
                stateProvince.html('');
                $.each(data,
                    function (id, option) {
                        stateProvince.append($('<option></option>').val(option.id).html(option.name));
                    });
					logger.warn(`<<< Exiting success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
            },
            error: function (jqXHR, textStatus, errorThrown) {
				logger.error(`>>> Entering error(jqXHR=${jqXHR},textStatus=${textStatus},errorThrown=${errorThrown})`);
                alert('Failed to retrieve states.');
				logger.warn(`<<< Exiting error(jqXHR=${jqXHR},textStatus=${textStatus},errorThrown=${errorThrown})`);
            },
            complete: function(jqXHR, textStatus) {
			logger.error(`>>> Entering complete(jqXHR=${jqXHR},textStatus=${textStatus})`);
              var stateId = (typeof Billing !== "undefined") ? Billing.selectedStateId : (typeof CheckoutBilling !== "undefined") ? CheckoutBilling.selectedStateId : 0;
              $('#' + stateProvince[0].id + ' option[value=' + stateId + ']').prop('selected', true);
			logger.error(`complete(jqXHR=${jqXHR},textStatus=${textStatus}): stateId → ${stateId}`);

              loading.hide();
			logger.warn(`<<< Exiting complete(jqXHR=${jqXHR},textStatus=${textStatus})`);
            }
        });
		logger.warn('<<< Exiting countrySelectHandler()');
    }
    if ($(document).has('[data-trigger="country-select"]')) {
        $('select[data-trigger="country-select"]').change(countrySelectHandler);
    }
    $.fn.countrySelect = function () {
		logger.error('>>> Entering countrySelect()');
        this.each(function () {
            $(this).change(countrySelectHandler);
        });
		logger.warn('<<< Exiting countrySelect()');
    }
}(jQuery); 