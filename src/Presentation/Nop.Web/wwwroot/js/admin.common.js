const logger = console;
//this method is used to show an element by removing the appropriate hiding class
//we don't use the jquery show/hide methods since they don't work with "display: flex" properly
$.fn.showElement = function () {
logger.debug('>>> Entering function()');
  this.removeClass('d-none');
logger.debug('<<< Exiting function()');
}

//this method is used to hide an element by adding the appropriate hiding class
//we don't use the jquery show/hide methods since they don't work with "display: flex" properly
$.fn.hideElement = function () {
  this.addClass('d-none');
}

function setLocation(url) {
	logger.debug(`>>> Entering setLocation(url=${url})`);
    window.location.href = url;
	logger.debug(`<<< Exiting setLocation(url=${url})`);
}

function OpenWindow(query, w, h, scroll) {
	logger.debug(`>>> Entering OpenWindow(query=${query},w=${w},h=${h},scroll=${scroll})`);
    var l = (screen.width - w) / 2;
    var t = (screen.height - h) / 2;
	logger.debug(`OpenWindow(query=${query},w=${w},h=${h},scroll=${scroll}): l → ${l}`);
	logger.debug(`OpenWindow(query=${query},w=${w},h=${h},scroll=${scroll}): t → ${t}`);

    winprops = 'resizable=1, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
    if (scroll) winprops += ',scrollbars=1';
    var f = window.open(query, "_blank", winprops);
	logger.debug(`<<< Exiting OpenWindow(query=${query},w=${w},h=${h},scroll=${scroll})`);
}

function showThrobber(message) {
	logger.debug(`>>> Entering showThrobber(message=${message})`);
    $('.throbber-header').html(message);
    window.setTimeout(function () {
		logger.debug('>>> Entering setTimeout()');
        $(".throbber").show();
		logger.debug('<<< Exiting setTimeout()');
    }, 1000);
	logger.debug(`<<< Exiting showThrobber(message=${message})`);
}

$(function() {
    $('.multi-store-override-option').each(function (k, v) {
		logger.debug('>>> Entering each()');
        checkOverriddenStoreValue(v, $(v).attr('data-for-input-selector'));
    });

    //we must intercept all events of pressing the Enter button in the search bar to be sure that the input focus remains in the context of the search
    $("div.card-search").keypress(function (event) {
		logger.debug('>>> Entering keypress()');
        if (event.which == 13 || event.keyCode == 13) {
          $("button.btn-search").trigger("click");
			logger.debug('<<< Exiting keypress()');
            return false;
        }
    });

    //pressing Enter in the tablex should not lead to any action
    $("div[id$='-grid']").keypress(function (event) {
        if (event.which == 13 || event.keyCode == 13) {
            return false;
        }
    });
});

function checkAllOverriddenStoreValue(item) {
	logger.debug(`>>> Entering checkAllOverriddenStoreValue(item=${item})`);
    $('.multi-store-override-option').each(function (k, v) {
        $(v).attr('checked', item.checked);
        checkOverriddenStoreValue(v, $(v).attr('data-for-input-selector'));
    });
	logger.debug(`<<< Exiting checkAllOverriddenStoreValue(item=${item})`);
}

function checkOverriddenStoreValue(obj, selector) {
	logger.debug(`>>> Entering checkOverriddenStoreValue(obj=${obj},selector=${selector})`);
    var elementsArray = selector.split(",");

    // first toggle appropriate hidden inputs for checkboxes
    if ($(selector).is(':checkbox')) {
		logger.debug(`checkOverriddenStoreValue(obj=${obj},selector=${selector}): elementsArray → ${elementsArray}`);
        var name = $(selector).attr('name');
		logger.debug(`checkOverriddenStoreValue(obj=${obj},selector=${selector}): name → ${name}`);
        $('input:hidden[name="' + name + '"]').attr('disabled', !$(obj).is(':checked'));
    }

    if (!$(obj).is(':checked')) {
        $(selector).attr('disabled', true);
    }
    else {
        $(selector).removeAttr('disabled');
		logger.debug(`<<< Exiting checkOverriddenStoreValue(obj=${obj},selector=${selector})`);
    }
}

function bindBootstrapTabSelectEvent(tabsId, inputId) {
	logger.debug(`>>> Entering bindBootstrapTabSelectEvent(tabsId=${tabsId},inputId=${inputId})`);
    $('#' + tabsId + ' > div ul li a[data-toggle="pill"]').on('shown.bs.tab', function (e) {
		logger.debug('>>> Entering on()');
        var tabName = $(e.target).attr("data-tab-name");
        $("#" + inputId).val(tabName);
    });
	logger.debug(`<<< Exiting bindBootstrapTabSelectEvent(tabsId=${tabsId},inputId=${inputId})`);
}

function display_nop_error(e) {
logger.debug(`>>> Entering display_nop_error(e=${e})`);
  if (e.error) {
    if ((typeof e.error) == 'string') {
      //single error
      //display the message
      alert(e.error);
    } else {
      //array of errors
      var message = "The following errors have occurred:";
	logger.debug(`display_nop_error(e=${e}): message → ${message}`);
      //create a message containing all errors.
      $.each(e.error, function (key, value) {
        if (value.errors) {
          message += "\n";
          message += value.errors.join("\n");
        }
      });
      //display the message
      alert(message);
    }
    //ignore empty error
  } else if (e.errorThrown) {
    alert('Error happened');
	logger.debug(`<<< Exiting display_nop_error(e=${e})`);
  }
}

// CSRF (XSRF) security
function addAntiForgeryToken(data) {
	logger.debug(`>>> Entering addAntiForgeryToken(data=${data})`);
    //if the object is undefined, create a new one.
    if (!data) {
        data = {};
    }
    //add token
    var tokenInput = $('input[name=__RequestVerificationToken]');
	logger.debug(`addAntiForgeryToken(data=${data}): data → ${data}`);
    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();
    }
    return data;
};

function saveUserPreferences(url, name, value) {
	logger.debug(`>>> Entering saveUserPreferences(url=${url},name=${name},value=${value})`);
    var postData = {
        name: name,
        value: value
    };
    addAntiForgeryToken(postData);
    $.ajax({
        cache: false,
        url: url,
        type: "POST",
        data: postData,
        dataType: "json",
        error: function (jqXHR, textStatus, errorThrown) {
		logger.debug(`>>> Entering error(jqXHR=${jqXHR},textStatus=${textStatus},errorThrown=${errorThrown})`);
          alert('Failed to save preferences.');
		logger.debug(`<<< Exiting error(jqXHR=${jqXHR},textStatus=${textStatus},errorThrown=${errorThrown})`);
        },
        complete: function (jqXHR, textStatus) {
		logger.debug(`>>> Entering complete(jqXHR=${jqXHR},textStatus=${textStatus})`);
          $("#ajaxBusy span").removeClass("no-ajax-loader");
		logger.debug(`<<< Exiting complete(jqXHR=${jqXHR},textStatus=${textStatus})`);
	logger.debug(`<<< Exiting saveUserPreferences(url=${url},name=${name},value=${value})`);
        }        
  });

};

function warningValidation(validationUrl, warningElementName, passedParameters) {
	logger.debug(`>>> Entering warningValidation(validationUrl=${validationUrl},warningElementName=${warningElementName},passedParameters=${passedParameters})`);
    addAntiForgeryToken(passedParameters);
    var element = $('[data-valmsg-for="' + warningElementName + '"]');
	logger.debug(`warningValidation(validationUrl=${validationUrl},warningElementName=${warningElementName},passedParameters=${passedParameters}): element → ${element}`);

    var messageElement = element.siblings('.field-validation-custom');
	logger.debug(`warningValidation(validationUrl=${validationUrl},warningElementName=${warningElementName},passedParameters=${passedParameters}): messageElement → ${messageElement}`);
    if (messageElement.length == 0) {
        messageElement = $(document.createElement("span"));
        messageElement.addClass('field-validation-custom');
        element.after(messageElement);
		logger.debug(`<<< Exiting warningValidation(validationUrl=${validationUrl},warningElementName=${warningElementName},passedParameters=${passedParameters})`);
    }

    $.ajax({
        cache: false,
        url: validationUrl,
        type: "POST",
        dataType: "json",
        data: passedParameters,
        success: function (data, textStatus, jqXHR) {
			logger.debug(`>>> Entering success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
            if (data.Result) {
                messageElement.addClass("warning");
                messageElement.html(data.Result);
            } else {
                messageElement.removeClass("warning");
                messageElement.html('');
				logger.debug(`<<< Exiting success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            messageElement.removeClass("warning");
            messageElement.html('');
        }
    });
};

function toggleNestedSetting(parentSettingName, parentFormGroupId) {
	logger.debug(`>>> Entering toggleNestedSetting(parentSettingName=${parentSettingName},parentFormGroupId=${parentFormGroupId})`);
    if ($('input[name="' + parentSettingName + '"]').is(':checked')) {
        $('#' + parentFormGroupId).addClass('opened');
    } else {
        $('#' + parentFormGroupId).removeClass('opened');
		logger.debug(`<<< Exiting toggleNestedSetting(parentSettingName=${parentSettingName},parentFormGroupId=${parentFormGroupId})`);
    }
}

function parentSettingClick(e) {
	logger.debug(`>>> Entering parentSettingClick(e=${e})`);
    toggleNestedSetting(e.data.parentSettingName, e.data.parentFormGroupId);
	logger.debug(`<<< Exiting parentSettingClick(e=${e})`);
}

function initNestedSetting(parentSettingName, parentSettingId, nestedSettingId) {
	logger.debug(`>>> Entering initNestedSetting(parentSettingName=${parentSettingName},parentSettingId=${parentSettingId},nestedSettingId=${nestedSettingId})`);
    var parentFormGroup = $('input[name="' + parentSettingName +'"]').closest('.form-group');
    var parentFormGroupId = $(parentFormGroup).attr('id');
	logger.debug(`initNestedSetting(parentSettingName=${parentSettingName},parentSettingId=${parentSettingId},nestedSettingId=${nestedSettingId}): parentFormGroup → ${parentFormGroup}`);
    if (!parentFormGroupId) {
        parentFormGroupId = parentSettingId;
    }
    $(parentFormGroup).addClass('parent-setting').attr('id', parentFormGroupId);
    if ($('#' + nestedSettingId + ' .form-group').length == $('#' + nestedSettingId + ' .form-group.advanced-setting').length) {
        $('#' + parentFormGroupId).addClass('parent-setting-advanced');
    }

    //$(document).on('click', 'input[name="' + parentSettingName + '"]', toggleNestedSetting(parentSettingName, parentFormGroupId));
    $('input[name="' + parentSettingName + '"]').click(
        { parentSettingName: parentSettingName, parentFormGroupId: parentFormGroupId }, parentSettingClick);
    toggleNestedSetting(parentSettingName, parentFormGroupId);
	logger.debug(`<<< Exiting initNestedSetting(parentSettingName=${parentSettingName},parentSettingId=${parentSettingId},nestedSettingId=${nestedSettingId})`);
}

//scroll to top
(function ($) {
    $.fn.backTop = function () {
        var backBtn = this;

        var position = 1000;
        var speed = 900;

        $(document).scroll(function () {
			logger.debug(`>>> Entering scroll(document=${document})`);
            var pos = $(window).scrollTop();

			logger.debug(`scroll(document=${document}): pos → ${pos}`);
            if (pos >= position) {
                backBtn.fadeIn(speed);
            } else {
                backBtn.fadeOut(speed);
				logger.debug(`<<< Exiting scroll(document=${document})`);
            }
        });

        backBtn.click(function () {
			logger.debug(`>>> Entering click(scrollTop=${scrollTop})`);
            $("html, body").animate({ scrollTop: 0 }, 900);
        });
    }
}(jQuery));

// Ajax activity indicator bound to ajax start/stop document events
$(document).ajaxStart(function () {
	logger.debug(`>>> Entering ajaxStart(document=${document})`);
    $('#ajaxBusy').show();
}).ajaxStop(function () {
    $('#ajaxBusy').hide();
    });

//no-tabs solution
$(function() {
  $(".card.card-secondary >.card-header").click(CardToggle);

  //expanded
  $('.card.card-secondary').on('expanded.lte.cardwidget', function () {
    WrapAndSaveBlockData($(this), false)
    
    if ($(this).find('table.dataTable').length > 0) {
      setTimeout(function () {
        ensureDataTablesRendered();
      }, 420);
    }
  });

  //collapsed
  $('.card.card-secondary').on('collapsed.lte.cardwidget', function () {
    WrapAndSaveBlockData($(this), true)
  });
});

function CardToggle() {
logger.debug('>>> Entering CardToggle()');
  var card = $(this).parent(".card.card-secondary");
  card.CardWidget('toggle'); 
logger.debug(`CardToggle(): card → ${card}`);
}

function WrapAndSaveBlockData(card, collapsed) {
logger.debug(`>>> Entering WrapAndSaveBlockData(card=${card},collapsed=${collapsed})`);
  var hideAttribute = card.attr("data-hideAttribute");
  saveUserPreferences(rootAppPath + 'admin/preferences/savepreference', hideAttribute, collapsed);
logger.debug(`WrapAndSaveBlockData(card=${card},collapsed=${collapsed}): hideAttribute → ${hideAttribute}`);
}

//collapse search block
$(function() {
  $(".row.search-row").click(ToggleSearchBlockAndSavePreferences);
});

function ToggleSearchBlockAndSavePreferences() {
	logger.debug('>>> Entering ToggleSearchBlockAndSavePreferences()');
    $(this).parents(".card-search").find(".search-body").slideToggle();
    var icon = $(this).find(".icon-collapse i");
	logger.debug(`ToggleSearchBlockAndSavePreferences(): icon → ${icon}`);
    if ($(this).hasClass("opened")) {
      icon.removeClass("fa-angle-up");
      icon.addClass("fa-angle-down");
      saveUserPreferences(rootAppPath + 'admin/preferences/savepreference', $(this).attr("data-hideAttribute"), true);
    } else {
      icon.addClass("fa-angle-up");
      icon.removeClass("fa-angle-down");
      saveUserPreferences(rootAppPath + 'admin/preferences/savepreference', $(this).attr("data-hideAttribute"), false);
    }

    $(this).toggleClass("opened");
	logger.debug('<<< Exiting ToggleSearchBlockAndSavePreferences()');
}

function ensureDataTablesRendered() {
logger.debug('>>> Entering ensureDataTablesRendered()');
  $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
logger.debug('<<< Exiting ensureDataTablesRendered()');
}

function reloadAllDataTables(itemCount) {
logger.debug(`>>> Entering reloadAllDataTables(itemCount=${itemCount})`);
  //depending on the number of elements, the time for animation of opening the menu should increase
  var timePause = 300;
  if (itemCount) {
    timePause = itemCount * 100;
  }
logger.debug(`<<< Exiting reloadAllDataTables(itemCount=${itemCount})`);
  $('table[class^="table"]').each(function () {
    setTimeout(function () {
      ensureDataTablesRendered();
    }, timePause);
  });
}

/**
 * @param {string} alertId Unique identifier of alert
 * @param {any} text Message text
 */
function showAlert(alertId, text)
{
	logger.debug(`>>> Entering showAlert(alertId=${alertId},text=${text})`);
    $('#' + alertId + '-info').text(text);
    $('#' + alertId).trigger("click");
	logger.debug(`<<< Exiting showAlert(alertId=${alertId},text=${text})`);
}

//scrolling and hidden DataTables issue workaround
//More info - https://datatables.net/examples/api/tabs_and_scrolling.html
$(function() {
  $('button[data-card-widget="collapse"]').on('click', function (e) {
    //hack with waiting animation. 
    //when page is loaded, a box that should be collapsed have style 'display: none;'.that's why a table is not updated
    setTimeout(function () {
      ensureDataTablesRendered();
    }, 1);
  });

  // when tab item click
  $('.nav-tabs .nav-item').on('click', function (e) {
    setTimeout(function () {
	logger.debug(`>>> Entering setTimeout(timeout=${timeout})`);
	logger.debug(`<<< Exiting setTimeout(timeout=${timeout})`);
      ensureDataTablesRendered();
    }, 1);
  });

  $('ul li a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    ensureDataTablesRendered();
  });

  $('#advanced-settings-mode').on('click', function (e) {
    ensureDataTablesRendered();
  });

  //when sidebar-toggle click
  $('#nopSideBarPusher').on('click', function (e) {
    reloadAllDataTables();
  });
});

/**
 * @param {string} masterCheckbox Master checkbox selector
 * @param {string} childCheckbox Child checkbox selector
 */
function prepareTableCheckboxes(masterCheckbox, childCheckbox) {
logger.debug(`>>> Entering prepareTableCheckboxes(masterCheckbox=${masterCheckbox},childCheckbox=${childCheckbox})`);
  //Handling the event of clicking on the master checkbox
  $(masterCheckbox).click(function () {
    $(childCheckbox).prop('checked', $(this).prop('checked'));
  });

  //Handling the event of clicking on a child checkbox
  $(childCheckbox).change(function () {
	logger.debug(`>>> Entering change(childCheckbox=${childCheckbox})`);
    $(masterCheckbox).prop('checked', $(childCheckbox + ':not(:checked)').length === 0 ? true : false);
  });

  //Determining the state of the master checkbox by the state of its children
  $(masterCheckbox).prop('checked', $(childCheckbox).length == $(childCheckbox + ':checked').length && $(childCheckbox).length > 0);
logger.debug(`<<< Exiting prepareTableCheckboxes(masterCheckbox=${masterCheckbox},childCheckbox=${childCheckbox})`);
}

function displayBarNotification(message, notifyTypeId, timeout) {
logger.debug(`>>> Entering displayBarNotification(message=${message},notifyTypeId=${notifyTypeId},timeout=${timeout})`);

  var cssStyles = ["alert-success", "alert-danger", "alert-warning"]

logger.debug(`displayBarNotification(message=${message},notifyTypeId=${notifyTypeId},timeout=${timeout}): cssStyles → ${cssStyles}`);
  cssStyle = notifyTypeId >= 0 && notifyTypeId <= 2 ? cssStyles[notifyTypeId] : cssStyles[0];

  var htmlcode = document.createElement('div');
  htmlcode.classList.add('alert', cssStyle, 'alert-dismissable');
logger.debug(`displayBarNotification(message=${message},notifyTypeId=${notifyTypeId},timeout=${timeout}): htmlcode → ${htmlcode}`);

  var button = document.createElement('button');
  button.classList.add('close');
logger.debug(`displayBarNotification(message=${message},notifyTypeId=${notifyTypeId},timeout=${timeout}): button → ${button}`);
  button.setAttribute('data-dismiss', 'alert');
  button.setAttribute('aria-hidden', 'true');
  button.innerHTML = '&times;';
  htmlcode.appendChild(button);

  var html = document.createElement('span');
  html.innerHTML = message;
logger.debug(`displayBarNotification(message=${message},notifyTypeId=${notifyTypeId},timeout=${timeout}): html → ${html}`);
logger.debug(`displayBarNotification(message=${message},notifyTypeId=${notifyTypeId},timeout=${timeout}): button.innerHTML → ${button.innerHTML}`);
  htmlcode.appendChild(html);

  $('#admin-notifications').append(htmlcode);

  //callback for notification removing
  var removeNoteItem = function () {
	logger.debug('>>> Entering removeNoteItem()');
    $(htmlcode).remove();
	logger.debug('<<< Exiting removeNoteItem()');
  };

logger.debug(`displayBarNotification(message=${message},notifyTypeId=${notifyTypeId},timeout=${timeout}): html.innerHTML → ${html.innerHTML}`);
  //timeout (if set)
  if (timeout > 0) {
    var notificationTimeout = setTimeout(function () {
      $(htmlcode).fadeOut('slow', removeNoteItem);
    }, timeout);

    $(htmlcode)
      .fadeIn('slow')
      .on('mouseenter', function () {
	logger.debug(`<<< Exiting displayBarNotification(message=${message},notifyTypeId=${notifyTypeId},timeout=${timeout})`);
        clearTimeout(notificationTimeout);
      });
  }
}