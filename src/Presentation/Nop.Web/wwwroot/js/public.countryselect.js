const logger = console;
﻿/*
** nopCommerce country select js functions
*/
+function ($) {
    'use strict';
    if ('undefined' == typeof (jQuery)) {
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
                        stateProvince.append($('<option></option>').val(option.id).html(option.name));
                    });
					logger.debug(`<<< Exiting success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to retrieve states.');
            },
            complete: function(jqXHR, textStatus) {
              var stateId = (typeof Billing !== "undefined") ? Billing.selectedStateId : (typeof CheckoutBilling !== "undefined") ? CheckoutBilling.selectedStateId : 0;
              $('#' + stateProvince[0].id + ' option[value=' + stateId + ']').prop('selected', true);

              loading.hide();
            }
        });
		logger.debug('<<< Exiting countrySelectHandler()');
    }
    if ($(document).has('[data-trigger="country-select"]')) {
        $('select[data-trigger="country-select"]').change(countrySelectHandler);
    }
    $.fn.countrySelect = function () {
        this.each(function () {
            $(this).change(countrySelectHandler);
        });
    }
}(jQuery); 