const logger = console;
/*
** nopCommerce filter level select js functions
*/
+function ($) {
	logger.debug('>>> Entering function()');
    'use strict';
    if ('undefined' == typeof (jQuery)) {
		logger.debug('<<< Exiting function()');
        throw new Error('jQuery JS required');
    }

    function filterLevel1SelectHandler() {
		logger.debug('>>> Entering filterLevel1SelectHandler()');
        var $this = $(this);
        var selectedItem = $this.val();
		logger.debug(`filterLevel1SelectHandler(): selectedItem → ${selectedItem}`);
        var filterLevel2 = $($this.data('filterlevel2'));
		logger.debug(`filterLevel1SelectHandler(): filterLevel2 → ${filterLevel2}`);
        var filterLevel3 = $($this.data('filterlevel3'));
		logger.debug(`filterLevel1SelectHandler(): filterLevel3 → ${filterLevel3}`);

        if (filterLevel2.length == 0)
            return;

        var loading = $($this.data('loading'));
        loading.show();

        $.ajax({
            cache: false,
            type: "GET",
            url: $this.data('url'),
            data: {
                'filterLevel1Value': selectedItem,
                'filterLevel2Value': '',
                'filterLevel3Value': ''
            },
            success: function (data, textStatus, jqXHR) {
			logger.debug(`>>> Entering success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
              filterLevel2.html('');
              if (selectedItem != '') {
				logger.debug(`filterLevel1SelectHandler(): loading → ${loading}`);
                filterLevel2.append($('<option></option>').val('').html(data[0].defaultItemText));
              }                
              $.each(data, function (index, item) {
				logger.debug(`>>> Entering each(data=${data})`);
                if (selectedItem === item.filterLevel1Value) {
                  filterLevel2.append($('<option></option>').val(item.filterLevel2Value).html(item.filterLevel2Value));
				logger.debug(`<<< Exiting each(data=${data})`);
                }
              });

                // Clear level 3 dropdown
                filterLevel3.html('');
                filterLevel3.append($('<option></option>').val('0').html(''));
				logger.debug(`<<< Exiting success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
            },
            error: function (jqXHR, textStatus, errorThrown) {
				logger.debug(`>>> Entering error(jqXHR=${jqXHR},textStatus=${textStatus},errorThrown=${errorThrown})`);
                alert('Failed to retrieve filter level 2 values.');
				logger.debug(`<<< Exiting error(jqXHR=${jqXHR},textStatus=${textStatus},errorThrown=${errorThrown})`);
            },
            complete: function (jqXHR, textStatus) {
				logger.debug(`>>> Entering complete(jqXHR=${jqXHR},textStatus=${textStatus})`);
                loading.hide();
				logger.debug(`<<< Exiting complete(jqXHR=${jqXHR},textStatus=${textStatus})`);
            }
        });
		logger.debug('<<< Exiting filterLevel1SelectHandler()');
    }

    function filterLevel2SelectHandler() {
		logger.debug('>>> Entering filterLevel2SelectHandler()');
        var $this = $(this);
        var selectedItem = $this.val();
		logger.debug(`filterLevel2SelectHandler(): selectedItem → ${selectedItem}`);
        var filterLevel1 = $($this.data('filterlevel1'));
		logger.debug(`filterLevel2SelectHandler(): filterLevel1 → ${filterLevel1}`);
        var filterLevel3 = $($this.data('filterlevel3'));
		logger.debug(`filterLevel2SelectHandler(): filterLevel3 → ${filterLevel3}`);
        var selectedLevel1 = filterLevel1.val();
		logger.debug(`filterLevel2SelectHandler(): selectedLevel1 → ${selectedLevel1}`);

        if (filterLevel3.length == 0)
            return;

        var loading = $($this.data('loading'));
        loading.show();

        $.ajax({
            cache: false,
            type: "GET",
            url: $this.data('url'),
            data: {
                'filterLevel1Value': selectedLevel1,
                'filterLevel2Value': selectedItem,
                'filterLevel3Value': ''
            },
            success: function (data, textStatus, jqXHR) {
                filterLevel3.html('');
                filterLevel3.append($('<option></option>').val('').html(data[0].defaultItemText));
                $.each(data, function (index, item) {
					logger.debug(`filterLevel2SelectHandler(): loading → ${loading}`);
                    if (selectedLevel1 === item.filterLevel1Value && selectedItem === item.filterLevel2Value) {
                        filterLevel3.append($('<option></option>').val(item.filterLevel3Value).html(item.filterLevel3Value));
                    }
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to retrieve filter level 3 values.');
            },
            complete: function (jqXHR, textStatus) {
                loading.hide();
            }
        });
		logger.debug('<<< Exiting filterLevel2SelectHandler()');
    }

    if ($(document).has('[data-trigger="filter-level1-select"]')) {
        $('select[data-trigger="filter-level1-select"]').change(filterLevel1SelectHandler);
    }
    if ($(document).has('[data-trigger="filter-level2-select"]')) {
        $('select[data-trigger="filter-level2-select"]').change(filterLevel2SelectHandler);
    }

    $.fn.filterLevelSelect = function () {
        this.each(function () {
            $(this).change(filterLevel1SelectHandler);
        });
    }
}(jQuery);