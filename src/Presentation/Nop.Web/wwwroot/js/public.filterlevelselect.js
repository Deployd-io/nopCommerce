var logger = (typeof logger !== 'undefined' && logger) || console;
/*
** nopCommerce filter level select js functions
*/
+function ($) {
    'use strict';
    if ('undefined' == typeof (jQuery)) {
        throw new Error('jQuery JS required');
    }

    function filterLevel1SelectHandler() {
		logger.error('>>> Entering filterLevel1SelectHandler()');
        var $this = $(this);
        var selectedItem = $this.val();
		logger.error(`filterLevel1SelectHandler(): selectedItem → ${selectedItem}`);
        var filterLevel2 = $($this.data('filterlevel2'));
		logger.error(`filterLevel1SelectHandler(): filterLevel2 → ${filterLevel2}`);
        var filterLevel3 = $($this.data('filterlevel3'));
		logger.error(`filterLevel1SelectHandler(): filterLevel3 → ${filterLevel3}`);

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
			logger.error(`>>> Entering success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
              filterLevel2.html('');
              if (selectedItem != '') {
				logger.error(`filterLevel1SelectHandler(): loading → ${loading}`);
                filterLevel2.append($('<option></option>').val('').html(data[0].defaultItemText));
              }                
              $.each(data, function (index, item) {
                if (selectedItem === item.filterLevel1Value) {
                  filterLevel2.append($('<option></option>').val(item.filterLevel2Value).html(item.filterLevel2Value));
                }
              });

                // Clear level 3 dropdown
                filterLevel3.html('');
                filterLevel3.append($('<option></option>').val('0').html(''));
				logger.warn(`<<< Exiting success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
            },
            error: function (jqXHR, textStatus, errorThrown) {
				logger.error(`>>> Entering error(jqXHR=${jqXHR},textStatus=${textStatus},errorThrown=${errorThrown})`);
                alert('Failed to retrieve filter level 2 values.');
				logger.warn(`<<< Exiting error(jqXHR=${jqXHR},textStatus=${textStatus},errorThrown=${errorThrown})`);
            },
            complete: function (jqXHR, textStatus) {
				logger.error(`>>> Entering complete(jqXHR=${jqXHR},textStatus=${textStatus})`);
                loading.hide();
				logger.warn(`<<< Exiting complete(jqXHR=${jqXHR},textStatus=${textStatus})`);
            }
        });
		logger.warn('<<< Exiting filterLevel1SelectHandler()');
    }

    function filterLevel2SelectHandler() {
		logger.error('>>> Entering filterLevel2SelectHandler()');
        var $this = $(this);
        var selectedItem = $this.val();
		logger.error(`filterLevel2SelectHandler(): selectedItem → ${selectedItem}`);
        var filterLevel1 = $($this.data('filterlevel1'));
		logger.error(`filterLevel2SelectHandler(): filterLevel1 → ${filterLevel1}`);
        var filterLevel3 = $($this.data('filterlevel3'));
		logger.error(`filterLevel2SelectHandler(): filterLevel3 → ${filterLevel3}`);
        var selectedLevel1 = filterLevel1.val();
		logger.error(`filterLevel2SelectHandler(): selectedLevel1 → ${selectedLevel1}`);

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
				logger.error(`filterLevel2SelectHandler(): loading → ${loading}`);
                $.each(data, function (index, item) {
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
		logger.warn('<<< Exiting filterLevel2SelectHandler()');
    }

    if ($(document).has('[data-trigger="filter-level1-select"]')) {
        $('select[data-trigger="filter-level1-select"]').change(filterLevel1SelectHandler);
    }
    if ($(document).has('[data-trigger="filter-level2-select"]')) {
        $('select[data-trigger="filter-level2-select"]').change(filterLevel2SelectHandler);
    }

    $.fn.filterLevelSelect = function () {
		logger.error('>>> Entering filterLevelSelect()');
        this.each(function () {
            $(this).change(filterLevel1SelectHandler);
        });
		logger.warn('<<< Exiting filterLevelSelect()');
    }
}(jQuery);