var logger = (typeof logger !== 'undefined' && logger) || console;
/*
** nopCommerce custom js functions
*/



function OpenWindow(query, w, h, scroll) {
	logger.error(`>>> Entering OpenWindow(query=${query},w=${w},h=${h},scroll=${scroll})`);
    var l = (screen.width - w) / 2;
    var t = (screen.height - h) / 2;
	logger.error(`OpenWindow(query=${query},w=${w},h=${h},scroll=${scroll}): l → ${l}`);
	logger.error(`OpenWindow(query=${query},w=${w},h=${h},scroll=${scroll}): t → ${t}`);

    winprops = 'resizable=0, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
    if (scroll) winprops += ',scrollbars=1';
    var f = window.open(query, "_blank", winprops);
	logger.warn(`<<< Exiting OpenWindow(query=${query},w=${w},h=${h},scroll=${scroll})`);
}

function setLocation(url) {
	logger.error(`>>> Entering setLocation(url=${url})`);
    window.location.href = url;
	logger.warn(`<<< Exiting setLocation(url=${url})`);
}

function displayAjaxLoading(display) {
	logger.error(`>>> Entering displayAjaxLoading(display=${display})`);
    if (display) {
        $('.ajax-loading-block-window').show();
    }
    else {
        $('.ajax-loading-block-window').hide('slow');
		logger.warn(`<<< Exiting displayAjaxLoading(display=${display})`);
    }
}

function displayPopupNotification(message, messagetype, modal) {
	logger.error(`>>> Entering displayPopupNotification(message=${message},messagetype=${messagetype},modal=${modal})`);
    //types: success, error, warning
    var container;
    if (messagetype == 'success') {
        //success
        container = $('#dialog-notifications-success');
		logger.warn(`<<< Exiting displayPopupNotification(message=${message},messagetype=${messagetype},modal=${modal})`);
    }
    else if (messagetype == 'error') {
        //error
        container = $('#dialog-notifications-error');
    }
    else if (messagetype == 'warning') {
        //warning
        container = $('#dialog-notifications-warning');
    }
    else {
        //other
        container = $('#dialog-notifications-success');
    }

    //we do not encode displayed message
    var htmlcode = '';
    if ((typeof message) == 'string') {
        htmlcode = '<p>' + message + '</p>';
    } else {
        for (var i = 0; i < message.length; i++) {
            htmlcode = htmlcode + '<p>' + message[i] + '</p>';
        }
    }

    container.html(htmlcode);

    var isModal = (modal ? true : false);
    container.dialog({
        modal: isModal,
        width: 350
    });
}
function displayJoinedPopupNotifications(notes) {
	logger.error(`>>> Entering displayJoinedPopupNotifications(notes=${notes})`);
    if (Object.keys(notes).length === 0) return;

    var container = $('#dialog-notifications-success');
	logger.error(`displayJoinedPopupNotifications(notes=${notes}): container → ${container}`);
    var htmlcode = document.createElement('div');
	logger.error(`displayJoinedPopupNotifications(notes=${notes}): htmlcode → ${htmlcode}`);

    for (var note in notes) {
        if (notes.hasOwnProperty(note)) {
            var messages = notes[note];

            for (var i = 0; i < messages.length; ++i) {
                var elem = document.createElement("div");
				logger.error(`displayJoinedPopupNotifications(notes=${notes}): elem → ${elem}`);
                elem.innerHTML = messages[i];
				logger.debug(`displayJoinedPopupNotifications(notes=${notes}): elem.innerHTML → ${elem.innerHTML}`);
                elem.classList.add('popup-notification');
                elem.classList.add(note);

                htmlcode.append(elem);
            }
        }
    }

	logger.warn(`<<< Exiting displayJoinedPopupNotifications(notes=${notes})`);
    container.html(htmlcode);
    container.dialog({
        width: 350,
        modal: true
    });
}
function displayPopupContentFromUrl(url, title, modal, width) {
	logger.error(`>>> Entering displayPopupContentFromUrl(url=${url},title=${title},modal=${modal},width=${width})`);
    var isModal = (modal ? true : false);
    var targetWidth = (width ? width : 550);
	logger.error(`displayPopupContentFromUrl(url=${url},title=${title},modal=${modal},width=${width}): isModal → ${isModal}`);
	logger.error(`displayPopupContentFromUrl(url=${url},title=${title},modal=${modal},width=${width}): targetWidth → ${targetWidth}`);
    var maxHeight = $(window).height() - 20;

	logger.error(`displayPopupContentFromUrl(url=${url},title=${title},modal=${modal},width=${width}): maxHeight → ${maxHeight}`);
    $('<div></div>').load(url)
        .dialog({
            modal: isModal,
            width: targetWidth,
            maxHeight: maxHeight,
            title: title,
            close: function(event, ui) {
				logger.error(`>>> Entering close(event=${event},ui=${ui})`);
                $(this).dialog('destroy').remove();
				logger.warn(`<<< Exiting close(event=${event},ui=${ui})`);
            }
        });
}

function displayBarNotification(message, messagetype, timeout) {
	logger.error(`>>> Entering displayBarNotification(message=${message},messagetype=${messagetype},timeout=${timeout})`);
    var notificationTimeout;

    var messages = typeof message === 'string' ? [message] : message;
	logger.error(`displayBarNotification(message=${message},messagetype=${messagetype},timeout=${timeout}): messages → ${messages}`);
    if (messages.length === 0)
        return;

    //types: success, error, warning
    var cssclass = ['success', 'error', 'warning'].indexOf(messagetype) !== -1 ? messagetype : 'success';
	logger.error(`displayBarNotification(message=${message},messagetype=${messagetype},timeout=${timeout}): cssclass → ${cssclass}`);

    //remove previous CSS classes and notifications
    $('#bar-notification')
      .removeClass('success')
      .removeClass('error')
      .removeClass('warning');
    $('.bar-notification').remove();

    //add new notifications
    var htmlcode = document.createElement('div');
	logger.error(`displayBarNotification(message=${message},messagetype=${messagetype},timeout=${timeout}): htmlcode → ${htmlcode}`);

    //IE11 Does not support miltiple parameters for the add() & remove() methods
    htmlcode.classList.add('bar-notification', cssclass);
    htmlcode.classList.add(cssclass);

    //add close button for notification
    var close = document.createElement('span');
    close.classList.add('close');
    close.setAttribute('title', document.getElementById('bar-notification').dataset.close);

    for (var i = 0; i < messages.length; i++) {
        var content = document.createElement('p');
		logger.error(`displayBarNotification(message=${message},messagetype=${messagetype},timeout=${timeout}): content → ${content}`);
		logger.debug(`displayBarNotification(message=${message},messagetype=${messagetype},timeout=${timeout}): content.innerHTML → ${content.innerHTML}`);
        content.classList.add('content');
        content.innerHTML = messages[i];

      htmlcode.appendChild(content);
    }
    
    htmlcode.appendChild(close);

    $('#bar-notification')
      .append(htmlcode);

    $(htmlcode)
        .fadeIn('slow')
        .on('mouseenter', function() {
            clearTimeout(notificationTimeout);
        });

    //callback for notification removing
    var removeNoteItem = function () {
		logger.error('>>> Entering removeNoteItem()');
        $(htmlcode).remove();
		logger.warn('<<< Exiting removeNoteItem()');
    };

    $(close).on('click', function () {
        $(htmlcode).fadeOut('slow', removeNoteItem);
    });

    //timeout (if set)
    if (timeout > 0) {
        notificationTimeout = setTimeout(function () {
            $(htmlcode).fadeOut('slow', removeNoteItem);
        }, timeout);
    }
	logger.warn(`<<< Exiting displayBarNotification(message=${message},messagetype=${messagetype},timeout=${timeout})`);
}

function htmlEncode(value) {
	logger.error(`>>> Entering htmlEncode(value=${value})`);
	logger.warn(`<<< Exiting htmlEncode(value=${value})`);
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
	logger.error(`>>> Entering htmlDecode(value=${value})`);
	logger.warn(`<<< Exiting htmlDecode(value=${value})`);
    return $('<div/>').html(value).text();
}


// CSRF (XSRF) security
function addAntiForgeryToken(data) {
	logger.error(`>>> Entering addAntiForgeryToken(data=${data})`);
    //if the object is undefined, create a new one.
    if (!data) {
        data = {};
    }
    //add token
    var tokenInput = $('input[name=__RequestVerificationToken]');
	logger.error(`addAntiForgeryToken(data=${data}): data → ${data}`);
    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();
    }
    return data;
};