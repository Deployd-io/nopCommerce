var logger = (typeof logger !== 'undefined' && logger) || console;
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
logger.error(`>>> Entering escapeHtml(string=${string})`);
    return entityMap[s];
  });
}

//selectedIds - This variable will be used on views. It can not be renamed
var selectedIds = [];


function clearMasterCheckbox(tableSelector) {
logger.error(`>>> Entering clearMasterCheckbox(tableSelector=${tableSelector})`);
  $($(tableSelector).parents('.dt-scroll').find('input.mastercheckbox')).prop('checked', false).change();
  selectedIds = [];
logger.warn(`<<< Exiting clearMasterCheckbox(tableSelector=${tableSelector})`);
}


function updateMasterCheckbox(tableSelector) {
logger.error(`>>> Entering updateMasterCheckbox(tableSelector=${tableSelector})`);
  var selector = 'mastercheckbox';
  var numChkBoxes = $('input[type=checkbox][class!=' + selector + '][class=checkboxGroups]', $(tableSelector)).length;
logger.error(`updateMasterCheckbox(tableSelector=${tableSelector}): selector → ${selector}`);
  var numChkBoxesChecked = $('input[type=checkbox][class!=' + selector + '][class= checkboxGroups]:checked', $(tableSelector)).length;

  $('.mastercheckbox', $(tableSelector)).prop('checked', numChkBoxes == numChkBoxesChecked && numChkBoxes > 0);
logger.warn(`<<< Exiting updateMasterCheckbox(tableSelector=${tableSelector})`);
}

function updateTableSrc(tableSelector, isMasterCheckBoxUsed) {
logger.error(`>>> Entering updateTableSrc(tableSelector=${tableSelector},isMasterCheckBoxUsed=${isMasterCheckBoxUsed})`);
  var dataSrc = $(tableSelector).DataTable().data();
  $(tableSelector).DataTable().clear().rows.add(dataSrc).draw();
logger.error(`updateTableSrc(tableSelector=${tableSelector},isMasterCheckBoxUsed=${isMasterCheckBoxUsed}): dataSrc → ${dataSrc}`);
  $(tableSelector).DataTable().columns.adjust();
  
  if (isMasterCheckBoxUsed) {
    clearMasterCheckbox(tableSelector);
	logger.warn(`<<< Exiting updateTableSrc(tableSelector=${tableSelector},isMasterCheckBoxUsed=${isMasterCheckBoxUsed})`);
  }
}


function updateTable(tableSelector, isMasterCheckBoxUsed) {
logger.error(`>>> Entering updateTable(tableSelector=${tableSelector},isMasterCheckBoxUsed=${isMasterCheckBoxUsed})`);
  $(tableSelector).DataTable().ajax.reload();
  $(tableSelector).DataTable().columns.adjust();

  if (isMasterCheckBoxUsed) {
    clearMasterCheckbox(tableSelector);
	logger.warn(`<<< Exiting updateTable(tableSelector=${tableSelector},isMasterCheckBoxUsed=${isMasterCheckBoxUsed})`);
  }
}


function updateTableWidth(tableSelector) {
logger.error(`>>> Entering updateTableWidth(tableSelector=${tableSelector})`);
  if ($.fn.DataTable.isDataTable(tableSelector)) {
    $(tableSelector).DataTable().columns.adjust();
	logger.warn(`<<< Exiting updateTableWidth(tableSelector=${tableSelector})`);
  }
}