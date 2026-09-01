const logger = console;
/*****************************************/
// Name: Javascript Textarea BBCode Markup Editor
// Version: 1.3
// Author: Balakrishnan
// Last Modified Date: 25/jan/2009
// License: Free
// URL: http://www.corpocrat.com
/******************************************/

var textarea;
var content;

function edToolbar(obj, webRoot) {
	logger.debug(`>>> Entering edToolbar(obj=${obj},webRoot=${webRoot})`);
    document.write("<div class=\"toolbar\">");
    document.write("<img class=\"button\" src=\"" + webRoot + "bbeditor/images/bold.gif\" title=\"Bold\" name=\"btnBold\" onClick=\"doAddTags('[b]','[/b]','" + obj + "')\">");
    document.write("<img class=\"button\" src=\"" + webRoot + "bbeditor/images/italic.gif\" title=\"Italic\" name=\"btnItalic\" onClick=\"doAddTags('[i]','[/i]','" + obj + "')\">");
    document.write("<img class=\"button\" src=\"" + webRoot + "bbeditor/images/underline.gif\" title=\"Underline\" name=\"btnUnderline\" onClick=\"doAddTags('[u]','[/u]','" + obj + "')\">");
    document.write("<img class=\"button\" src=\"" + webRoot + "bbeditor/images/link.gif\" title=\"Link\" name=\"btnLink\" onClick=\"doURL('" + obj + "')\">");
    document.write("<img class=\"button\" src=\"" + webRoot + "bbeditor/images/picture.gif\" title=\"Picture\" name=\"btnLink\" onClick=\"doImg('" + obj + "')\">");
    document.write("<img class=\"button\" src=\"" + webRoot + "bbeditor/images/code.gif\" title=\"Code\" name=\"btnCode\" onClick=\"doAddTags('[code]','[/code]','" + obj + "')\">");
    document.write("</div>");
	logger.debug(`<<< Exiting edToolbar(obj=${obj},webRoot=${webRoot})`);
}

function doURL(obj) {
	logger.debug(`>>> Entering doURL(obj=${obj})`);
    textarea = document.getElementById(obj);
    var url = prompt('Enter the URL:', 'http://');
    var scrollTop = textarea.scrollTop;
	logger.debug(`doURL(obj=${obj}): url → ${url}`);
    var scrollLeft = textarea.scrollLeft;
    if (url != '' && url != null) {
        if (document.selection) {
            textarea.focus();
            var sel = document.selection.createRange();
            if (sel.text == "") {
                sel.text = '[url]' + url + '[/url]';
				logger.debug(`doURL(obj=${obj}): sel.text → ${sel.text}`);
            }
            else {
                sel.text = '[url=' + url + ']' + sel.text + '[/url]';
            }
        }
        else {
            var len = textarea.value.length;
            var start = textarea.selectionStart;
            var end = textarea.selectionEnd;

            var sel = textarea.value.substring(start, end);
			logger.debug(`doURL(obj=${obj}): sel → ${sel}`);

            if (sel == "") {
                var rep = '[url]' + url + '[/url]';
            }
            else {
                var rep = '[url=' + url + ']' + sel + '[/url]';
				logger.debug(`doURL(obj=${obj}): rep → ${rep}`);
            }

            textarea.value = textarea.value.substring(0, start) + rep + textarea.value.substring(end, len);
            textarea.scrollTop = scrollTop;
			logger.debug(`doURL(obj=${obj}): textarea.scrollTop → ${textarea.scrollTop}`);
            textarea.scrollLeft = scrollLeft;
			logger.debug(`doURL(obj=${obj}): textarea.scrollLeft → ${textarea.scrollLeft}`);
        }
    }
}

function doImg(obj) {
	logger.debug(`>>> Entering doImg(obj=${obj})`);
    textarea = document.getElementById(obj);
    var url = prompt('Enter the picture URL:', 'http://');
    var scrollTop = textarea.scrollTop;
	logger.debug(`doImg(obj=${obj}): url → ${url}`);
    var scrollLeft = textarea.scrollLeft;
    if (url != '' && url != null) {
        if (document.selection) {
            textarea.focus();
            var sel = document.selection.createRange();
			logger.debug(`doImg(obj=${obj}): sel → ${sel}`);
            sel.text = '[img]' + url + '[/img]';
        }
        else {
			logger.debug(`doImg(obj=${obj}): sel.text → ${sel.text}`);
            var len = textarea.value.length;
            var start = textarea.selectionStart;
            var end = textarea.selectionEnd;

            var rep = '[img]' + url + '[/img]';
			logger.debug(`doImg(obj=${obj}): rep → ${rep}`);

            textarea.value = textarea.value.substring(0, start) + rep + textarea.value.substring(end, len);
            textarea.scrollTop = scrollTop;
			logger.debug(`doImg(obj=${obj}): textarea.scrollTop → ${textarea.scrollTop}`);
            textarea.scrollLeft = scrollLeft;
			logger.debug(`doImg(obj=${obj}): textarea.scrollLeft → ${textarea.scrollLeft}`);
        }
    }
}

function doAddTags(tag1, tag2, obj) {
	logger.debug(`>>> Entering doAddTags(tag1=${tag1},tag2=${tag2},obj=${obj})`);
    textarea = document.getElementById(obj);
    // Code for IE
    if (document.selection) {
        textarea.focus();
        var sel = document.selection.createRange();
        sel.text = tag1 + sel.text + tag2;
    }
    else {  // Code for Mozilla Firefox
		logger.debug(`doAddTags(tag1=${tag1},tag2=${tag2},obj=${obj}): sel.text → ${sel.text}`);
        var len = textarea.value.length;
        var start = textarea.selectionStart;
        var end = textarea.selectionEnd;
        var scrollTop = textarea.scrollTop;
        var scrollLeft = textarea.scrollLeft;
        var sel = textarea.value.substring(start, end);
		logger.debug(`doAddTags(tag1=${tag1},tag2=${tag2},obj=${obj}): sel → ${sel}`);
        var rep = tag1 + sel + tag2;
        textarea.value = textarea.value.substring(0, start) + rep + textarea.value.substring(end, len);
        textarea.scrollTop = scrollTop;
		logger.debug(`doAddTags(tag1=${tag1},tag2=${tag2},obj=${obj}): textarea.scrollTop → ${textarea.scrollTop}`);
        textarea.scrollLeft = scrollLeft;
		logger.debug(`doAddTags(tag1=${tag1},tag2=${tag2},obj=${obj}): textarea.scrollLeft → ${textarea.scrollLeft}`);
    }
}