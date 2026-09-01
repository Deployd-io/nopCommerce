const logger = console;
var entityMap = {
  '&': '&amp;',
  '<': '&lt;',
  '>': '&gt;',
  '"': '&quot;',
  "'": '&#39;',
  '/': '&#x2F;',
  '`': '&#x60;',
  '=': '&#x3D;'
};

function escapeHtml(string) {
  if (string == null) {
    return '';
  }
  return String(string).replace(/[&<>"'`=\/]/g, function (s) {
logger.debug(`<<< Exiting escapeHtml(string=${string})`);
    return entityMap[s];
  });
}

//selectedIds - This variable will be used on views. It can not be renamed
var selectedIds = [];


function clearMasterCheckbox(tableSelector) {
logger.debug(`>>> Entering clearMasterCheckbox(tableSelector=${tableSelector})`);
  $($(tableSelector).parents('.dt-scroll').find('input.mastercheckbox')).prop('checked', false).change();
  selectedIds = [];
logger.debug(`<<< Exiting clearMasterCheckbox(tableSelector=${tableSelector})`);
}


function updateMasterCheckbox(tableSelector) {
logger.debug(`>>> Entering updateMasterCheckbox(tableSelector=${tableSelector})`);
  var selector = 'mastercheckbox';
  var numChkBoxes = $('input[type=checkbox][class!=' + selector + '][class=checkboxGroups]', $(tableSelector)).length;
logger.debug(`updateMasterCheckbox(tableSelector=${tableSelector}): selector → ${selector}`);
  var numChkBoxesChecked = $('input[type=checkbox][class!=' + selector + '][class= checkboxGroups]:checked', $(tableSelector)).length;

  $('.mastercheckbox', $(tableSelector)).prop('checked', numChkBoxes == numChkBoxesChecked && numChkBoxes > 0);
logger.debug(`<<< Exiting updateMasterCheckbox(tableSelector=${tableSelector})`);
}

function updateTableSrc(tableSelector, isMasterCheckBoxUsed) {
logger.debug(`>>> Entering updateTableSrc(tableSelector=${tableSelector},isMasterCheckBoxUsed=${isMasterCheckBoxUsed})`);
  var dataSrc = $(tableSelector).DataTable().data();
  $(tableSelector).DataTable().clear().rows.add(dataSrc).draw();
logger.debug(`updateTableSrc(tableSelector=${tableSelector},isMasterCheckBoxUsed=${isMasterCheckBoxUsed}): dataSrc → ${dataSrc}`);
  $(tableSelector).DataTable().columns.adjust();
  
  if (isMasterCheckBoxUsed) {
    clearMasterCheckbox(tableSelector);
	logger.debug(`<<< Exiting updateTableSrc(tableSelector=${tableSelector},isMasterCheckBoxUsed=${isMasterCheckBoxUsed})`);
  }
}


function updateTable(tableSelector, isMasterCheckBoxUsed) {
logger.debug(`>>> Entering updateTable(tableSelector=${tableSelector},isMasterCheckBoxUsed=${isMasterCheckBoxUsed})`);
  $(tableSelector).DataTable().ajax.reload();
  $(tableSelector).DataTable().columns.adjust();

  if (isMasterCheckBoxUsed) {
    clearMasterCheckbox(tableSelector);
	logger.debug(`<<< Exiting updateTable(tableSelector=${tableSelector},isMasterCheckBoxUsed=${isMasterCheckBoxUsed})`);
  }
}


function updateTableWidth(tableSelector) {
logger.debug(`>>> Entering updateTableWidth(tableSelector=${tableSelector})`);
  if ($.fn.DataTable.isDataTable(tableSelector)) {
    $(tableSelector).DataTable().columns.adjust();
	logger.debug(`<<< Exiting updateTableWidth(tableSelector=${tableSelector})`);
  }
}